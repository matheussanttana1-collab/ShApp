using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShApp.Core.Entities;
using ShApp.Core.Interfaces;

namespace ShApp.Mobile.ViewModels;

public partial class CadastroUsuarioViewModel : ViewModelBase
{
    private readonly IUsuarioRepository _usuarioRepository;

    [ObservableProperty]
    public partial string Nome { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasErro))]
    public partial string? ErroLabel { get; set; }

    public bool HasErro => !string.IsNullOrEmpty(ErroLabel);

    public CadastroUsuarioViewModel(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    // Se já existe um usuário (ex.: reabrindo o app), pula direto para o Shell normal —
    // esta tela só deve ser vista uma vez, na primeira vez que o app roda no aparelho.
    public async Task InicializarAsync()
    {
        var usuario = await _usuarioRepository.ObterPrimeiroAsync();
        if (usuario is not null)
            IrParaApp();
    }

    [RelayCommand]
    private async Task CriarUsuarioAsync()
    {
        if (IsBusy)
            return;

        // Validação ANTES de construir o objeto de domínio: Usuario lança ArgumentException
        // para nome vazio, e essa exceção não deve estourar na UI.
        var nome = Nome.Trim();
        if (string.IsNullOrWhiteSpace(nome))
        {
            ErroLabel = "Informe seu nome para continuar.";
            return;
        }

        ErroLabel = null;
        IsBusy = true;
        try
        {
            var usuario = new Usuario(nome);
            await _usuarioRepository.AdicionarAsync(usuario);
            IrParaApp();
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Troca a página raiz da Window para o Shell normal — sem isso não existe nenhuma
    // forma de acessar o app depois de criar o usuário.
    private static void IrParaApp()
    {
        if (Application.Current is { Windows.Count: > 0 } app)
            app.Windows[0].Page = new AppShell();
    }
}
