namespace DotNet9.BlockingStartup.Api.DataContracts;

public class ApiProduct
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CategoryInfo Category { get; set; } = new CategoryInfo();
    public List<string> Images { get; set; } = new List<string>();
    public double Price { get; set; }
    public DateTime CreationAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CategoryInfo
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public DateTime CreationAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}