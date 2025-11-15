using System.Text.Json;
using DotNet.BackgroundService.Shared.Data;
using DotNet.BackgroundService.Shared.DataContracts;
using DotNet.BackgroundService.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DotNet.BackgroundService.Shared.Services;

public class ProductService(ProductDbContext context, HttpClient httpClient, ILogger<ProductService> logger) : IProductService
{
    public async Task<SyncResult> SyncProductsFromApiAsync(CancellationToken stoppingToken)
    {
        var result = new SyncResult();
        var startTime = DateTime.Now;

        try
        {
            logger.LogInformation("Starting bulk products sync from Fake Store API...");
            result.StartedAt = startTime;

            // Fetch products from multiple pages to get 10,000+ products
            await FetchAndSaveProductsAsync(stoppingToken);

            result.CompletedAt = DateTime.Now;
            result.Duration = result.CompletedAt - startTime;
            result.TotalProductsSynced = await context.Products.CountAsync(stoppingToken);

            logger.LogWarning($"Bulk products sync completed in {result.Duration.TotalSeconds:F2} seconds");
            logger.LogInformation($"Total products in database: {result.TotalProductsSynced:N0}");
            
            return result;
        }
        catch (Exception ex)
        {
            result.CompletedAt = DateTime.Now;
            result.Duration = result.CompletedAt - startTime;
            result.ErrorMessage = ex.Message;
            logger.LogError(ex, "Error during bulk products sync");
            throw;
        }
    }

    public async Task<ProductSummaryDto> GetProductSummaryAsync()
    {
        var productsCount = await context.Products.CountAsync();
        var categories = await context.Products.Select(p => p.Category).Distinct().CountAsync();
        var avgPrice = await context.Products.AnyAsync()
            ? (double)await context.Products.AverageAsync(p => p.Price)
            : 0.0;

        return new ProductSummaryDto
        {
            TotalProducts = productsCount,
            TotalCategories = categories,
            AveragePrice = Math.Round(avgPrice, 2),
            Status = productsCount > 0 ? "Products loaded successfully" : "Still loading or failed to load products"
        };
    }

    public async Task<ProductStatusDto> GetProductStatusAsync()
    {
        var productsCount = await context.Products.CountAsync();
        var lastProduct = await context.Products.OrderByDescending(p => p.UpdatedAt).FirstOrDefaultAsync();

        return new ProductStatusDto
        {
            TotalProducts = productsCount,
            LastUpdated = lastProduct?.UpdatedAt,
            Note = productsCount > 0 
                ? $"Database contains {productsCount:N0} products. This endpoint was only reachable AFTER the bulk products sync completed in .NET 9."
                : "Database is empty or still loading products from external API"
        };
    }

    public async Task<ProductStatsDto> GetProductStatsAsync()
    {
        if (!await context.Products.AnyAsync())
        {
            return new ProductStatsDto
            {
                Message = "No products loaded yet. The bulk products sync from Fake Store API is still in progress or failed.",
                Note = "In .NET 9, the entire application startup is blocked until all products are fetched and saved to SQLite database.",
                ProductsCount = 0,
                EstimatedTime = "10-15+ seconds for 10,000+ products"
            };
        }

        var stats = await context.Products
            .GroupBy(p => p.Category)
            .Select(g => new CategoryStat { Category = g.Key, Count = g.Count(), AvgPrice = (double)g.Average(p => p.Price) })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        var priceRanges = await context.Products
            .Select(p => new 
            {
                p.Price,
                PriceRange = p.Price <= 10 ? "Under $10" :
                            p.Price <= 50 ? "$10-$50" :
                            p.Price <= 100 ? "$50-$100" : "Over $100"
            })
            .GroupBy(p => p.PriceRange)
            .Select(g => new PriceRangeStat { PriceRange = g.Key, Count = g.Count() })
            .ToListAsync();

        return new ProductStatsDto
        {
            Message = "Products statistics - Only available after bulk sync completes",
            TotalProducts = await context.Products.CountAsync(),
            Categories = stats.Count,
            TopCategories = stats,
            PriceDistribution = priceRanges,
            Note = "This demonstrates .NET 9's blocking startup behavior - the API was not available until all products were processed."
        };
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        return await context.Products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductSampleDto>> GetSampleProductsAsync()
    {
        if (!await context.Products.AnyAsync())
        {
            return Array.Empty<ProductSampleDto>();
        }

        return await context.Products
            .Take(5)
            .Select(p => new ProductSampleDto
            {
                Id = p.Id,
                Title = p.Title,
                Category = p.Category,
                Price = p.Price,
                Rating = p.Rating,
                RatingCount = p.RatingCount
            })
            .ToListAsync();
    }

    private async Task FetchAndSaveProductsAsync(CancellationToken stoppingToken)
    {
        const int batchSize = 10; // Platzi API limit per request
        const int targetProducts = 150; // Smaller batch for blocking startup demo
        var totalFetched = 0;

        for (var page = 0; totalFetched < targetProducts; page++)
        {
            var offset = page * batchSize;
            logger.LogInformation($"Fetching products page {page + 1} (offset: {offset}, limit: {batchSize})...");
            
            try
            {
                // Call Platzi Fake Store API
                var response = await httpClient.GetAsync($"https://api.escuelajs.co/api/v1/products?limit={batchSize}&offset={offset}", stoppingToken);
                
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning($"API call failed for page {page}: {response.StatusCode}");
                    break;
                }

                var content = await response.Content.ReadAsStringAsync(stoppingToken);
                var apiProducts = JsonSerializer.Deserialize<List<ApiProduct>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiProducts == null || !apiProducts.Any())
                {
                    logger.LogInformation($"No more products available. Total fetched: {totalFetched}");
                    break;
                }

                // Transform API products to our entity model
                var products = apiProducts.Select(apiProduct => new Product
                {
                    Title = apiProduct.Title,
                    Description = apiProduct.Description,
                    Category = apiProduct.Category.Name,
                    Price = (decimal)apiProduct.Price,
                    ImageUrl = apiProduct.Images.FirstOrDefault() ?? apiProduct.Category.Image,
                    Rating = 0.0, // Platzi API doesn't provide rating data
                    RatingCount = 0, // Platzi API doesn't provide rating count
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

                // Save in batches to database
                logger.LogInformation($"Saving batch of {products.Count} products to database...");
                context.Products.AddRange(products);
                await context.SaveChangesAsync(stoppingToken);

                totalFetched += products.Count;
                logger.LogInformation($"Batch saved successfully. Total products: {totalFetched:N0}");

                // Add delay to simulate processing time - shorter for demo
                if (totalFetched < targetProducts)
                {
                    await Task.Delay(10_000, stoppingToken); // 10 second delay per batch
                }
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("Products sync was cancelled");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error fetching products from page {page}");
                throw;
            }
        }

        logger.LogInformation($"Products sync completed. Total products synced: {totalFetched:N0}");
    }
}
