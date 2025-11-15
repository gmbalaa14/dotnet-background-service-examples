namespace DotNet9.BlockingStartup.Api.Services.Background;

// This demonstrates blocking startup behavior in .NET 9 with realistic startup checks
public class BlockingStartupService(
    ILogger<BlockingStartupService> logger, 
    IServiceScopeFactory scopeFactory,
    HttpClient httpClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("BLOCKING STARTUP SERVICE starting...");
        
        try
        {
            // Use scope factory to resolve scoped IProductService from singleton context
            using var scope = scopeFactory.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            
            logger.LogWarning("STARTING REALISTIC BLOCKING STARTUP CHECKS...");
            logger.LogWarning("Blocking startup demonstration begins at: {StartTime}", DateTime.Now.ToString("HH:mm:ss.fff"));
            
            // STEP 1: BLOCKING STARTUP HEALTH CHECKS (60 seconds total)
            var startupResult = PerformBlockingStartupHealthChecks(stoppingToken);
            
            logger.LogWarning("Realistic blocking startup completed");
            logger.LogInformation("Startup validation: {ChecksCompleted} checks passed, {ExternalCallsMade} external calls made", 
                startupResult.ChecksCompleted, startupResult.ExternalCallsMade);
            logger.LogInformation("All startup checks passed - system ready for main product sync");
            
            // STEP 2: NOW call the actual async product sync
            logger.LogWarning("Starting main products sync...");
            var syncResult = await productService.SyncProductsFromApiAsync(stoppingToken);
            
            logger.LogWarning("ALL BLOCKING OPERATIONS COMPLETE - API NOW AVAILABLE");
            logger.LogInformation("Final completion at: {EndTime}", DateTime.Now.ToString("HH:mm:ss.fff"));
            logger.LogInformation("Total products synced: {TotalProducts}", syncResult.TotalProductsSynced);
            
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in blocking startup service - this would prevent app from starting!");
        }
    }
    
    // REALISTIC BLOCKING STARTUP HEALTH CHECKS
    private StartupHealthResult PerformBlockingStartupHealthChecks(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting realistic blocking startup health checks...");
        
        var result = new StartupHealthResult();
        
        // Health Check 1: Database connectivity (blocking)
        logger.LogInformation("Check 1: Testing database connectivity...");
        CheckDatabaseConnectivity(stoppingToken);
        Thread.Sleep(3000); // Simulate 3 seconds database check
        result.ChecksCompleted++;
        
        // Health Check 2: Configuration validation (blocking)
        logger.LogInformation("Check 2: Validating application configuration...");
        ValidateConfiguration(stoppingToken);
        Thread.Sleep(2000); // Simulate 2 seconds config validation
        result.ChecksCompleted++;
        
        // Health Check 3: External service ping (single real HTTP call)
        logger.LogInformation("Check 3: Pinging external product service...");
        var isExternalServiceAvailable = PingExternalService(stoppingToken);
        result.ExternalCallsMade++;
        result.ChecksCompleted++;
        
        if (!isExternalServiceAvailable)
        {
            logger.LogWarning("External service not available, but continuing startup...");
        }
        
        // Health Check 4: Cache warming (blocking)
        logger.LogInformation("Check 4: Warming up application cache...");
        WarmupApplicationCache(stoppingToken);
        Thread.Sleep(5000); // Simulate 5 seconds cache warming
        result.ChecksCompleted++;
        
        // Health Check 5: Security validation (blocking)
        logger.LogInformation("Check 5: Performing security validations...");
        ValidateSecuritySettings(stoppingToken);
        Thread.Sleep(2000); // Simulate 2 seconds security check
        result.ChecksCompleted++;
        
        // Health Check 6: Final system readiness (blocking)
        logger.LogInformation("Check 6: Final system readiness check...");
        PerformFinalSystemCheck(stoppingToken);
        Thread.Sleep(4000); // Simulate 4 seconds final check
        result.ChecksCompleted++;
        
        // Add some additional blocking time to reach ~60 seconds total
        logger.LogInformation("Additional startup processing...");
        Thread.Sleep(30000); // Additional 30 seconds of blocking startup work
        result.ChecksCompleted++;
        
        logger.LogWarning("All {ChecksCompleted} startup health checks completed", result.ChecksCompleted);
        return result;
    }
    
    // Real external service ping (one simple HTTP call)
    private bool PingExternalService(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Pinging external product service...");
            
            // Simple health check call
            var response = httpClient.GetAsync("https://api.escuelajs.co/api/v1/products?limit=1", stoppingToken).Result;
            var isAvailable = response.IsSuccessStatusCode;
            
            logger.LogInformation(isAvailable 
                ? "External service is available and responding" 
                : "External service returned status: {StatusCode}", response.StatusCode);
            
            return isAvailable;
        }
        catch (Exception ex)
        {
            logger.LogWarning("External service ping failed: {ErrorMessage}", ex.Message);
            return false;
        }
    }
    
    // Database connectivity check (simulated blocking)
    private void CheckDatabaseConnectivity(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            throw new OperationCanceledException("Database connectivity check cancelled");
            
        logger.LogInformation("Testing database connection pool...");
        logger.LogInformation("Validating schema and migrations...");
        logger.LogInformation("Checking database performance metrics...");
        
        // Simulate database operations
        Thread.Sleep(500);
        if (stoppingToken.IsCancellationRequested)
            throw new OperationCanceledException("Database connectivity check cancelled");
            
        logger.LogInformation("Database connectivity validation passed");
    }
    
    // Configuration validation (simulated blocking)
    private void ValidateConfiguration(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            throw new OperationCanceledException("Configuration validation cancelled");
            
        logger.LogInformation("Validating connection strings...");
        logger.LogInformation("Checking environment variables...");
        logger.LogInformation("Validating feature flags...");
        
        Thread.Sleep(500);
        if (stoppingToken.IsCancellationRequested)
            throw new OperationCanceledException("Configuration validation cancelled");
            
        logger.LogInformation("Configuration validation passed");
    }
    
    // Cache warming (simulated blocking)
    private void WarmupApplicationCache(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            throw new OperationCanceledException("Cache warming cancelled");
            
        logger.LogInformation("Initializing distributed cache...");
        logger.LogInformation("Pre-loading frequently accessed data...");
        logger.LogInformation("Building search indexes...");
        
        Thread.Sleep(1000);
        if (stoppingToken.IsCancellationRequested)
            throw new OperationCanceledException("Cache warming cancelled");
            
        logger.LogInformation("Application cache warming completed");
    }
    
    // Security validation (simulated blocking)
    private void ValidateSecuritySettings(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            throw new OperationCanceledException("Security validation cancelled");
            
        logger.LogInformation("Validating SSL certificates...");
        logger.LogInformation("Checking authentication settings...");
        logger.LogInformation("Validating security policies...");
        
        Thread.Sleep(500);
        if (stoppingToken.IsCancellationRequested)
            throw new OperationCanceledException("Security validation cancelled");
            
        logger.LogInformation("Security validation passed");
    }
    
    // Final system check (simulated blocking)
    private void PerformFinalSystemCheck(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            throw new OperationCanceledException("Final system check cancelled");
            
        logger.LogInformation("Running system diagnostics...");
        logger.LogInformation("Checking memory and CPU usage...");
        logger.LogInformation("Validating service dependencies...");
        
        Thread.Sleep(1000);
        if (stoppingToken.IsCancellationRequested)
            throw new OperationCanceledException("Final system check cancelled");
            
        logger.LogInformation("Final system readiness check passed");
    }
    
    // Result class to track startup metrics
    private class StartupHealthResult
    {
        public int ChecksCompleted { get; set; }
        public int ExternalCallsMade { get; set; }
    }
}
