namespace ShApp.Mobile.Views;

public partial class IniciarTreinoPage : ContentPage
{
	public IniciarTreinoPage()
	{
		InitializeComponent();
	}

	private async void OnIniciarTreinoClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("sessao-treino");
	}
}
