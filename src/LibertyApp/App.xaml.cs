using LibertyApp.Properties;
using LibertyApp.ViewModels;
using LibertyApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Windows;

namespace LibertyApp;

public partial class App : Application
{
    public IServiceProvider Services { get; }

    public new static App Current => (App)Application.Current;

    public App()
    {
        var services = new ServiceCollection()
            .AddSingleton(typeof(MainWindowViewModel))
            .AddSingleton(typeof(ConnectionViewModel))
            .AddSingleton(typeof(AboutViewModel))
            .AddSingleton(typeof(DonateViewModel));

        Services = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        WpfSingleInstance.Make(Resource.ConnectionName, false);

        ConnectionSetup.ImportPfx();

        var splashScreen = ShowSplashScreen(Thread.CurrentThread.CurrentUICulture.Name);
        splashScreen.Show(true);

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (Current.MainWindow != null && ((MainWindowViewModel)Current.MainWindow.DataContext)
            .ConnectionViewModel
            .IsConnected)
        {
            ConnectionSetup.Disconnect();
            Current.Shutdown();
        }
    }

    /// <summary>
    /// Splash screen localization
    /// </summary>
    private static SplashScreen ShowSplashScreen(string local) => local switch
    {
        "ru-RU" => new SplashScreen("Resources/splashscreen.ru-RU.jpg"),
        _ => new SplashScreen("Resources/splashscreen.jpg"),
    };
}