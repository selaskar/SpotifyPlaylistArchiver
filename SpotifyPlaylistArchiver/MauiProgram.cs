using System.Reflection;
using Duende.IdentityModel.OidcClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpotifyPlaylistArchiver.Authentication;
using SpotifyPlaylistArchiver.Repository.Extensions;
using SpotifyPlaylistArchiver.ViewModels;

namespace SpotifyPlaylistArchiver;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        IConfiguration appConfiguration = GetConfig();
        
        builder.Configuration.AddConfiguration(appConfiguration);


        // setup OidcClient
        builder.Services.AddSingleton(new OidcClient(new()
        {
            //TODO: configuration
            Authority = "https://accounts.spotify.com/",

            ClientId = "b710698b21444d6e8c77a863920bfacb",
            LoadProfile = false,
            Scope = "user-library-read",
            RedirectUri = "spotifyarchiver://callback",

            Browser = new MauiAuthenticationBrowser()
        }));

        builder.Services.AddTransient<MainViewModel>();

        builder.Services.ConfigureCoreRepository(appConfiguration);

        return builder.Build();
    }


    private static IConfiguration GetConfig()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string embeddedConfigFileName = $"{assembly.GetName().Name}.appsettings.json";
        using Stream? stream = assembly.GetManifestResourceStream(embeddedConfigFileName);
        return new ConfigurationBuilder()
            .AddJsonStream(stream!)
            .Build();
    }
}
