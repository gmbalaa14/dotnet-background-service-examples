using DotNet.BackgroundService.Shared.DataContracts;

namespace DotNet.BackgroundService.Shared.Services;

public interface IProductService
{
    Task<SyncResult> SyncProductsFromApiAsync(CancellationToken stoppingToken);
    Task<ProductSummaryDto> GetProductSummaryAsync();
    Task<ProductStatusDto> GetProductStatusAsync();
    Task<ProductStatsDto> GetProductStatsAsync();
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task<IEnumerable<ProductSampleDto>> GetSampleProductsAsync();
}
