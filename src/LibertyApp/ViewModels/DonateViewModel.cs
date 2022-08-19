using LibertyApp.Language;
using LibertyApp.Properties;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
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
				"USDT" => Resource.USDT,
				"BTC" => Resource.BTC,
				"Ethereum" => Resource.ETH,
				"NEAR" => Resource.NEAR,
				"Credo" => Resource.Credo,
				_ => throw new ArgumentOutOfRangeException(nameof(parameter)),
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
					"Patreon" => Resource.Patreon,
					"Boosty" => Resource.Boosty,
					"Github" => Resource.Github,
					_ => throw new ArgumentOutOfRangeException(nameof(parameter)),
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