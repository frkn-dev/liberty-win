using LibertyApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace LibertyApp.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = App.Current.Services.GetService<MainWindowViewModel>();
	}
}
