using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MsSql;
using Microsoft.EntityFrameworkCore;
using TechChallange.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using DotNet.Testcontainers.Builders;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;

namespace TechChallange.Test.IntegrationTests
{
    public class TechChallangeApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer;
        public TechChallangeApplicationFactory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _msSqlContainer = new MsSqlBuilder()
                    .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
                      .WithPassword("password(!)Strong")
                             .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
                             .Build();
            }
            else
            {
                _msSqlContainer = new MsSqlBuilder().Build();
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                ConfigureDbContext(services);
            });

            //builder.UseEnvironment("Development");
            base.ConfigureWebHost(builder);
        }

        private void ConfigureDbContext(IServiceCollection services)
        {
            var context = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(TechChallangeContext));
            if (context != null)
            {
                services.Remove(context);
                var options = services.Where(r => (r.ServiceType == typeof(DbContextOptions))
                  || (r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))).ToArray();
                foreach (var option in options)
                {
                    services.Remove(option);
                }
            }

            services.AddDbContext<TechChallangeContext>(options =>
            {
                options.UseSqlServer(_msSqlContainer.GetConnectionString());
            });

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var dbContext = serviceProvider.GetRequiredService<TechChallangeContext>();
                dbContext.Database.Migrate();
            }
        }

        //protected override IHost CreateHost(IHostBuilder builder)
        //{
        //    builder.ConfigureServices(services =>
        //    {

        //        var descriptorSqlaa = services.SingleOrDefault(d =>
        //           d.ServiceType == typeof(TechChallangeContext));

        //        services.Remove(descriptorSqlaa);


        //        var descriptorSql = services.SingleOrDefault(d =>
        //            d.ServiceType == typeof(DbContextOptions<TechChallangeContext>));

        //        if (descriptorSql != null)
        //        {
        //            services.Remove(descriptorSql);
        //        }

        //        services.AddDbContext<TechChallangeContext>(options =>
        //            options.UseSqlServer(_connectionString!)
        //        );


        //        //var descriptorRedis = services.SingleOrDefault(options =>
        //        //    options.ServiceType == typeof(IDistributedCache));

        //        //if (descriptorRedis != null)
        //        //{
        //        //    services.Remove(descriptorRedis);
        //        //}

        //        //services.AddStackExchangeRedisCache(options =>
        //        //{
        //        //    options.InstanceName = nameof(CacheRepository);
        //        //    options.Configuration = _connectionStringRedis;
        //        //});

        //        //services.AddScoped<ICacheRepository, CacheRepository>();
        //        //services.AddScoped<ICacheWrapper, CacheWrapper>();

        //    });

        //    using var connection = new SqlConnection(_connectionString);


        //    var host = base.CreateHost(builder);

        //    return host;
        //}

        public async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();
            var x = _msSqlContainer.GetConnectionString();

            var aa = x;
        }

        public async new Task DisposeAsync()
        {
            await _msSqlContainer.StopAsync();
        }
    }

}
