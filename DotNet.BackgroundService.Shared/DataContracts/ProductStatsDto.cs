namespace DotNet.BackgroundService.Shared.DataContracts;

public record ProductStatsDto
{
    public string Message { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
    public int ProductsCount { get; init; }
    public string EstimatedTime { get; init; } = string.Empty;
    public int TotalProducts { get; init; }
    public int Categories { get; init; }
    public IEnumerable<CategoryStat> TopCategories { get; init; } = [];
    public IEnumerable<PriceRangeStat> PriceDistribution { get; init; } = [];
}

public record CategoryStat
{
    public string Category { get; init; } = string.Empty;
    public int Count { get; init; }
    public double AvgPrice { get; init; }
}

public record PriceRangeStat
{
    public string PriceRange { get; init; } = string.Empty;
    public int Count { get; init; }
}