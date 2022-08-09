using System.Windows.Controls;
using LibertyApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LibertyApp.Views;

public partial class DonateView : UserControl
{
    public DonateViewModel ViewModel => (DonateViewModel)DataContext;

    public DonateView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<DonateViewModel>();
    }
}
