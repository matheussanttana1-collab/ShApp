using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShApp.Core.Interfaces;
using ShApp.Mobile.ViewModels.Support;

namespace ShApp.Mobile.ViewModels;

public partial class PerfilViewModel : ViewModelBase
{
    private static readonly string[] AbreviacoesSemana = ["Seg", "Ter", "Qua", "Qui", "Sex", "Sáb", "Dom"];

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRotinaRepository _rotinaRepository;

    [ObservableProperty]
    public partial string NomeUsuario { get; set; }

    [ObservableProperty]
    public partial string Inicial { get; set; }

    [ObservableProperty]
    public partial string SubtituloLabel { get; set; }

    [ObservableProperty]
    public partial string MembroDesdeLabel { get; set; }

    [ObservableProperty]
    public partial bool HasRotina { get; set; }

    [ObservableProperty]
    public partial string RotinaNomeLabel { get; set; }

    [ObservableProperty]
    public partial string RotinaResumoLabel { get; set; }

    [ObservableProperty]
    public partial bool ProximaSessaoDisponivel { get; set; }

    [ObservableProperty]
    public partial string ProximaSessaoLabel { get; set; }

    // Placeholders — agregação de estatísticas (sessões, volume, sequência, frequência) e biometria
    // ainda não têm fonte de dados (ver DashboardViewModel); ficam para uma próxima etapa.
    [ObservableProperty]
    public partial string SessoesConcluidasLabel { get; set; }

    [ObservableProperty]
    public partial string SessoesConcluidasDeltaLabel { get; set; }

    [ObservableProperty]
    public partial string CargaTotalLabel { get; set; }

    [ObservableProperty]
    public partial string CargaTotalDeltaLabel { get; set; }

    [ObservableProperty]
    public partial string SequenciaAtivaLabel { get; set; }

    [ObservableProperty]
    public partial string SequenciaAtivaDeltaLabel { get; set; }

    [ObservableProperty]
    public partial string FrequenciaMediaLabel { get; set; }

    [ObservableProperty]
    public partial string FrequenciaMediaDeltaLabel { get; set; }

    [ObservableProperty]
    public partial string PesoAtualLabel { get; set; }

    [ObservableProperty]
    public partial string AlturaLabel { get; set; }

    [ObservableProperty]
    public partial string BfEstimadoLabel { get; set; }

    public string VersaoLabel { get; } = $"ShApp v{AppInfo.Current.VersionString} • Build {AppInfo.Current.BuildString}";

    public PerfilViewModel(IUsuarioRepository usuarioRepository, IRotinaRepository rotinaRepository)
    {
        _usuarioRepository = usuarioRepository;
        _rotinaRepository = rotinaRepository;

        NomeUsuario = "—";
        Inicial = "?";
        SubtituloLabel = string.Empty;
        MembroDesdeLabel = string.Empty;
        RotinaNomeLabel = "Nenhuma rotina ativa";
        RotinaResumoLabel = "Crie uma rotina para começar a treinar.";
        ProximaSessaoLabel = string.Empty;

        SessoesConcluidasLabel = "—";
        SessoesConcluidasDeltaLabel = string.Empty;
        CargaTotalLabel = "— ton";
        CargaTotalDeltaLabel = string.Empty;
        SequenciaAtivaLabel = "— sem";
        SequenciaAtivaDeltaLabel = string.Empty;
        FrequenciaMediaLabel = "— x/sem";
        FrequenciaMediaDeltaLabel = string.Empty;

        PesoAtualLabel = "— kg";
        AlturaLabel = "— m";
        BfEstimadoLabel = "— %";
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            var usuario = await _usuarioRepository.ObterPrimeiroAsync();
            if (usuario is null)
            {
                NomeUsuario = "Convidado";
                Inicial = "?";
                SubtituloLabel = "Nenhum usuário cadastrado ainda.";
                MembroDesdeLabel = "Novo por aqui";
                HasRotina = false;
                ProximaSessaoDisponivel = false;
                return;
            }

            NomeUsuario = usuario.Nome;
            var nomeLimpo = usuario.Nome.Trim();
            Inicial = nomeLimpo.Length > 0 ? nomeLimpo[..1].ToUpperInvariant() : "?";

            var rotinas = (await _rotinaRepository.ObterPorUsuarioAsync(usuario.Id)).ToList();
            MembroDesdeLabel = rotinas.Count > 0
                ? $"Desde {DiaDaSemanaDisplay.FormatarMesAno(rotinas.Min(r => r.DataInicio))}"
                : "Novo por aqui";

            var rotina = await _rotinaRepository.ObterAtivaPorUsuarioAsync(usuario.Id);
            if (rotina is null)
            {
                HasRotina = false;
                SubtituloLabel = "Nenhuma rotina ativa";
                RotinaNomeLabel = "Nenhuma rotina ativa";
                RotinaResumoLabel = "Crie uma rotina para começar a treinar.";
                ProximaSessaoDisponivel = false;
                return;
            }

            HasRotina = true;
            SubtituloLabel = rotina.DivisaoDeTreino.Nome;
            RotinaNomeLabel = rotina.Nome;

            var diasOrdenados = rotina.DiasDaSemana
                .OrderBy(d => ((int)d + 6) % 7)
                .Select(d => AbreviacoesSemana[((int)d + 6) % 7]);
            var diasTexto = string.Join(", ", diasOrdenados);
            var diasRotulo = rotina.DiasDaSemana.Count == 1 ? "1 dia semanal" : $"{rotina.DiasDaSemana.Count} dias semanais";
            RotinaResumoLabel = $"{diasRotulo} ({diasTexto})";

            var hoje = DateOnly.FromDateTime(DateTime.Today);
            var proximas = RotacaoHelper.ObterProximasOcorrencias(rotina, hoje, 1);
            if (proximas.Count > 0)
            {
                var (_, dia) = proximas[0];
                ProximaSessaoDisponivel = true;
                ProximaSessaoLabel = $"{dia.Nome} ({GrupoMuscularDisplay.ObterNomes(dia.GruposMusculares)})";
            }
            else
            {
                ProximaSessaoDisponivel = false;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task VerRotinaAsync()
    {
        if (Shell.Current is not null)
            await Shell.Current.GoToAsync("//rotinas-page");
    }
}
