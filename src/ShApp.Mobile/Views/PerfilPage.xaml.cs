using ShApp.Mobile.ViewModels;

namespace ShApp.Mobile.Views;

public partial class PerfilPage : ContentPage
{
	private readonly PerfilViewModel _viewModel;

	public PerfilPage(PerfilViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_ = _viewModel.LoadAsync();
	}
}
