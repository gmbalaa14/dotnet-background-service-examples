namespace DotNet10.NonBlockingStartup.Api.DataContracts;

public record ProductSummaryDto
{
    public int TotalProducts { get; init; }
    public int TotalCategories { get; init; }
    public double AveragePrice { get; init; }
    public string Status { get; init; } = string.Empty;
}