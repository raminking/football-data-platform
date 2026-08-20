namespace FootballDataPlatform.Application.Abstractions.ExternalData;

public interface IFootballDataSourceResolver
{
    IFootballDataSource Resolve(string sourceKey);
}
