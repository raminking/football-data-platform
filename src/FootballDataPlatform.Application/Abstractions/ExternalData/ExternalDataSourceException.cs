namespace FootballDataPlatform.Application.Abstractions.ExternalData;

public sealed class ExternalDataSourceException : Exception
{
    public string SourceKey { get; }
    public ExternalDataErrorCategory Category { get; }
    public int? HttpStatusCode { get; }
    public TimeSpan? RetryAfter { get; }

    public ExternalDataSourceException(
        string sourceKey,
        ExternalDataErrorCategory category,
        string message,
        int? httpStatusCode = null,
        TimeSpan? retryAfter = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        SourceKey = sourceKey;
        Category = category;
        HttpStatusCode = httpStatusCode;
        RetryAfter = retryAfter;
    }
}