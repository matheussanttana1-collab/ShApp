using ShApp.Mobile.ViewModels;

namespace ShApp.Mobile.Views;

public partial class CadastroUsuarioPage : ContentPage
{
	private readonly CadastroUsuarioViewModel _viewModel;

	public CadastroUsuarioPage(CadastroUsuarioViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_ = _viewModel.InicializarAsync();
	}
}
