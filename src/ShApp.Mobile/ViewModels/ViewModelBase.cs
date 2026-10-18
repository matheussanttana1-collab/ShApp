using CommunityToolkit.Mvvm.ComponentModel;

namespace ShApp.Mobile.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    public partial bool IsBusy { get; set; }
}
