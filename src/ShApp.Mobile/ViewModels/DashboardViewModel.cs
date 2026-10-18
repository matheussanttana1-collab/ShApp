using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ShApp.Core.Interfaces;
using ShApp.Mobile.ViewModels.Items;
using ShApp.Mobile.ViewModels.Support;

namespace ShApp.Mobile.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRotinaRepository _rotinaRepository;

    [ObservableProperty]
    public partial bool IsEmptyState { get; set; }

    [ObservableProperty]
    public partial bool HasRotina { get; set; }

    [ObservableProperty]
    public partial string EmptyStateMessage { get; set; }

    [ObservableProperty]
    public partial bool TreinoHojeDisponivel { get; set; }

    [ObservableProperty]
    public partial string CicloLabel { get; set; }

    [ObservableProperty]
    public partial string TreinoHojeBadge { get; set; }

    [ObservableProperty]
    public partial string TreinoHojeTitulo { get; set; }

    [ObservableProperty]
    public partial string TreinoHojeSubtitulo { get; set; }

    // Placeholders — agregação de estatísticas (streak, heatmap, recorde, volume) fica para uma próxima etapa.
    [ObservableProperty]
    public partial string SessaoLabel { get; set; }

    [ObservableProperty]
    public partial string ExerciciosCountLabel { get; set; }

    [ObservableProperty]
    public partial string VolumeLabel { get; set; }

    [ObservableProperty]
    public partial string TempoEstimadoLabel { get; set; }

    [ObservableProperty]
    public partial string StreakLabel { get; set; }

    [ObservableProperty]
    public partial string TotalTreinosLabel { get; set; }

    [ObservableProperty]
    public partial string PeriodoLabel { get; set; }

    [ObservableProperty]
    public partial string ConstanciaLabel { get; set; }

    [ObservableProperty]
    public partial string RecordeValorLabel { get; set; }

    [ObservableProperty]
    public partial string RecordeExercicioLabel { get; set; }

    [ObservableProperty]
    public partial string RecordeDeltaLabel { get; set; }

    [ObservableProperty]
    public partial string VolumeTotalValorLabel { get; set; }

    [ObservableProperty]
    public partial string VolumeTotalDetalheLabel { get; set; }

    [ObservableProperty]
    public partial string VolumeTotalMediaLabel { get; set; }

    public ObservableCollection<SequenciaDiaViewModel> SequenciaDias { get; } = [];

    public DashboardViewModel(IUsuarioRepository usuarioRepository, IRotinaRepository rotinaRepository)
    {
        _usuarioRepository = usuarioRepository;
        _rotinaRepository = rotinaRepository;

        IsEmptyState = true;
        EmptyStateMessage = "Nenhuma rotina ativa.";
        CicloLabel = string.Empty;
        TreinoHojeBadge = string.Empty;
        TreinoHojeTitulo = string.Empty;
        TreinoHojeSubtitulo = string.Empty;

        SessaoLabel = "—";
        ExerciciosCountLabel = "—";
        VolumeLabel = "—";
        TempoEstimadoLabel = "—";
        StreakLabel = "Em breve";
        TotalTreinosLabel = "— treinos";
        PeriodoLabel = DiaDaSemanaDisplay.FormatarMesAno(DateOnly.FromDateTime(DateTime.Today));
        ConstanciaLabel = "—";
        RecordeValorLabel = "—";
        RecordeExercicioLabel = "Sem dados ainda";
        RecordeDeltaLabel = string.Empty;
        VolumeTotalValorLabel = "—";
        VolumeTotalDetalheLabel = "Sem dados ainda";
        VolumeTotalMediaLabel = string.Empty;
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
            CicloLabel = $"{rotina.DivisaoDeTreino.Nome.ToUpperInvariant()} CONTÍNUO";

            var hoje = DateOnly.FromDateTime(DateTime.Today);
            TreinoHojeDisponivel = hoje >= rotina.DataInicio && rotina.DiasDaSemana.Contains(hoje.DayOfWeek);

            if (TreinoHojeDisponivel)
            {
                var diaHoje = rotina.ObterDiaDeTreinoPara(hoje);
                var itensHoje = rotina.Itens
                    .Where(i => i.DiaDeTreino.Id == diaHoje.Id)
                    .OrderBy(i => i.Ordem)
                    .ToList();

                TreinoHojeBadge = diaHoje.Nome;
                TreinoHojeTitulo = GrupoMuscularDisplay.ObterNomes(diaHoje.GruposMusculares);
                TreinoHojeSubtitulo = $"Dia {diaHoje.Nome} • {rotina.DivisaoDeTreino.Nome}";

                ExerciciosCountLabel = itensHoje.Count == 1 ? "1 item" : $"{itensHoje.Count} itens";

                var seriesTotais = itensHoje.Sum(i => i.SeriesAlvo);
                VolumeLabel = $"{seriesTotais} séries";

                var minutosEstimados = Math.Round(seriesTotais * DescansoConfig.SegundosPadrao / 60.0);
                TempoEstimadoLabel = $"~{minutosEstimados:0} min";
            }
            else
            {
                TreinoHojeBadge = "—";
                TreinoHojeTitulo = "Dia de descanso";
                TreinoHojeSubtitulo = "Nenhum treino configurado para hoje";
                ExerciciosCountLabel = "—";
                VolumeLabel = "—";
                TempoEstimadoLabel = "—";
            }

            SequenciaDias.Clear();
            var ocorrencias = RotacaoHelper.ObterProximasOcorrencias(rotina, hoje, 3);
            for (var i = 0; i < ocorrencias.Count; i++)
            {
                var (data, dia) = ocorrencias[i];
                var nomeGrupo = GrupoMuscularDisplay.ObterNomes(dia.GruposMusculares);
                SequenciaDias.Add(new SequenciaDiaViewModel(dia.Nome, nomeGrupo, data, hoje, i));
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SetEmptyState(string mensagem)
    {
        IsEmptyState = true;
        HasRotina = false;
        EmptyStateMessage = mensagem;
        TreinoHojeDisponivel = false;
        SequenciaDias.Clear();
    }
}
