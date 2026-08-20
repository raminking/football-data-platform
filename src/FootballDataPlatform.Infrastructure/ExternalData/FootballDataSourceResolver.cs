using FootballDataPlatform.Application.Abstractions.ExternalData;

namespace FootballDataPlatform.Infrastructure.ExternalData;

public sealed class FootballDataSourceResolver(IEnumerable<IFootballDataSource> sources) : IFootballDataSourceResolver
{
    private readonly IReadOnlyDictionary<string, IFootballDataSource> _sources =
        sources.ToDictionary(x => x.SourceKey, StringComparer.OrdinalIgnoreCase);

    public IFootballDataSource Resolve(string sourceKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceKey);

        if (_sources.TryGetValue(sourceKey.Trim(), out var source))
            return source;

        throw new KeyNotFoundException($"Football data source '{sourceKey}' is not registered.");
    }
}
