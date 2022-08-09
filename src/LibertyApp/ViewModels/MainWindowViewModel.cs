using LibertyApp.Language;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibertyApp.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    #region Properties

    private object _currentView;
    public object CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public GlobalViewModel Instance { get; }
    public ConnectionViewModel ConnectionViewModel { get; }
    public AboutViewModel AboutViewModel { get; }
    public DonateViewModel DonateViewModel { get; }

    #endregion

    #region Commands

    public ICommand ShowConnectionViewCommand { get; }
    public ICommand ShowAboutViewCommand { get; }
    public ICommand ShowDonateViewCommand { get; }

    public ICommand MoveWindowCommand { get; }

    public ICommand MaximizeWindowCommand { get; }

    public ICommand MinimizeWindowCommand { get; }

    public ICommand ShutdownWindowAsyncCommand { get; }

    #endregion

    #region Constructors

    public MainWindowViewModel()
    {
        Instance = GlobalViewModel.Instance;
        ConnectionViewModel = App.Current.Services.GetService<ConnectionViewModel>();
        AboutViewModel = App.Current.Services.GetService<AboutViewModel>();
        DonateViewModel = App.Current.Services.GetService<DonateViewModel>();

        CurrentView = ConnectionViewModel;

        App.Current.MainWindow.MaxHeight = SystemParameters.MaximumWindowTrackHeight;

        MoveWindowCommand = new RelayCommand(MoveWindow);
        MaximizeWindowCommand = new RelayCommand(MaximizeWindow);
        MinimizeWindowCommand = new RelayCommand(MinimizeWindow);
        ShutdownWindowAsyncCommand = new AsyncRelayCommand(ShutdownWindowAsync);

        ShowConnectionViewCommand = new RelayCommand(ShowConnectionView);
        ShowAboutViewCommand = new RelayCommand(ShowAboutView);
        ShowDonateViewCommand = new RelayCommand(ShowDonateView);
    }

    #endregion

    #region Private methods

    private static void MoveWindow() => Application.Current.MainWindow.DragMove();

    private static void MaximizeWindow()
    {
        App.Current.MainWindow.WindowState = App.Current.MainWindow.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private static void MinimizeWindow() => App.Current.MainWindow.WindowState = WindowState.Minimized;

    private async Task ShutdownWindowAsync()
    {
        if (ConnectionViewModel.IsConnected)
        {
            if (MessageBox.Show(Strings.ExitApplicationMessage,
                    Strings.ExitApplicationCaption,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await ConnectionViewModel.ConnectCommandAsync.ExecuteAsync(null);

                App.Current.Shutdown();
            }
        }
        else
        {
            App.Current.Shutdown();
        }
    }

    private void ShowConnectionView() => CurrentView = ConnectionViewModel;

    private void ShowAboutView() => CurrentView = AboutViewModel;

    private void ShowDonateView() => CurrentView = DonateViewModel;

    #endregion
}