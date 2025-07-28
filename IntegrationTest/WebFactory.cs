using Backend.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IntegrationTest;

public class WebFactory : WebApplicationFactory<Program>
{
    public IServiceScope Scope { get; private set; }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var primaryPixelsDbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<LingoLiftContext>));
            var usersDbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UsersContext>));
            if (usersDbContextDescriptor == null) throw new Exception("Couldn't find registered UserDB");
            if (primaryPixelsDbContextDescriptor == null) throw new Exception("Couldn't find registered SolarDB");
            if (primaryPixelsDbContextDescriptor != null)
                services.Remove(primaryPixelsDbContextDescriptor);
            if (usersDbContextDescriptor != null) 
                services.Remove(usersDbContextDescriptor);

            // Remove registered Db services
            var dbProviderServices = services.Where(service => service.ImplementationType?.FullName?.Contains("SqlServer") == true).ToList();
            foreach (var service in dbProviderServices)
            {
                services.Remove(service);
            }

            //Add Inmemo dbs
            services.AddDbContext<LingoLiftContext>(options =>
            {
                options.UseInMemoryDatabase("LingoDb");
            });

            services.AddDbContext<UsersContext>(options =>
            {
                options.UseInMemoryDatabase("UsersDb");
            });
        });
        
        builder.UseEnvironment("Testing");
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        Scope = host.Services.CreateScope();
        
        // Init Databases
        try
        {
            var primaryPixelsContext = Scope.ServiceProvider.GetRequiredService<LingoLiftContext>();
            primaryPixelsContext.Database.EnsureDeleted();
            primaryPixelsContext.Database.EnsureCreated();

            var userContext = Scope.ServiceProvider.GetRequiredService<UsersContext>();
            userContext.Database.EnsureDeleted();
            userContext.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize test databases", ex);
        }
        return host;
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Scope?.Dispose();
        }
        base.Dispose(disposing);
    }
}