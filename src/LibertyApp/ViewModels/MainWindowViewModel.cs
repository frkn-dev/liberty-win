using LibertyApp.Language;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows;

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

	public ConnectionViewModel ConnectionViewModel { get; }
	public AboutViewModel AboutViewModel { get; }
	public DonateViewModel DonateViewModel { get; }

	#endregion

	#region Commands

	public IRelayCommand ShowConnectionViewCommand { get; }
	public IRelayCommand ShowAboutViewCommand { get; }
	public IRelayCommand ShowDonateViewCommand { get; }
	public IRelayCommand ShutdownWindowCommand { get; }

	#endregion

	#region Constructors

	public MainWindowViewModel()
	{
		ConnectionViewModel = App.Current.Services.GetService<ConnectionViewModel>();
		AboutViewModel = App.Current.Services.GetService<AboutViewModel>();
		DonateViewModel = App.Current.Services.GetService<DonateViewModel>();

		CurrentView = ConnectionViewModel;

		App.Current.MainWindow.MaxHeight = SystemParameters.MaximumWindowTrackHeight;

		ShutdownWindowCommand = new RelayCommand(ShutdownWindow);

		ShowConnectionViewCommand = new RelayCommand(ShowConnectionView);
		ShowAboutViewCommand = new RelayCommand(ShowAboutView);
		ShowDonateViewCommand = new RelayCommand(ShowDonateView);
	}

	#endregion

	#region Private methods

	private void ShutdownWindow()
	{
		if (ConnectionViewModel.IsConnected)
		{
			if (MessageBox.Show(Strings.ExitApplicationMessage,
					Strings.ExitApplicationCaption,
					MessageBoxButton.YesNo,
					MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				ConnectionSetup.Disconnect();

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