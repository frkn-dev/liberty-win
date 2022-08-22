using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibertyApp.Language;
using LibertyApp.Models;
using LibertyApp.Properties;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace LibertyApp.ViewModels;

public class ConnectionViewModel : ObservableObject
{
	#region Fields

	/// <summary>
	/// Connection timer
	/// </summary>
	private readonly DispatcherTimer _dispatcherTimer;

	/// <summary>
	/// Connection start value
	/// </summary>
	private DateTime _startTime;

	/// <summary>
	/// Connection control background images of states
	/// </summary>
	private static readonly ImageBrush[] BackgroundImageStates =
	{
		new(new BitmapImage(new Uri("pack://application:,,,/Resources/background.jpg"))),
		new(new BitmapImage(new Uri("pack://application:,,,/Resources/background-connecting.jpg"))),
		new(new BitmapImage(new Uri("pack://application:,,,/Resources/background-connected.jpg")))
	};

	#endregion

	#region Public properties

	public ConnectionSpeed ConnectionSpeed { get; }

	public Brush BackgroundImage
	{
		get => _backgroundImage;
		private set => SetProperty(ref _backgroundImage, value);
	}

	private Brush _backgroundImage;

	/// <summary>
	/// Connection button state text and tooltip
	/// </summary>
	public string ConnectButtonText
	{
		get => _connectButtonText;
		private set => SetProperty(ref _connectButtonText, value);
	}

	private string _connectButtonText = Strings.ConnectText;

	/// <summary>
	/// Connection state text
	/// </summary>
	public string ConnectionState
	{
		get => _connectionState;
		private set => SetProperty(ref _connectionState, value);
	}

	private string _connectionState = Strings.StatusDisconnected;

	/// <summary>
	/// Connection timer value
	/// </summary>
	public TimeSpan Timer
	{
		get => _timer;
		private set => SetProperty(ref _timer, value);
	}

	private TimeSpan _timer;

	/// <summary>
	/// Connection state value
	/// </summary>
	public bool IsConnected
	{
		get => _isConnected;
		private set => SetProperty(ref _isConnected, value);
	}

	private bool _isConnected = false;

	#endregion

	#region Constructors

	public ConnectionViewModel()
	{
		BackgroundImage = BackgroundImageStates[0];

		ConnectionSpeed = new ConnectionSpeed();

		_dispatcherTimer = new DispatcherTimer
		{
			Interval = new TimeSpan(0, 0, 1),
		};

		_dispatcherTimer.Tick += (_, _) =>
		{
			Timer = DateTime.Now - _startTime;
			CommandManager.InvalidateRequerySuggested();
			ConnectionSpeed.CalculateDownloadSpeed();
			ConnectionSpeed.CalculateUploadSpeed();
		};

		ConnectCommandAsync = new AsyncRelayCommand(ConnectAsync);
	}

	#endregion

	#region Commands

	/// <summary>
	/// Connection relay command
	/// </summary>
	public IAsyncRelayCommand ConnectCommandAsync { get; }

	/// <summary>
	/// Connection relay command handler
	/// </summary>
	private async Task ConnectAsync()
	{
		try
		{
			// prepare connection process
			using var process = new Process
			{
				StartInfo = new ProcessStartInfo("cmd.exe")
				{
					WorkingDirectory = Environment.CurrentDirectory,
					UseShellExecute = false,
					CreateNoWindow = true,
					ArgumentList =
					{
						"/c",
						"rasdial",
					},
				},
			};

			// if connected already
			if (IsConnected)
			{
				// change interface properties
				ShowConnectingAssets();
				ConnectButtonText = ConnectionState = Strings.DisconnectingText;

				// argument to disconnect
				process.StartInfo.ArgumentList.Add("/d");

				process.Start();
				await process.WaitForExitAsync();

				// handle result

				/* List of error codes for dial-up connections or VPN connections:
					https://docs.microsoft.com/en-us/troubleshoot/windows-client/networking/error-codes-for-dial-up-vpn-connection */

				/* Routing and Remote Access Error Codes:
					https://docs.microsoft.com/en-us/windows/win32/rras/routing-and-remote-access-error-codes */

				switch (process.ExitCode)
				{
					// disconnect success
					case 0:
						// stop timer
						_dispatcherTimer.Stop();
						Timer = TimeSpan.Zero;
						// change interface properties
						IsConnected = false;
						ShowDefaultAssets();
						ConnectButtonText = Strings.ConnectText;
						ConnectionState = Strings.StatusDisconnected;
						App.Current.NotifyIcon.ShowBalloonTip(100, Strings.AppName, Strings.StatusDisconnected, ToolTipIcon.Info);
						break;

					// handle any disconnecting errors
					default:
						App.Current.NotifyIcon.ShowBalloonTip(3000, Strings.ConnectionErrorCaption, process.ExitCode.ToString(), ToolTipIcon.Error);
						// change interface properties
						ShowDefaultAssets();
						ConnectButtonText = Strings.ConnectText;
						ConnectionState = Strings.StatusDisconnected;
						break;
				}
			}
			// if not connected already
			else
			{
				// change interface properties
				ShowConnectingAssets();
				ConnectButtonText = Strings.ConnectingText;
				ConnectionState = Strings.StatusConnecting;

				// arguments to establish a connection
				process.StartInfo.ArgumentList.Add(Resources.ConnectionName);

				process.Start();
				await process.WaitForExitAsync();

				// handle the connection result
				switch (process.ExitCode)
				{
					// success
					case 0:
						// start timer
						_startTime = DateTime.Now;
						_dispatcherTimer.Start();
						// change interface properties
						IsConnected = true;
						ShowConnectedAssets();
						ConnectButtonText = Strings.DisconnectText;
						ConnectionState = Strings.StatusConnected;
						App.Current.NotifyIcon.ShowBalloonTip(100, Strings.AppName, Strings.StatusConnected,
							ToolTipIcon.Info);
						break;

					// any errors
					default:
						App.Current.NotifyIcon.ShowBalloonTip(3000, Strings.ConnectionErrorCaption, process.ExitCode.ToString(), ToolTipIcon.Error);
						// change interface properties
						ShowDefaultAssets();
						ConnectButtonText = Strings.ConnectText;
						ConnectionState = Strings.StatusDisconnected;
						break;
				}
			}
		}
		catch (Exception e)
		{
			App.Current.NotifyIcon.ShowBalloonTip(3000, Strings.ConnectionErrorCaption, e.Message, ToolTipIcon.Error);

			// change interface properties
			IsConnected = false;
			ShowDefaultAssets();
			ConnectButtonText = Strings.ConnectText;
			ConnectionState = Strings.StatusDisconnected;
		}
	}

	#endregion

	#region Private methods

	/// <summary>
	/// Set control default background state
	/// </summary>
	private void ShowDefaultAssets() => BackgroundImage = BackgroundImageStates[0];

	/// <summary>
	/// Set control background state to connecting
	/// </summary>
	private void ShowConnectingAssets() => BackgroundImage = BackgroundImageStates[1];

	/// <summary>
	/// Set control background state to connected
	/// </summary>
	private void ShowConnectedAssets() => BackgroundImage = BackgroundImageStates[2];

	#endregion
}