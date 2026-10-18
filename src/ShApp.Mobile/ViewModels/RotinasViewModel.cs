using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShApp.Core.Entities;
using ShApp.Core.Interfaces;
using ShApp.Mobile.ViewModels.Items;
using ShApp.Mobile.ViewModels.Support;

namespace ShApp.Mobile.ViewModels;

public partial class RotinasViewModel : ViewModelBase
{
    private static readonly string[] AbreviacoesSemana = ["SEG", "TER", "QUA", "QUI", "SEX", "SÁB", "DOM"];

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRotinaRepository _rotinaRepository;
    private readonly ISessaoDeTreinoRepository _sessaoDeTreinoRepository;

    private Guid? _rotinaAtivaId;

    [ObservableProperty]
    public partial bool IsEmptyState { get; set; }

    [ObservableProperty]
    public partial bool HasRotina { get; set; }

    [ObservableProperty]
    public partial string EmptyStateMessage { get; set; }

    [ObservableProperty]
    public partial string NomeDoPlano { get; set; }

    [ObservableProperty]
    public partial string DescricaoDoPlano { get; set; }

    [ObservableProperty]
    public partial string PlanoResumoTopo { get; set; }

    [ObservableProperty]
    public partial string DataInicioLabel { get; set; }

    [ObservableProperty]
    public partial string ProximaSessaoResumo { get; set; }

    [ObservableProperty]
    public partial string HojeResumoLabel { get; set; }

    [ObservableProperty]
    public partial string PassoAtualLabel { get; set; }

    [ObservableProperty]
    public partial string EstruturaHeaderLabel { get; set; }

    [ObservableProperty]
    public partial string EstruturaContagemLabel { get; set; }

    [ObservableProperty]
    public partial string RodapeCicloLabel { get; set; }

    [ObservableProperty]
    public partial string IniciarSessaoLabel { get; set; }

    public ObservableCollection<CicloSemanalDiaViewModel> CicloSemanal { get; } = [];

    public ObservableCollection<DiaRotinaCardViewModel> DiasDaDivisao { get; } = [];

    public RotinasViewModel(
        IUsuarioRepository usuarioRepository,
        IRotinaRepository rotinaRepository,
        ISessaoDeTreinoRepository sessaoDeTreinoRepository)
    {
        _usuarioRepository = usuarioRepository;
        _rotinaRepository = rotinaRepository;
        _sessaoDeTreinoRepository = sessaoDeTreinoRepository;

        IsEmptyState = true;
        EmptyStateMessage = "Nenhuma rotina ativa.";
        NomeDoPlano = string.Empty;
        DescricaoDoPlano = string.Empty;
        PlanoResumoTopo = string.Empty;
        DataInicioLabel = string.Empty;
        ProximaSessaoResumo = string.Empty;
        HojeResumoLabel = string.Empty;
        PassoAtualLabel = string.Empty;
        EstruturaHeaderLabel = string.Empty;
        EstruturaContagemLabel = string.Empty;
        RodapeCicloLabel = string.Empty;
        IniciarSessaoLabel = string.Empty;
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
                SetEmptyState("Nenhum usuário cadastrado ainda.");
                return;
            }

            var rotina = await _rotinaRepository.ObterAtivaPorUsuarioAsync(usuario.Id);
            if (rotina is null)
            {
                SetEmptyState("Nenhuma rotina ativa.");
                return;
            }

            IsEmptyState = false;
            HasRotina = true;
            _rotinaAtivaId = rotina.Id;

            var divisao = rotina.DivisaoDeTreino;
            var hoje = DateOnly.FromDateTime(DateTime.Today);
            var hojeEhDiaDeTreino = hoje >= rotina.DataInicio && rotina.DiasDaSemana.Contains(hoje.DayOfWeek);
            var diaHoje = hojeEhDiaDeTreino ? rotina.ObterDiaDeTreinoPara(hoje) : null;

            NomeDoPlano = rotina.Nome;

            var diasRotativosLabel = divisao.Dias.Count == 1 ? "1 dia rotativo" : $"{divisao.Dias.Count} dias rotativos";
            DescricaoDoPlano = $"Divisão {divisao.Nome} Contínua • {diasRotativosLabel} • {rotina.DiasDaSemana.Count}x por semana";
            PlanoResumoTopo = $"Plano: {divisao.Nome} Contínuo ({rotina.DiasDaSemana.Count}x/sem)";
            DataInicioLabel = $"Início: {DiaDaSemanaDisplay.FormatarDataCurta(rotina.DataInicio)}";
            RodapeCicloLabel = DataInicioLabel;

