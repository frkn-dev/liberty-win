using System.Windows;
using LibertyApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LibertyApp.Views;

public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<MainWindowViewModel>();
    }
}
