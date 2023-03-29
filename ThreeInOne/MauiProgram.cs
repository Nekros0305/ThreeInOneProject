using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System.Net.Http.Headers;
using System.Reflection;
using ThreeInOne.Configuration.SunInfo;
using ThreeInOne.Interfaces.SunInfo;
using ThreeInOne.Policies.Suninfo;
using ThreeInOne.Services.SunInfo;
using ThreeInOne.ViewModels;
using ThreeInOne.ViewModels.Sudoku;
using ThreeInOne.ViewModels.SunInfo;
using ThreeInOne.ViewModels.TicTacToe;
using ThreeInOne.Views;
using ThreeInOne.WPFStuff.Converters;

namespace ThreeInOne;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).Configuration.SetBasePath(AppContext.BaseDirectory);

        var a = Assembly.GetExecutingAssembly();
        var stream = a.GetManifestResourceStream("ThreeInOne.appsettings.json")
            ?? throw new FileNotFoundException("Required Settings Manifest was not found");

        var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();
        builder.Configuration.AddConfiguration(config);

        builder.Services
            .Config(builder.Configuration)
            .RegisterServices(builder.Configuration)
            .RegisterPages()
            .AddMemoryCache();

        builder.Logging.AddSerilog();

        return builder.Build();
    }

    private static IServiceCollection Config(this IServiceCollection sc, IConfiguration config)
    {
        sc.Configure<IpServiceConfig>(config.GetRequiredSection(nameof(IpServiceConfig)));
        sc.Configure<ContainerConfig>(config.GetRequiredSection(nameof(ContainerConfig)));

        var logPath = Path.Combine(FileSystem.AppDataDirectory, "Logs", "log.txt");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error) //was set to Error to avoid Warnings in Logfile about bad implementations in Sudoku
            .Enrich.FromLogContext()
            .WriteTo.File(logPath, flushToDiskInterval: new TimeSpan(0, 0, 30), encoding: System.Text.Encoding.UTF8, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 3)
            .CreateLogger();

        return sc;
    }

    private static IServiceCollection RegisterServices(this IServiceCollection sc, IConfiguration configuration)
    {
        sc.AddHttpClient(nameof(SunInfoServiceCache), c =>
        {
            c.BaseAddress = new Uri("https://api.sunrise-sunset.org/");
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).SingleTimeoutWithRetryPolicy();
        var toRegister = configuration.GetSection(nameof(ContainerConfig))
            .GetValue<string>(nameof(ContainerConfig.RegisterLocationFrom));

        switch (toRegister)
        {
            case "Api":
                var url = configuration.GetSection(nameof(ContainerConfig))
                    .GetValue<string>(nameof(ContainerConfig.RegisterApiUrl));
                sc.AddHttpClient(nameof(LocationServiceFromApi), c =>
                {
                    c.BaseAddress = new Uri(url!);
                    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });
                sc.AddTransient<ILocationService, LocationServiceFromApi>();
                break;
#if WINDOWS
            case "Ip":
                sc.AddHttpClient<IIpService, IpServiceReal>();
                sc.AddTransient<ILocationService, LocationServiceFromIp>();
                break;
#endif
            default:
                sc.AddTransient<ILocationService, LocationServiceFake>();
                break;
        }
        sc.AddTransient<ISunInfoService, SunInfoServiceCache>();
        sc.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        return sc;
    }

    private static IServiceCollection RegisterPages(this IServiceCollection sc)
    {
        sc.AddSingleton<MainPage>();
        sc.AddSingleton<MainPageViewModel>();

        sc.AddTransient<SunInfoPage>();
        sc.AddTransient<SunInfoPageViewModel>();

        sc.AddTransient<TicTacToePage>();
        sc.AddTransient<TicTacToePageViewModel>();

        sc.AddTransient<SudokuPage>();
        sc.AddTransient<SudokuPageViewModel>();
        sc.AddSingleton<IValueConverter, TwoDimentionalArrayReader>();

        return sc;
    }
}