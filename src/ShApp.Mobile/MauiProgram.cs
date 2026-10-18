using Microsoft.Extensions.Logging;
using ShApp.Core.Interfaces;
using ShApp.Infrastructure.Data;
using ShApp.Infrastructure.Repositories;
using ShApp.Infrastructure.Seeding;
using ShApp.Mobile.ViewModels;
using ShApp.Mobile.Views;

namespace ShApp.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Geist-Light.ttf", "GeistLight");
				fonts.AddFont("Geist-Regular.ttf", "GeistRegular");
				fonts.AddFont("Geist-Medium.ttf", "GeistMedium");
				fonts.AddFont("Geist-SemiBold.ttf", "GeistSemiBold");
				fonts.AddFont("Geist-Bold.ttf", "GeistBold");
				fonts.AddFont("JetBrainsMono-Regular.ttf", "JetBrainsMonoRegular");
				fonts.AddFont("JetBrainsMono-Medium.ttf", "JetBrainsMonoMedium");
				fonts.AddFont("JetBrainsMono-SemiBold.ttf", "JetBrainsMonoSemiBold");
				fonts.AddFont("MaterialSymbolsOutlined.ttf", "MaterialSymbolsOutlined");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var caminhoBancoDeDados = Path.Combine(FileSystem.AppDataDirectory, "shapp.db3");
		builder.Services.AddSingleton(new AppDbConnectionFactory(caminhoBancoDeDados));
		builder.Services.AddSingleton<IExercicioRepository, ExercicioRepository>();
		builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
		builder.Services.AddSingleton<IDivisaoDeTreinoRepository, DivisaoDeTreinoRepository>();
		builder.Services.AddSingleton<IRotinaRepository, RotinaRepository>();
		builder.Services.AddSingleton<ISessaoDeTreinoRepository, SessaoDeTreinoRepository>();
		builder.Services.AddSingleton<DadosPadraoSeeder>();

		builder.Services.AddTransient<CadastroUsuarioViewModel>();
		builder.Services.AddTransient<DashboardViewModel>();
		builder.Services.AddTransient<RotinasViewModel>();
		builder.Services.AddTransient<RotinaWizardViewModel>();
		builder.Services.AddTransient<PerfilViewModel>();
		builder.Services.AddTransient<SessaoDeTreinoViewModel>();
		builder.Services.AddTransient<CadastroUsuarioPage>();
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<RotinasPage>();
		builder.Services.AddTransient<RotinaWizardPage>();
		builder.Services.AddTransient<PerfilPage>();
		builder.Services.AddTransient<SessaoDeTreinoPage>();
		builder.Services.AddTransient<IniciarTreinoPage>();

		return builder.Build();
	}
}
