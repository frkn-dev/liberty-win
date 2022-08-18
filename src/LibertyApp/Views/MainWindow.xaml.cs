using LibertyApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace LibertyApp.Views;

public partial class MainWindow : Window
{
	#region Disabling or Hiding the Maximize Button Of a Window

	/* WPF: Disabling or Hiding the Minimize, Maximize or Close Button Of a Window:
	 https://social.technet.microsoft.com/wiki/contents/articles/29183.wpf-disabling-or-hiding-the-minimize-maximize-or-close-button-of-a-window.aspx */

	[DllImport("user32.dll")]
	private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

	[DllImport("user32.dll")]
	private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

	private const int GwlStyle = -16;

	private const int WsMaximizeBox = 0x10000; //maximize button

	private IntPtr _windowHandle;

	private void MainWindow_SourceInitialized(object sender, EventArgs e)
	{
		_windowHandle = new WindowInteropHelper(this).Handle;
		SetWindowLong(_windowHandle, GwlStyle, GetWindowLong(_windowHandle, GwlStyle) & ~WsMaximizeBox);
	}

	#endregion

	public MainWindow()
	{
		InitializeComponent();
		DataContext = App.Current.Services.GetService<MainWindowViewModel>();
		SourceInitialized += MainWindow_SourceInitialized;
	}

	private void Window_Closing(object sender, CancelEventArgs e)
	{
		(DataContext as MainWindowViewModel)?
			.ShutdownWindowCommand
			.Execute(null);
	}
}
