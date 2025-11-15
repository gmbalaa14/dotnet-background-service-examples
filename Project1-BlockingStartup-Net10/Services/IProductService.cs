using DotNet10.NonBlockingStartup.Api.DataContracts;

namespace DotNet10.NonBlockingStartup.Api.Services;

public interface IProductService
{
    Task<SyncResult> SyncProductsFromApiAsync(CancellationToken stoppingToken);
    Task<ProductSummaryDto> GetProductSummaryAsync();
    Task<ProductStatusDto> GetProductStatusAsync();
    Task<ProductStatsDto> GetProductStatsAsync();
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task<IEnumerable<ProductSampleDto>> GetSampleProductsAsync();
}
