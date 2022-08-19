using LibertyApp.Language;
using LibertyApp.Properties;
using LibertyApp.ViewModels;
using LibertyApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace LibertyApp;

public partial class App : Application
{
	public readonly NotifyIcon NotifyIcon;

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

		NotifyIcon = new NotifyIcon
		{
			Icon = new Icon("./Resources/icon.ico"),
			Text = Strings.AppName,
		};
	}

	protected override void OnStartup(StartupEventArgs e)
	{
		WpfSingleInstance.Make(Resource.ConnectionName, false);

		ConnectionSetup.ImportPfx();

		MainWindow = new MainWindow();
		MainWindow.Closing += (sender, args) =>
		{
			args.Cancel = true;
			ShowHideWindow(sender, args);
		};
		MainWindow.Show();

		NotifyIcon.DoubleClick += ShowHideWindow;

		NotifyIcon.ContextMenuStrip = new ContextMenuStrip();
		NotifyIcon.ContextMenuStrip.Items.Add(Strings.ShowHide, Image.FromFile("./Resources/icon.ico"), ShowHideWindow);
		NotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
		NotifyIcon.ContextMenuStrip.Items.Add(Strings.Exit, null, (sender, args) =>
		{
			if ((MainWindow.DataContext as MainWindowViewModel).ConnectionViewModel.IsConnected)
			{
				if (System.Windows.MessageBox.Show(Strings.ExitApplicationMessage,
						Strings.ExitApplicationCaption,
						MessageBoxButton.YesNo,
						MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					ConnectionSetup.Disconnect();
					App.Current.NotifyIcon.ShowBalloonTip(100, Strings.AppName, Strings.StatusDisconnected, ToolTipIcon.Info);
					CloseApp();
				}
			}
			else
			{
				CloseApp();
			}
		});

		NotifyIcon.Visible = true;

		base.OnStartup(e);
	}

	/// <summary>
	/// Show/hide main window
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private void ShowHideWindow(object sender, EventArgs e)
	{
		switch (MainWindow?.WindowState)
		{
			case WindowState.Minimized:
				MainWindow.WindowState = WindowState.Normal;
				MainWindow.ShowInTaskbar = true;
				MainWindow.Activate();
				break;
			case WindowState.Normal:
				MainWindow.WindowState = WindowState.Minimized;
				MainWindow.ShowInTaskbar = false;
				App.Current.NotifyIcon.ShowBalloonTip(100, Strings.AppName, Strings.AppInTrayNotify, ToolTipIcon.Info);
				break;
			case WindowState.Maximized:
				MainWindow.WindowState = WindowState.Minimized;
				MainWindow.ShowInTaskbar = false;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(MainWindow.WindowState));
		}
	}

	private void CloseApp()
	{
		NotifyIcon.Visible = false;
		NotifyIcon.Dispose();

		Current.Shutdown();
	}
}