using ShApp.Mobile.ViewModels;

namespace ShApp.Mobile.Views;

public partial class RotinasPage : ContentPage
{
	private readonly RotinasViewModel _viewModel;

	public RotinasPage(RotinasViewModel viewModel)
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

	private async void OnPerfilIconTapped(object? sender, TappedEventArgs e)
	{
		if (Shell.Current is not null)
			await Shell.Current.GoToAsync("//perfil-page");
	}
}