            var proxima = RotacaoHelper.ObterProximasOcorrencias(rotina, hoje, 1).FirstOrDefault();
            ProximaSessaoResumo = proxima.Dia is not null
                ? $"Treino {proxima.Dia.Nome} — {GrupoMuscularDisplay.ObterNomes(proxima.Dia.GruposMusculares)}"
                : "Nenhuma sessão agendada";

            HojeResumoLabel = diaHoje is not null
                ? $"{DiaDaSemanaDisplay.ObterNome(hoje.DayOfWeek)} (Treino {diaHoje.Nome})"
                : $"{DiaDaSemanaDisplay.ObterNome(hoje.DayOfWeek)} (Descanso)";

            IniciarSessaoLabel = diaHoje is not null
                ? $"Iniciar Sessão de Hoje (Treino {diaHoje.Nome})"
                : "Nenhum treino hoje";

            if (diaHoje is not null)
            {
                var diasOrdenados = divisao.Dias.ToList();
                var indice = diasOrdenados.FindIndex(d => d.Id == diaHoje.Id);
                PassoAtualLabel = $"Passo {indice + 1} de {diasOrdenados.Count}";
            }
            else
            {
                PassoAtualLabel = "Dia de descanso";
            }

            EstruturaHeaderLabel = $"ESTRUTURA DA DIVISÃO ({divisao.Dias.Count} DIAS)";
            var totalExercicios = rotina.Itens.Count;
            var totalSeries = rotina.Itens.Sum(i => i.SeriesAlvo);
            EstruturaContagemLabel = $"{totalExercicios} Exercícios • {totalSeries} Séries";

            CarregarCicloSemanal(rotina, hoje);
            await CarregarDiasDaDivisaoAsync(usuario.Id, rotina, diaHoje);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CarregarCicloSemanal(Rotina rotina, DateOnly hoje)
    {
        CicloSemanal.Clear();

        var diasDesdeSegunda = ((int)hoje.DayOfWeek + 6) % 7;
        var segunda = hoje.AddDays(-diasDesdeSegunda);

        for (var i = 0; i < 7; i++)
        {
            var data = segunda.AddDays(i);
            string? letra = null;

            if (data >= rotina.DataInicio && rotina.DiasDaSemana.Contains(data.DayOfWeek))
                letra = rotina.ObterDiaDeTreinoPara(data).Nome;

            CicloSemanal.Add(new CicloSemanalDiaViewModel(i, AbreviacoesSemana[i], letra, data == hoje));
        }
    }

    private async Task CarregarDiasDaDivisaoAsync(Guid usuarioId, Rotina rotina, DiaDeTreino? diaHoje)
    {
        DiasDaDivisao.Clear();

        foreach (var dia in rotina.DivisaoDeTreino.Dias)
        {
            var itensDoDia = rotina.Itens
                .Where(i => i.DiaDeTreino.Id == dia.Id)
                .OrderBy(i => i.Ordem)
                .ToList();

            var linhas = new List<ExercicioRowViewModel>();
            var ordem = 1;
            foreach (var item in itensDoDia)
            {
                var ultimoPeso = await _sessaoDeTreinoRepository.ObterUltimoPesoUtilizadoAsync(usuarioId, item.Exercicio.Id);
                linhas.Add(new ExercicioRowViewModel(ordem, item.Exercicio.Nome, item.SeriesAlvo, item.RepeticoesAlvo, ultimoPeso));
                ordem++;
            }

            var isToday = diaHoje is not null && dia.Id == diaHoje.Id;
            DiasDaDivisao.Add(new DiaRotinaCardViewModel(
                dia.Id,
                dia.Nome,
                GrupoMuscularDisplay.ObterNomes(dia.GruposMusculares),
                linhas,
                isToday));
        }
    }

    private void SetEmptyState(string mensagem)
    {
        IsEmptyState = true;
        HasRotina = false;
        EmptyStateMessage = mensagem;
        CicloSemanal.Clear();
        DiasDaDivisao.Clear();
    }

    [RelayCommand]
    private async Task AbrirCriarRotinaAsync()
    {
        if (Shell.Current is not null)
            await Shell.Current.GoToAsync("rotina-wizard?modo=criar");
    }

    [RelayCommand]
    private async Task AbrirEditarRotinaAsync()
    {
        if (_rotinaAtivaId is not { } rotinaId || Shell.Current is null)
            return;

        await Shell.Current.GoToAsync($"rotina-wizard?modo=editar&rotinaId={rotinaId}");
    }
}
