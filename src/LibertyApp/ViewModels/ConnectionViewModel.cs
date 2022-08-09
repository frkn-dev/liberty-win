using LibertyApp.Language;
using LibertyApp.Properties;
using LibertyApp.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
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
		new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/background.jpg"))),
		new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/background-connecting.jpg"))),
		new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/background-connected.jpg")))
	};

	#endregion

	#region Public properties

	/// <summary>
	/// Connection button state text and tooltip
	/// </summary>
	public string ConnectButtonText
	{
		get => _connectButtonText;
		set => SetProperty(ref _connectButtonText, value);
	}
	private string _connectButtonText = Strings.ConnectText;

	/// <summary>
	/// Connection state text
	/// </summary>
	public string ConnectionState
	{
		get => _connectionState;
		set => SetProperty(ref _connectionState, value);
	}
	private string _connectionState = Strings.StatusDisconnected;

	/// <summary>
	/// Connection timer value
	/// </summary>
	public TimeSpan Timer
	{
		get => _timer;
		set => SetProperty(ref _timer, value);
	}
	private TimeSpan _timer;

	/// <summary>
	/// Connection state value
	/// </summary>
	public bool IsConnected
	{
		get => _isConnected;
		set => SetProperty(ref _isConnected, value);
	}
	private bool _isConnected;

	#endregion

	#region Constructors

	public ConnectionViewModel()
	{
		_dispatcherTimer = new DispatcherTimer
		{
			Interval = new TimeSpan(0, 0, 1),
		};

		_dispatcherTimer.Tick += (_, _) =>
		{
			Timer = DateTime.Now - _startTime;
			CommandManager.InvalidateRequerySuggested();
		};

		ConnectCommandAsync = new AsyncRelayCommand<object>(ConnectAsync);
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
	private async Task ConnectAsync(object parameter)
	{
		var control = parameter as ConnectionView;

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
					ArgumentList = {
							"/c",
							"rasdial",
						},
				},
			};

			// if connected already
			if (IsConnected)
			{
				// change interface properties
				ShowConnectingAssets(control);
				ConnectButtonText = ConnectionState = Strings.DisconnectingText;

				// argument to disconnect
				process.StartInfo.ArgumentList.Add("/d");

				process.Start();
				await process.WaitForExitAsync();

				// handle result
				/* List of error codes for dial-up connections or VPN connections: https://docs.microsoft.com/en-us/troubleshoot/windows-client/networking/error-codes-for-dial-up-vpn-connection */
				/* Routing and Remote Access Error Codes: https://docs.microsoft.com/en-us/windows/win32/rras/routing-and-remote-access-error-codes */
				switch (process.ExitCode)
				{
					// disconnect success
					case 0:
						// stop timer
						_dispatcherTimer.Stop();
						Timer = TimeSpan.Zero;
						// change interface properties
						IsConnected = false;
						ShowDefaultAssets(control);
						ConnectButtonText = Strings.ConnectText;
						ConnectionState = Strings.StatusDisconnected;
						break;

					// handle any disconnecting errors
					default:
						MessageBox.Show(process.ExitCode.ToString(),
							Strings.ConnectionErrorCaption,
							MessageBoxButton.OK,
							MessageBoxImage.Error);
						// change interface properties
						ShowDefaultAssets(control);
						ConnectButtonText = Strings.ConnectText;
						ConnectionState = Strings.StatusDisconnected;
						break;
				}
			}
			// if not connected already
			else
			{
				// change interface properties
				ShowConnectingAssets(control);
				ConnectButtonText = Strings.ConnectingText;
				ConnectionState = Strings.StatusConnecting;

				// arguments to establish a connection
				process.StartInfo.ArgumentList.Add(Resource.ConnectionName);
				process.StartInfo.ArgumentList.Add($"/phonebook:{Resource.PhoneBookPath}");

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
						ShowConnectedAssets(control);
						ConnectButtonText = Strings.DisconnectText;
						ConnectionState = Strings.StatusConnected;
						break;

					// any errors
					default:
						MessageBox.Show(process.ExitCode.ToString(),
							Strings.ConnectionErrorCaption,
							MessageBoxButton.OK,
							MessageBoxImage.Error);
						// change interface properties
						ShowDefaultAssets(control);
						ConnectButtonText = Strings.ConnectText;
						ConnectionState = Strings.StatusDisconnected;
						break;
				}
			}
		}
		catch (Exception e)
		{
			MessageBox.Show($"{e.Message}",
				Strings.ConnectionErrorCaption,
				MessageBoxButton.OK,
				MessageBoxImage.Error);

			// change interface properties
			IsConnected = false;
			ShowDefaultAssets(control);
			ConnectButtonText = Strings.ConnectText;
			ConnectionState = Strings.StatusDisconnected;
		}
	}

	#endregion

	#region Private methods

	/// <summary>
	/// Set control default background state
	/// </summary>
	/// <param name="control"></param>
	private static void ShowDefaultAssets(ConnectionView control)
	{
		control.Background = BackgroundImageStates[0];
		control.ConnectionButton.IsCancel = true;
	}

	/// <summary>
	/// Set control background state to connecting
	/// </summary>
	/// <param name="control"></param>
	private static void ShowConnectingAssets(ConnectionView control) =>
		control.Background = BackgroundImageStates[1];

	/// <summary>
	/// Set control background state to connected
	/// </summary>
	/// <param name="control"></param>
	private static void ShowConnectedAssets(ConnectionView control)
	{
		control.Background = BackgroundImageStates[2];
		control.ConnectionButton.IsCancel = false;
	}

	#endregion
}