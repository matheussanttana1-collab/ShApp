using ShApp.Mobile.ViewModels;

namespace ShApp.Mobile.Views;

public partial class SessaoDeTreinoPage : ContentPage
{
	private readonly SessaoDeTreinoViewModel _viewModel;

	public SessaoDeTreinoPage(SessaoDeTreinoViewModel viewModel)
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

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		_viewModel.Pausar();
	}

	protected override bool OnBackButtonPressed()
	{
		// Mesmo caminho do botão "Encerrar Treino": salva o progresso (já protegido contra
		// chamada dupla por _sessaoSalva) em vez de descartar o treino em silêncio quando o
		// usuário usa o gesto/seta de voltar do sistema em vez do botão explícito.
		_viewModel.EncerrarTreinoCommand.Execute(null);
		return true;
	}
}
