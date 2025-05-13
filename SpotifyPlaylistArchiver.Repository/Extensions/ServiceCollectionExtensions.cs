using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpotifyPlaylistArchiver.Repository.Abstract;
using SpotifyPlaylistArchiver.Repository.DbContexts;
using SpotifyPlaylistArchiver.Repository.Mappers;
using SpotifyPlaylistArchiver.Repository.Options;

namespace SpotifyPlaylistArchiver.Repository.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureCoreRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CoreCatalogOptions>()
            .Bind(configuration.GetRequiredSection(CoreCatalogOptions.Section), binderOptions => binderOptions.ErrorOnUnknownConfiguration = true);

        services.AddDbContext<CoreCatalogDbContext>((serviceProvider, builder) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<CoreCatalogOptions>>().Value;

            builder.UseSqlServer(options.ConnectionString);
        });

        services.AddAutoMapper(typeof(EntityModelMappings));

        services.AddScoped<ICoreCatalogRepository, CoreCatalogRepository>();

        return services;
    }
}
