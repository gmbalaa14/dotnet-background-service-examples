using System.Diagnostics;
using DotNet9.BlockingStartup.Api.Data;
using DotNet9.BlockingStartup.Api.Services;
using DotNet9.BlockingStartup.Api.Services.Background;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add EF Core with local SQLite database
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlite("Data Source=products.db"));

// Add services
builder.Services.AddHttpClient();

// Register product service as scoped with interface
builder.Services.AddScoped<IProductService, ProductService>();

// Add our blocking startup service
builder.Services.AddSingleton<BlockingStartupService>();
builder.Services.AddHostedService<BlockingStartupService>();

var app = builder.Build();

// Clean up database at startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    context.Products.RemoveRange(context.Products);
    context.SaveChanges();
    Console.WriteLine("🗄️ Database cleaned - all existing products removed");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Console.WriteLine("=== .NET 9 Blocking Startup Demo - BackgroundService Implementation ===");
Console.WriteLine($"Starting application at: {DateTime.Now:HH:mm:ss.fff}");
Console.WriteLine("🔒 BLOCKING: BackgroundService will block startup until products sync completes...");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
