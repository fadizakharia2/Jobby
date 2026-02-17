using System.Security.Claims;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Jobby.Data.context;
using Jobby.Data.entities;
using Jobby.Data.enums;
using Jobby.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Jobby.Controllers
{
    [Route("api/applications/{applicationId:guid}/files")]
    [ApiController]
    public class ApplicationFilesController(BlobServiceClient blob,AppDbContext db, IConfiguration cfg, IAuthorizationService auth) : ControllerBase
    {

        [Authorize]
        [HttpGet("{fileId:guid}/sas")]
        public async Task<ActionResult> GetReadSas(Guid applicationId, Guid fileId, CancellationToken ct)
        {
            var row = await db.ApplicationFiles.Where(x => x.ApplicationId == applicationId && x.FileId == fileId).Join(db.Files.AsNoTracking(), af => af.FileId, f => f.Id, (af, f) =>
            new {
            af.ApplicationId,
            f.Id,
            f.StorageKey,
            f.OriginalName,
            f.ContentType
            }).FirstOrDefaultAsync(ct);
            var minutes = Math.Clamp(10, 1, 30);
            if (row == null)
            {
                return NotFound("File not found for this application.");
            }
            var application = await db.JobApplications
                .AsNoTracking()
                .Select(a => new { a.Id, a.OrganizationId })
                .FirstOrDefaultAsync(a => a.Id == applicationId, ct);

            if (application is null)
            {
                return NotFound("Application not found.");
            }
            var foundOrganization = await db.Organizations.FirstOrDefaultAsync(x => x.Id == application.OrganizationId, ct);
            if (foundOrganization is null)
            {
                return NotFound("Organization for the application not found");
            }
            var ok = await auth.AuthorizeAsync(User, new Dictionary<string, Guid> { ["applicationId"]=row.ApplicationId,["organizationId"]=foundOrganization.Id}, "OrgCandidate");
            if (!ok.Succeeded) return Forbid();

            var containerName = cfg["Azure:Container"];
            var container = blob.GetBlobContainerClient(containerName);
            var blobClient = container.GetBlobClient(row.StorageKey);
            var expiresOn = DateTimeOffset.UtcNow.AddMinutes(minutes);
            var sasUrl = await CreateReadSasUrlAsync(blobClient, container.Name, row.StorageKey, row.OriginalName, expiresOn, ct);

            return Ok(new { url = sasUrl, expiresAt = expiresOn });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UploadFile(Guid applicationId, [FromQuery] FilePurpose Purpose, [FromForm] IFormFile file, CancellationToken ct)
        {
         
            var allowedTypes = new HashSet<string> { "image/png", "image/jpeg", "Application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
            if (string.IsNullOrWhiteSpace(file.ContentType) || !allowedTypes.Contains(file.ContentType))
            {
                return BadRequest("Invalid type of document");
            }
            var foundApplication = await db.JobApplications.FirstOrDefaultAsync(elem => elem.Id == applicationId, ct);
            if (foundApplication == null)
            {
                return BadRequest("Application id is invalid");
            }
            var ext = Path.GetExtension(file.FileName);
            var container = blob.GetBlobContainerClient(cfg["AzureStorage:Container"]);
            await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);
            var blobName = $"applcations/{applicationId}/{Guid.NewGuid()}{ext}";
            var blobClient = container.GetBlobClient(blobName);
            await using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = { ContentType = file.ContentType },
                    Metadata = new Dictionary<string, string>()
                    {
                        ["originalName"]=file.FileName
                    }
                },ct);
            }
            var fileData = new Files()
            {
                Id = Guid.NewGuid(),
                ContentType = file.ContentType,
                OriginalName = file.FileName,
                OwnerUser = foundApplication.CandidateUser,
                SizeBytes = file.Length,
                StorageKey = blobName,
                CreatedAt = DateTimeOffset.UtcNow
            };
            db.Files.Add(fileData);

            var applicationFilesData = new ApplicationFiles()
            {
                Id = Guid.NewGuid(),
                ApplicationId = foundApplication.Id,
                FileId = fileData.Id,
                FilePurpose = Purpose,
                CreatedAt= DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            db.ApplicationFiles.Add(applicationFilesData);

            await db.SaveChangesAsync(ct);

            return Ok("file uploaded successfully");
        }
        private async Task<string> CreateReadSasUrlAsync(
       BlobClient blobClient,
       string containerName,
       string blobName,
       string downloadFileName,
       DateTimeOffset expiresOn,
       CancellationToken ct)
        {
            if (blobClient.CanGenerateSasUri)
            {
                var sas = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    BlobName = blobName,
                    Resource = "b",
                    ExpiresOn = expiresOn,
                    Protocol = SasProtocol.Https
                };
                sas.SetPermissions(BlobSasPermissions.Read);
                sas.ContentDisposition = $"attachment; filename=\"{downloadFileName}\"";

                return blobClient.GenerateSasUri(sas).ToString();
            }
            var startsOn = DateTimeOffset.UtcNow.AddMinutes(-2);

            var delegationKey = await blob.GetUserDelegationKeyAsync(startsOn, expiresOn, ct);
            var builder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                StartsOn = startsOn,
                ExpiresOn = expiresOn,
                Protocol = SasProtocol.Https,
                ContentDisposition = $"attachment; filename=\"{downloadFileName}\""
            };
            builder.SetPermissions(BlobSasPermissions.Read);

            var sasQuery = builder.ToSasQueryParameters(delegationKey.Value, blob.AccountName);

            return new UriBuilder(blobClient.Uri)
            {
                Query = sasQuery.ToString()
            }.Uri.ToString();
        }

    }
}
