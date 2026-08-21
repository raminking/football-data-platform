namespace FootballDataPlatform.Application.Abstractions.ExternalData;

public enum ExternalDataErrorCategory
{
    Authentication,
    RateLimited,
    Timeout,
    Network,
    ServerError,
    InvalidResponse,
    InvalidData
}