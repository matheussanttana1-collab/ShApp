using ShApp.Mobile.ViewModels;

namespace ShApp.Mobile.Views;

public partial class RotinaWizardPage : ContentPage, IQueryAttributable
{
	private readonly RotinaWizardViewModel _viewModel;

	public RotinaWizardPage(RotinaWizardViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		var modo = query.TryGetValue("modo", out var modoValor) && string.Equals(modoValor?.ToString(), "editar", StringComparison.OrdinalIgnoreCase)
			? ModoWizard.Editar
			: ModoWizard.Criar;

		Guid? rotinaId = query.TryGetValue("rotinaId", out var idValor) && Guid.TryParse(idValor?.ToString(), out var id)
			? id
			: null;

		_viewModel.AplicarParametros(modo, rotinaId);
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_ = _viewModel.InicializarAsync();
	}
}
