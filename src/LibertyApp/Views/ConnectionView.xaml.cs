using System.Windows.Controls;
using LibertyApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LibertyApp.Views;

public partial class ConnectionView : UserControl
{
    public ConnectionViewModel ViewModel => (ConnectionViewModel)DataContext;

    public ConnectionView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<ConnectionViewModel>();
    }
}
