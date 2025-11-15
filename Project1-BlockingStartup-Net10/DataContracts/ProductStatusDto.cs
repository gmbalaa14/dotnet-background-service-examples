namespace DotNet10.NonBlockingStartup.Api.DataContracts;

public record ProductStatusDto
{
    public int TotalProducts { get; init; }
    public DateTime? LastUpdated { get; init; }
    public string Note { get; init; } = string.Empty;
}