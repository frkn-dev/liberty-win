using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibertyApp.Language;
using LibertyApp.Properties;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;

namespace LibertyApp.ViewModels;

public class DonateViewModel : ObservableObject
{
	public IRelayCommand<object> CopyToClipboardCommand { get; }
	public IRelayCommand<object> OpenInBrowserCommand { get; }

	public DonateViewModel()
	{
		CopyToClipboardCommand = new RelayCommand<object>(CopyToClipboard);
		OpenInBrowserCommand = new RelayCommand<object>(OpenInBrowser);
	}

	private static void CopyToClipboard(object sender)
	{
		try
		{
			var parameter = sender as string;

			var credentials = parameter switch
			{
				"USDT" => Resources.USDT,
				"BTC" => Resources.BTC,
				"Ethereum" => Resources.ETH,
				"NEAR" => Resources.NEAR,
				"Credo" => Resources.Credo,
				_ => throw new ArgumentOutOfRangeException(nameof(sender)),
			};

			Clipboard.SetText(credentials);

			App.Current.NotifyIcon.ShowBalloonTip(3000, Strings.CopyToClipboardCaption, string.Format(Strings.CopyToClipboardMessage, parameter, credentials), ToolTipIcon.Info);
		}
		catch (Exception e)
		{
			MessageBox.Show(e.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	private static void OpenInBrowser(object sender)
	{
		try
		{
			if (sender is null) throw new ArgumentNullException(nameof(sender));

			var parameter = sender as string;

			var processStartInfo = new ProcessStartInfo
			{
				UseShellExecute = true,
				FileName = parameter switch
				{
					"Patreon" => Resources.Patreon,
					"Boosty" => Resources.Boosty,
					"Github" => Resources.Github,
					_ => throw new ArgumentOutOfRangeException(nameof(sender)),
				},
			};

			Process.Start(processStartInfo);
		}
		catch (Exception e)
		{
			MessageBox.Show(e.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}