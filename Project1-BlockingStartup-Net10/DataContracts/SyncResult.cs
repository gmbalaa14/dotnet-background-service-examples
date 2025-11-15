namespace DotNet10.NonBlockingStartup.Api.DataContracts;

public class SyncResult
{
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public TimeSpan Duration { get; set; }
    public int TotalProductsSynced { get; set; }
    public string? ErrorMessage { get; set; }
}