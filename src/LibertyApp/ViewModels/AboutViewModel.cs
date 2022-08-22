using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibertyApp.Language;
using LibertyApp.Properties;
using System;
using System.Diagnostics;
using System.Windows;

namespace LibertyApp.ViewModels;
public class AboutViewModel : ObservableObject
{
	public IRelayCommand<object> OpenInBrowserCommand { get; }

	public AboutViewModel()
	{
		OpenInBrowserCommand = new RelayCommand<object>(OpenInBrowser);
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
					"Telegram" => Resources.Telegram,
					"Twitter" => Resources.Twitter,
					"Github" => Resources.Github,
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