namespace DotNet.BackgroundService.Shared.DataContracts;

public record ProductSampleDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public double Rating { get; init; }
    public int RatingCount { get; init; }
}