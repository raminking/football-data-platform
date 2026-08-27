namespace FootballDataPlatform.Infrastructure.ExternalData;

public sealed class ExternalDataRetryOptions
{
    public const string SectionName = "ExternalData:Retry";

    public int MaxRetries { get; init; } = 2;
    public int BaseDelaySeconds { get; init; } = 1;
    public int MaxDelaySeconds { get; init; } = 30;
    public double JitterRatio { get; init; } = 0.25;
}
