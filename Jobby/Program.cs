using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using Azure.Identity;
using Azure.Storage.Blobs;
using FluentValidation;
using Jobby.Auth;
using Jobby.Data.context;
using Jobby.Data.entities;
using Jobby.Dtos.Validations.Organization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}); ;
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",   // Angular dev
                "http://localhost:5173",   // Vite (if used)
                "https://yourdomain.com"   // Production frontend
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // REQUIRED if using cookies
    });
});
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddDataProtection();
builder.Services
    .AddIdentityCore<User>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAutoMapper(cfg => { },
    typeof(Jobby.Dtos.Profiles.OrganizationProfile).Assembly);

builder.Services.AddValidatorsFromAssemblyContaining<CreateOrganizationValidator>();

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
        };
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("OrgAdmin", policy => policy.Requirements.Add(
               new OrgRoleRequirement("ADMIN")))
    .AddPolicy("OrgRecruiter", policy => policy.Requirements.Add(
            new OrgRoleRequirement("ADMIN", "RECRUITER")))
    .AddPolicy("OrgCandidate",policy => policy.Requirements.Add(
            new ApplicationRoleRequirement("ADMIN","RECRUITER","CANDIDATE")
        ));
if (builder.Environment.IsProduction())
{
    builder.Services.AddSingleton(x => {
        var cfg = x.GetRequiredService<IConfiguration>();
        var accountName = cfg["AzureStorage:AccountName"];
        var serviceUri = $"https://{accountName}.blob.core.windows.net";
        return new BlobServiceClient(new Uri(serviceUri), new DefaultAzureCredential());
    });
} else {
    builder.Services.AddSingleton(x =>
    {
        var cfg = x.GetRequiredService<IConfiguration>();
        var connectionString = cfg["AzureStorage:ConnectionString"];
        return new BlobServiceClient(connectionString);
    });
}
builder.Services.AddScoped<IAuthorizationHandler, OrgRoleHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ApplicationRoleHandler>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.seed();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
