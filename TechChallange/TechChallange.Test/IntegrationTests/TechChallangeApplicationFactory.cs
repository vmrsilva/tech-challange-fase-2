using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;
using Testcontainers.Redis;
using Microsoft.EntityFrameworkCore;
using TechChallange.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.SqlClient;
using TechChallange.Domain.Region.Entity;
using TechChallange.Infrastructure.Cache;
using TechChallange.Domain.Cache;
using Microsoft.Extensions.Caching.Distributed;

namespace TechChallange.Test.IntegrationTests
{
    public class TechChallangeApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder().Build();
        private readonly RedisContainer _redisContainer = new RedisBuilder().Build();

        private string? _connectionString;
        private string? _connectionStringRedis;

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptorSql = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<TechChallangeContext>));

                if (descriptorSql != null)
                {
                    services.Remove(descriptorSql);
                }

                services.AddDbContext<TechChallangeContext>(options =>
                    options.UseSqlServer(_connectionString!)
                );


                var descriptorRedis = services.SingleOrDefault(d =>
          d.ServiceType == typeof(IDistributedCache));

                if (descriptorRedis != null)
                {
                    services.Remove(descriptorRedis);
                }

                // Adicionando a nova configuração do Redis
                services.AddStackExchangeRedisCache(options =>
                {
                    options.InstanceName = nameof(CacheRepository);
                    options.Configuration = _connectionStringRedis; // Certifique-se que `_connectionString` contém a conexão do Redis
                });

                services.AddScoped<ICacheRepository, CacheRepository>();
                services.AddScoped<ICacheWrapper, CacheWrapper>();

            });

            return base.CreateHost(builder);
        }

        public async Task InitializeAsync()
        {
            await _sqlContainer.StartAsync();
            _connectionString = _sqlContainer.GetConnectionString();
            Environment.SetEnvironmentVariable("ConnectionStrings.Database", _connectionString);
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await _redisContainer.StartAsync();
            _connectionStringRedis = _redisContainer.GetConnectionString();
            Environment.SetEnvironmentVariable("ConnectionStrings.Cache", _connectionStringRedis);


            await WaitForDatabaseAsync();

            using (var scope = Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TechChallangeContext>();
                await context.Database.EnsureCreatedAsync();

                var region = new RegionEntity("SP", "11");

                context.Region.Add(region);
                await context.SaveChangesAsync();
            }
        }

        public async Task DisposeAsync()
        {
            await _sqlContainer.StopAsync();
            await _redisContainer.StopAsync();
        }

        private async Task WaitForDatabaseAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    await connection.OpenAsync();
                    return;
                }
                catch
                {
                    await Task.Delay(5000);
                }
            }
            throw new Exception("Banco de dados não respondeu dentro do tempo esperado.");
        }
    }

}
