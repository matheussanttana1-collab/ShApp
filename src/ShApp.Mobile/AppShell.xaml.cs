using ShApp.Mobile.Views;

namespace ShApp.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("rotina-wizard", typeof(RotinaWizardPage));
		Routing.RegisterRoute("sessao-treino", typeof(SessaoDeTreinoPage));
	}
}
