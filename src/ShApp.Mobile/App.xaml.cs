using Microsoft.Extensions.DependencyInjection;
using ShApp.Infrastructure.Seeding;
using ShApp.Mobile.Views;

namespace ShApp.Mobile;

public partial class App : Application
{
	private readonly DadosPadraoSeeder _seeder;
	private readonly IServiceProvider _serviceProvider;

	public App(DadosPadraoSeeder seeder, IServiceProvider serviceProvider)
	{
		InitializeComponent();
		_seeder = seeder;
		_serviceProvider = serviceProvider;
	}

	protected override async void OnStart()
	{
		try
		{
			await _seeder.SemearSeNecessarioAsync();
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Falha ao semear dados padrão: {ex}");
		}
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// Começa pela tela de cadastro: ela mesma pula para o AppShell se já existir um
		// usuário (ObterPrimeiroAsync), e é o único jeito de criar o primeiro usuário do
		// aparelho — sem ela, o Shell inteiro fica preso em empty states sem saída.
		// Resolvida aqui (não injetada no construtor) porque nesse ponto o InitializeComponent()
		// já rodou e os ResourceDictionary de Colors.xaml/Styles.xaml já estão mesclados —
		// construir a página antes disso quebra todo StaticResource (ex.: ObBackground).
		var cadastroUsuarioPage = _serviceProvider.GetRequiredService<CadastroUsuarioPage>();
		var window = new Window(cadastroUsuarioPage);

#if WINDOWS
		window.Created += (_, _) =>
		{
			const int phoneWidth = 420;
			const int phoneHeight = 900;

			var mauiWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
			if (mauiWindow is null)
				return;

			var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(mauiWindow);
			var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
			var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

			if (appWindow is not null)
			{
				appWindow.Resize(new Windows.Graphics.SizeInt32(phoneWidth, phoneHeight));
				appWindow.Title = "ShApp";
			}
		};
#endif

		return window;
	}
}
