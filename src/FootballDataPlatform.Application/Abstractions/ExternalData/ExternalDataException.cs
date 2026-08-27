namespace FootballDataPlatform.Application.Abstractions.ExternalData;

public sealed class ExternalDataException : Exception
{
    public ExternalDataException(
        ExternalDataErrorCategory category,
        string sourceKey,
        string operation,
        string message,
        int? statusCode = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Category = category;
        SourceKey = sourceKey;
        Operation = operation;
        StatusCode = statusCode;
    }

    public ExternalDataErrorCategory Category { get; }
    public string SourceKey { get; }
    public string Operation { get; }
    public int? StatusCode { get; }
}
