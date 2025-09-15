using System.Windows;
using AutodartsTrainer.Services;
using AutodartsTrainer.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AutodartsTrainer;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();

        var localization = Services.GetRequiredService<LocalizationService>();
        Resources["Localization"] = localization;

        var themeService = Services.GetRequiredService<ThemeService>();
        themeService.ApplyTheme(themeService.CurrentTheme);

        var mainViewModel = Services.GetRequiredService<MainViewModel>();
        mainViewModel.InitializeAsync().GetAwaiter().GetResult();

        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.DataContext = mainViewModel;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (Services.GetService<MainViewModel>() is { } vm)
        {
            vm.ShutdownAsync().GetAwaiter().GetResult();
        }

        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<LocalizationService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<AudioService>();
        services.AddSingleton<CameraService>();
        services.AddSingleton<PersistenceService>();
        services.AddSingleton<ExportService>();
        services.AddSingleton<GameCoordinator>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
    }
}
