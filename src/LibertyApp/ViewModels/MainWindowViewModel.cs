using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibertyApp.Language;
using LibertyApp.Properties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace LibertyApp.ViewModels;

public class MainWindowViewModel : ObservableObject
{
	public ConnectionViewModel ConnectionViewModel { get; }
	public AboutViewModel AboutViewModel { get; }
	public DonateViewModel DonateViewModel { get; }

	public object CurrentView
	{
		get => _currentView;
		private set => SetProperty(ref _currentView, value);
	}
	private object _currentView;

	public bool AutoRun
	{
		get => _autoRun;
		set => SetProperty(ref _autoRun, value);
	}
	private bool _autoRun;

	public bool HideInTray
	{
		get => _hideInTray;
		set => SetProperty(ref _hideInTray, value);
	}
	private bool _hideInTray;

	public bool AutoConnect
	{
		get => _autoConnect;
		set => SetProperty(ref _autoConnect, value);
	}
	private bool _autoConnect;

	public IRelayCommand ShowConnectionViewCommand { get; }
	public IRelayCommand ShowAboutViewCommand { get; }
	public IRelayCommand ShowDonateViewCommand { get; }

	public IRelayCommand HideCommand { get; }
	public IRelayCommand ShutdownCommand { get; }

	public MainWindowViewModel()
	{
		AutoRun = Properties.Settings.Default.AutoRun;
		HideInTray = Properties.Settings.Default.HideInTray;
		AutoConnect = Properties.Settings.Default.AutoConnect;

		ConnectionViewModel = App.Current.Services.GetService<ConnectionViewModel>();
		AboutViewModel = App.Current.Services.GetService<AboutViewModel>();
		DonateViewModel = App.Current.Services.GetService<DonateViewModel>();

		CurrentView = ConnectionViewModel;

		ShowConnectionViewCommand = new RelayCommand(() => CurrentView = ConnectionViewModel);
		ShowAboutViewCommand = new RelayCommand(() => CurrentView = AboutViewModel);
		ShowDonateViewCommand = new RelayCommand(() => CurrentView = DonateViewModel);

		HideCommand = new RelayCommand(() => App.Current.MainWindow.Close());
		ShutdownCommand = new RelayCommand(() => App.Current.OnExitApplication(null, null));

		SettingsAutoRunCommand = new RelayCommand(AutoRunSettings);
		SettingsHideInTrayCommand = new RelayCommand(HideInTraySettings);
		SettingsAutoConnectCommand = new RelayCommand(AutoConnectSettings);

		HelpCommand = new RelayCommand(Help);
		AboutCommand = new RelayCommand(About);
	}

	public IRelayCommand SettingsAutoRunCommand { get; }
	public void AutoRunSettings()
	{
		if (Properties.Settings.Default.AutoRun != AutoRun)
		{
			if (!Properties.Settings.Default.AutoRun)
			{
				if (System.Windows.MessageBox.Show(
					Strings.LaunchWithWindowsExplicit,
					Strings.LaunchWithWindows,
					System.Windows.MessageBoxButton.YesNo,
					System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
				{
					try
					{
						using var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

						key.DeleteValue(Strings.AppName, false);

						key.SetValue(Strings.AppName, Application.ExecutablePath);

						Properties.Settings.Default.AutoRun = AutoRun;
						Properties.Settings.Default.Save();

						App.Current.NotifyIcon.ShowBalloonTip(3000,
							Strings.AppName,
							Strings.LaunchWithWindowsMessage,
							System.Windows.Forms.ToolTipIcon.Info);
					}
					catch (Exception ex)
					{
						App.Current.NotifyIcon.ShowBalloonTip(3000,
							Strings.AppName,
							ex.Message,
							System.Windows.Forms.ToolTipIcon.Error);
					}
				}
				else
				{
					AutoRun = false;
				}
			}
			else
			{
				try
				{
					using var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

					key.DeleteValue(Strings.AppName, false);

					Properties.Settings.Default.AutoRun = AutoRun;
					Properties.Settings.Default.Save();
				}
				catch (Exception ex)
				{
					App.Current.NotifyIcon.ShowBalloonTip(3000,
						Strings.AppName,
						ex.Message,
						System.Windows.Forms.ToolTipIcon.Error);
				}
			}
		}
	}

	public IRelayCommand SettingsHideInTrayCommand { get; }
	public void HideInTraySettings()
	{
		if (Properties.Settings.Default.HideInTray != HideInTray)
		{
			Properties.Settings.Default.HideInTray = HideInTray;
			Properties.Settings.Default.Save();

			if (HideInTray)
			{
				App.Current.NotifyIcon.ShowBalloonTip(3000,
					Strings.AppName,
					Strings.LaunchHiddenMessage,
					System.Windows.Forms.ToolTipIcon.Info);
			}
		}
	}

	public IRelayCommand SettingsAutoConnectCommand { get; }
	public void AutoConnectSettings()
	{
		if (Properties.Settings.Default.AutoConnect != AutoConnect)
		{
			if (!Properties.Settings.Default.AutoConnect)
			{
				if (System.Windows.MessageBox.Show(
					Strings.LaunchConnectExplicit,
					Strings.ConnectAtLaunch,
					System.Windows.MessageBoxButton.YesNo,
					System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
				{
					Properties.Settings.Default.AutoConnect = AutoConnect;
					Properties.Settings.Default.Save();

					App.Current.NotifyIcon.ShowBalloonTip(3000,
						Strings.AppName,
						Strings.LaunchConnectMessage,
						System.Windows.Forms.ToolTipIcon.Info);
				}
				else
				{
					AutoConnect = false;
				}
			}
			else
			{
				Properties.Settings.Default.AutoConnect = AutoConnect;
				Properties.Settings.Default.Save();
			}
		}
	}

	public IRelayCommand HelpCommand { get; }
	public void Help()
	{
		var processStartInfo = new ProcessStartInfo
		{
			UseShellExecute = true,
			FileName = $"{Resources.Github}/blob/main/README.md",
		};

		Process.Start(processStartInfo);
	}

	public IRelayCommand AboutCommand { get; }
	public void About() => System.Windows.MessageBox.Show(
		Assembly.GetExecutingAssembly().GetName().Version.ToString(),
		Strings.Version,
		System.Windows.MessageBoxButton.OK,
		System.Windows.MessageBoxImage.Information);
}