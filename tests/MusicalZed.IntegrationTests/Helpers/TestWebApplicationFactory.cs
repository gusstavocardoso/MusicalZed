using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicalZed.Infrastructure.Data;

namespace MusicalZed.IntegrationTests.Helpers;

/// <summary>
/// Factory de testes que usa SQLite in-memory com conexão persistente.
/// A conexão fica aberta durante toda a vida da factory, garantindo que
/// o banco in-memory não seja descartado entre requisições.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    // Conexão mantida aberta para que o banco in-memory persista
    private readonly SqliteConnection _keepAliveConnection;

    public TestWebApplicationFactory()
    {
        _keepAliveConnection = new SqliteConnection("DataSource=:memory:");
        _keepAliveConnection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext de produção
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<MusicalZedDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Registra o DbContext usando a conexão in-memory persistente
            services.AddDbContext<MusicalZedDbContext>(options =>
                options.UseSqlite(_keepAliveConnection));
        });

        builder.UseEnvironment("Testing");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _keepAliveConnection.Dispose();

        base.Dispose(disposing);
    }
}
