using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechChallange.Domain.Base.Repository;
using TechChallange.Domain.Cache;
using TechChallange.Domain.Contact.Repository;
using TechChallange.Domain.Contact.Service;
using TechChallange.Domain.Region.Repository;
using TechChallange.Domain.Region.Service;
using TechChallange.Infrastructure.Cache;
using TechChallange.Infrastructure.Context;
using TechChallange.Infrastructure.Repository.Base;
using TechChallange.Infrastructure.Repository.Contact;
using TechChallange.Infrastructure.Repository.Region;

namespace TechChallange.IoC
{
    public static class DomainInjection
    {
        public static void AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureContext(services, configuration);
            ConfigureBase(services);
            ConfigureRegion(services);
            ConfigureContact(services);
            ConfigureCache(services, configuration);
        }

        public static void ConfigureContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TechChallangeContext>(options => options.UseSqlServer(configuration.GetConnectionString("Database")));

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var dbContext = serviceProvider.GetRequiredService<TechChallangeContext>();
                dbContext.Database.Migrate();
            }
        }

        public static void ConfigureBase(IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        }
        public static void ConfigureRegion(IServiceCollection services)
        {
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<IRegionService, RegionService>();
        }

        public static void ConfigureContact(IServiceCollection services)
        {
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IContactService, ContactService>();
        }
        public static void ConfigureCache(IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options => {
                options.InstanceName = nameof(CacheRepository);
                options.Configuration = configuration.GetConnectionString("Cache");
            });
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<ICacheWrapper, CacheWrapper>();
        }
    }
}
