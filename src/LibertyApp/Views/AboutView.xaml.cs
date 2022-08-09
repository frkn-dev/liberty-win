using System.Windows.Controls;
using LibertyApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LibertyApp.Views;

public partial class AboutView : UserControl
{
    public AboutViewModel ViewModel => (AboutViewModel)DataContext;

    public AboutView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<AboutViewModel>();
    }
}
