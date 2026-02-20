namespace Jobby.Data.enums
{
    public enum InterviewType
    {
        Phone,
        Video,
        Onsite
    }
    public enum InterviewStage
    {
        Screening,
        Tech,
        HR,
        Final,
        Other
    }
    public enum InterviewStatus
    {
        Scheduled,
        Completed,
        Cancelled,
        NoShow
    }
    public enum InterviewOutcome
    {
        Passed,
        Failed,
        OnHold
    }
}
