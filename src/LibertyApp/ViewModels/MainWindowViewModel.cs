using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace LibertyApp.ViewModels;

public class MainWindowViewModel : ObservableObject
{
	public ConnectionViewModel ConnectionViewModel { get; }
	public AboutViewModel AboutViewModel { get; }
	public DonateViewModel DonateViewModel { get; }

	public IRelayCommand ShowConnectionViewCommand { get; }
	public IRelayCommand ShowAboutViewCommand { get; }
	public IRelayCommand ShowDonateViewCommand { get; }

	public object CurrentView
	{
		get => _currentView;
		private set => SetProperty(ref _currentView, value);
	}
	private object _currentView;

	public MainWindowViewModel()
	{
		ConnectionViewModel = App.Current.Services.GetService<ConnectionViewModel>();
		AboutViewModel = App.Current.Services.GetService<AboutViewModel>();
		DonateViewModel = App.Current.Services.GetService<DonateViewModel>();

		CurrentView = ConnectionViewModel;

		ShowConnectionViewCommand = new RelayCommand(() => CurrentView = ConnectionViewModel);
		ShowAboutViewCommand = new RelayCommand(() => CurrentView = AboutViewModel);
		ShowDonateViewCommand = new RelayCommand(() => CurrentView = DonateViewModel);
	}
}