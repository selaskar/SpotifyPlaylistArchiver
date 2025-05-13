using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SpotifyPlaylistArchiver.Repository.DbContexts;
using SpotifyPlaylistArchiver.Repository.Options;

namespace SpotifyPlaylistArchiver.DbUpdater;

internal class Program
{
    internal static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddOptions<CoreCatalogOptions>().BindConfiguration(CoreCatalogOptions.Section);

        builder.Services.AddDbContext<CoreCatalogDbContext>((serviceProvider, builder) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<CoreCatalogOptions>>().Value;

            builder.UseSqlServer(options.ConnectionString);
        });

        using IHost host = builder.Build();

        await host.RunAsync();
    }
}
