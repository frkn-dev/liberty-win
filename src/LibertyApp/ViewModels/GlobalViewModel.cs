using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace LibertyApp.ViewModels;

public class GlobalViewModel : ObservableObject
{
    public static GlobalViewModel Instance
    {
        get; private set;
    }

    public GlobalViewModel()
    {
        Instance = new GlobalViewModel();
    }
}
