using Microsoft.AspNetCore.Mvc;
using DotNet9.BlockingStartup.Api.Services;

namespace DotNet9.BlockingStartup.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ILogger<ProductController> logger, IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        logger.LogInformation("Product endpoint called");
        
        var summary = await productService.GetProductSummaryAsync();
        
        return Ok(new 
        { 
            Message = "This endpoint is only available AFTER the bulk products sync (10,000+ products) completes",
            Timestamp = DateTime.Now,
            DatabaseStatus = new
            {
                summary.TotalProducts,
                summary.TotalCategories,
                summary.AveragePrice,
                summary.Status
            }
        });
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var status = await productService.GetProductStatusAsync();

        return Ok(new 
        { 
            Status = "Product API is running",
            Time = DateTime.Now,
            DatabaseInfo = new
            {
                status.TotalProducts,
                status.LastUpdated,
                status.Note
            }
        });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetProductsStats()
    {
        var stats = await productService.GetProductStatsAsync();

        return Ok(new
        {
            stats.Message,
            stats.TotalProducts,
            stats.Categories,
            stats.TopCategories,
            stats.PriceDistribution,
            stats.Note
        });
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await productService
            .GetCategoriesAsync();

        return Ok(new
        {
            Categories = categories,
            Count = categories.Count(),
            Note = "Product categories loaded after bulk sync completes"
        });
    }

    [HttpGet("sample")]
    public async Task<IActionResult> GetSampleProducts()
    {
        var sampleProducts = await productService.GetSampleProductsAsync();

        if (!sampleProducts.Any())
        {
            return Ok(new
            {
                Message = "No products available yet. Bulk sync still in progress or failed.",
                Products = Array.Empty<object>()
            });
        }

        return Ok(new
        {
            Message = "Sample products from database",
            Products = sampleProducts
        });
    }
}
