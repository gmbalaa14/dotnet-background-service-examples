using DotNet9.BlockingStartup.Api.DataContracts;

namespace DotNet9.BlockingStartup.Api.Services;

public interface IProductService
{
    Task<SyncResult> SyncProductsFromApiAsync(CancellationToken stoppingToken);
    Task<ProductSummaryDto> GetProductSummaryAsync();
    Task<ProductStatusDto> GetProductStatusAsync();
    Task<ProductStatsDto> GetProductStatsAsync();
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task<IEnumerable<ProductSampleDto>> GetSampleProductsAsync();
}
