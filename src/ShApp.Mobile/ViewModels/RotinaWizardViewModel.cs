using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShApp.Core.Entities;
using ShApp.Core.Enums;
using ShApp.Core.Interfaces;
using ShApp.Mobile.ViewModels.Items;
using ShApp.Mobile.ViewModels.Support;
using ShApp.Mobile.ViewModels.Wizard;

namespace ShApp.Mobile.ViewModels;

public enum ModoWizard
{
    Criar,
    Editar,
}

public partial class RotinaWizardViewModel : ViewModelBase
{
    private const string NomePadraoUsuario = "Atleta";
    private static readonly string[] NomesEtapas = ["Estrutura", "Frequência e Dias", "Exercícios", "Revisão"];
    private static readonly string[] AbreviacoesSemana = ["SEG", "TER", "QUA", "QUI", "SEX", "SÁB", "DOM"];
    private static readonly DayOfWeek[] DiasDaSemanaOrdem =
    [
        DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
        DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday,
    ];

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRotinaRepository _rotinaRepository;
    private readonly IDivisaoDeTreinoRepository _divisaoDeTreinoRepository;
    private readonly IExercicioRepository _exercicioRepository;

    private Guid _usuarioId;
    private bool _inicializado;

    public ModoWizard Modo { get; private set; } = ModoWizard.Criar;
    public Guid? RotinaIdEditando { get; private set; }

    [ObservableProperty]
    public partial int CurrentStep { get; set; } = 1;

    public bool IsStep1 => CurrentStep == 1;
    public bool IsStep2 => CurrentStep == 2;
    public bool IsStep3 => CurrentStep == 3;
    public bool IsStep4 => CurrentStep == 4;

    public bool Step1Concluido => CurrentStep > 1;
    public bool Step2Concluido => CurrentStep > 2;
    public bool Step3Concluido => CurrentStep > 3;

    public string PassoAtualLabel => $"PASSO {CurrentStep} DE 4";

    public string TituloEtapaLabel =>
        $"{(Modo == ModoWizard.Criar ? "Criar" : "Editar")} Rotina Etapa {CurrentStep}";

    public string EtapaNomeLabel => $"Etapa {CurrentStep}: {NomesEtapas[CurrentStep - 1]}";

    public double ProgressoFracao => CurrentStep / 4.0;

    public string ProgressoPercentLabel => $"{(int)(ProgressoFracao * 100)}% concluído";

    public bool IsUltimaEtapa => CurrentStep == 4;

    public string BotaoAvancarLabel => CurrentStep == 4 ? "Salvar Rotina & Ativar Agora" : "Próximo";

    // ===================== ETAPA 1: DIVISÃO =====================

    public ObservableCollection<DivisaoPresetOptionViewModel> Presets { get; } =
        new(DivisaoPresets.Catalogo.Select(p => new DivisaoPresetOptionViewModel(p)));

    [ObservableProperty]
    public partial string NomeDaRotina { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsDivisaoCustomSelecionada { get; set; }

    public string TituloDivisaoAtual =>
        Presets.FirstOrDefault(p => p.IsSelecionado)?.Definicao.Titulo
        ?? (IsDivisaoCustomSelecionada ? "Divisão Personalizada (Livre)" : "Nenhuma divisão selecionada");

    public ObservableCollection<DiaDraftViewModel> Dias { get; } = [];

    public bool TemDias => Dias.Count > 0;

    [ObservableProperty]
    public partial string NovoDiaLetra { get; set; } = string.Empty;

    public ObservableCollection<GrupoMuscularToggleViewModel> GruposParaNovoDia { get; } =
        new(Enum.GetValues<GrupoMuscular>().Select(g => new GrupoMuscularToggleViewModel(g, GrupoMuscularDisplay.ObterNome(g))));

    // ===================== ETAPA 2: FREQUÊNCIA =====================

    [ObservableProperty]
    public partial int MetaSemanal { get; set; } = 4;

    public ObservableCollection<DiaDaSemanaToggleViewModel> DiasDaSemanaOpcoes { get; } =
        new(DiasDaSemanaOrdem.Select((d, i) => new DiaDaSemanaToggleViewModel(d, AbreviacoesSemana[i], false)));

    public ObservableCollection<CicloSemanalDiaViewModel> GradeSemanalPreview { get; } = [];

    public string FrequenciaMediaLabel { get; private set; } = string.Empty;

    // ===================== ETAPA 3: EXERCÍCIOS =====================

    [ObservableProperty]
    public partial DiaDraftViewModel? AbaSelecionada { get; set; }

    public ObservableCollection<Exercicio> ExerciciosFiltrados { get; } = [];

    private List<Exercicio> _catalogoExercicios = [];

    [ObservableProperty]
    public partial bool IsPickerExercicioAberto { get; set; }

    [ObservableProperty]
    public partial string FiltroExercicio { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NovoExercicioNome { get; set; } = string.Empty;

    public GrupoMuscular[] TodosGruposMusculares { get; } = Enum.GetValues<GrupoMuscular>();

    [ObservableProperty]
    public partial GrupoMuscular? NovoExercicioGrupoSelecionado { get; set; }

    // ===================== ETAPA 4: REVISÃO =====================

    public string ResumoDivisaoFrequenciaLabel { get; private set; } = string.Empty;

    public string ResumoInicioLabel { get; private set; } = string.Empty;

    public string ResumoTotaisLabel { get; private set; } = string.Empty;

    public RotinaWizardViewModel(
        IUsuarioRepository usuarioRepository,
        IRotinaRepository rotinaRepository,
        IDivisaoDeTreinoRepository divisaoDeTreinoRepository,
        IExercicioRepository exercicioRepository)
    {
        _usuarioRepository = usuarioRepository;
        _rotinaRepository = rotinaRepository;
        _divisaoDeTreinoRepository = divisaoDeTreinoRepository;
        _exercicioRepository = exercicioRepository;

        Dias.CollectionChanged += (_, _) => OnPropertyChanged(nameof(TemDias));
    }

    public void AplicarParametros(ModoWizard modo, Guid? rotinaId)
    {
        Modo = modo;
        RotinaIdEditando = rotinaId;
    }

    partial void OnCurrentStepChanged(int value)
    {
        OnPropertyChanged(nameof(IsStep1));
        OnPropertyChanged(nameof(IsStep2));
        OnPropertyChanged(nameof(IsStep3));
        OnPropertyChanged(nameof(IsStep4));
        OnPropertyChanged(nameof(Step1Concluido));
        OnPropertyChanged(nameof(Step2Concluido));
        OnPropertyChanged(nameof(Step3Concluido));
        OnPropertyChanged(nameof(PassoAtualLabel));
        OnPropertyChanged(nameof(TituloEtapaLabel));
        OnPropertyChanged(nameof(EtapaNomeLabel));
        OnPropertyChanged(nameof(ProgressoFracao));
        OnPropertyChanged(nameof(ProgressoPercentLabel));
        OnPropertyChanged(nameof(IsUltimaEtapa));
        OnPropertyChanged(nameof(BotaoAvancarLabel));

        if (value == 2)
            AtualizarGradeSemanal();

        if (value == 3 && AbaSelecionada is null && Dias.Count > 0)
            SelecionarAba(Dias[0]);

        if (value == 4)
            AtualizarResumoFinal();
    }

    public async Task InicializarAsync()
    {
        if (_inicializado)
            return;
        _inicializado = true;

        var usuario = await _usuarioRepository.ObterPrimeiroAsync();
        if (usuario is null)
        {
            usuario = new Usuario(NomePadraoUsuario);
            await _usuarioRepository.AdicionarAsync(usuario);
        }
        _usuarioId = usuario.Id;

        _catalogoExercicios = (await _exercicioRepository.ObterTodosAsync()).OrderBy(e => e.Nome).ToList();
        AtualizarFiltroExercicios();

        if (Modo == ModoWizard.Editar && RotinaIdEditando is { } rotinaId)
            await CarregarRotinaParaEdicaoAsync(rotinaId);
        else
            SelecionarPreset(Presets[0]);
    }

    private async Task CarregarRotinaParaEdicaoAsync(Guid rotinaId)
    {
        var rotina = await _rotinaRepository.ObterPorIdAsync(rotinaId);
        if (rotina is null)
        {
            SelecionarPreset(Presets[0]);
            return;
        }

        NomeDaRotina = rotina.Nome;
        IsDivisaoCustomSelecionada = true;
        foreach (var preset in Presets)
            preset.IsSelecionado = false;

        Dias.Clear();
        foreach (var dia in rotina.DivisaoDeTreino.Dias)
        {
            var draft = new DiaDraftViewModel(dia.Nome, dia.GruposMusculares.ToList());
            foreach (var item in rotina.Itens.Where(i => i.DiaDeTreino.Id == dia.Id).OrderBy(i => i.Ordem))
                draft.Itens.Add(new ItemDraftViewModel(item.Exercicio, item.SeriesAlvo, item.RepeticoesAlvo));

            Dias.Add(draft);
        }

        MetaSemanal = rotina.DiasDaSemana.Count;
        foreach (var opcao in DiasDaSemanaOpcoes)
            opcao.IsSelecionado = rotina.DiasDaSemana.Contains(opcao.DiaDaSemana);

        OnPropertyChanged(nameof(TituloDivisaoAtual));
    }

    // ===================== COMANDOS — ETAPA 1 =====================

    [RelayCommand]
    private void SelecionarPreset(DivisaoPresetOptionViewModel opcao)
    {
        foreach (var preset in Presets)
            preset.IsSelecionado = preset == opcao;

        IsDivisaoCustomSelecionada = false;

        NomeDaRotina = opcao.Definicao.NomeSugerido;
        MetaSemanal = opcao.Definicao.MetaSemanalSugerida;

        Dias.Clear();
        foreach (var (letra, grupos) in opcao.Definicao.Dias)
            Dias.Add(new DiaDraftViewModel(letra, grupos));

        AplicarDistribuicaoPadrao(MetaSemanal);
        OnPropertyChanged(nameof(TituloDivisaoAtual));
    }

    [RelayCommand]
    private void SelecionarDivisaoCustom()
    {
        foreach (var preset in Presets)
            preset.IsSelecionado = false;

        IsDivisaoCustomSelecionada = true;

        if (string.IsNullOrWhiteSpace(NomeDaRotina) || Presets.Any(p => p.Definicao.NomeSugerido == NomeDaRotina))
            NomeDaRotina = "Divisão Livre Personalizada";

        Dias.Clear();
        OnPropertyChanged(nameof(TituloDivisaoAtual));
    }

    [RelayCommand]
    private void AdicionarDiaCustom()
    {
        var letra = NovoDiaLetra.Trim();
        var grupos = GruposParaNovoDia.Where(g => g.IsSelecionado).Select(g => g.Valor).ToList();

        if (string.IsNullOrWhiteSpace(letra) || grupos.Count == 0)
            return;

        Dias.Add(new DiaDraftViewModel(letra, grupos));

        NovoDiaLetra = string.Empty;
        foreach (var g in GruposParaNovoDia)
            g.IsSelecionado = false;
    }

    [RelayCommand]
    private void RemoverDiaCustom(DiaDraftViewModel dia) => Dias.Remove(dia);

    // ===================== COMANDOS — ETAPA 2 =====================

    [RelayCommand]
    private void IncrementarMeta()
    {
        if (MetaSemanal >= 7)
            return;

        MetaSemanal++;
        AplicarDistribuicaoPadrao(MetaSemanal);
    }

    [RelayCommand]
    private void DecrementarMeta()
    {
        if (MetaSemanal <= 1)
            return;

        MetaSemanal--;
        AplicarDistribuicaoPadrao(MetaSemanal);
    }

    [RelayCommand]
    private void ToggleDiaDaSemana(DiaDaSemanaToggleViewModel opcao)
    {
        opcao.IsSelecionado = !opcao.IsSelecionado;
        MetaSemanal = DiasDaSemanaOpcoes.Count(d => d.IsSelecionado);
        AtualizarGradeSemanal();
    }

    [RelayCommand]
    private void VoltarParaDivisao() => CurrentStep = 1;

    private void AplicarDistribuicaoPadrao(int meta)
    {
        // Distribuição padrão: espalha os dias de treino ao longo da semana, priorizando intercalar com descanso.
        var padroes = new Dictionary<int, DayOfWeek[]>
        {
            [1] = [DayOfWeek.Wednesday],
            [2] = [DayOfWeek.Monday, DayOfWeek.Thursday],
            [3] = [DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday],
            [4] = [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Friday],
            [5] = [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday],
            [6] = [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday],
            [7] = DiasDaSemanaOrdem,
        };

        var selecionados = padroes.GetValueOrDefault(meta, DiasDaSemanaOrdem);
        foreach (var opcao in DiasDaSemanaOpcoes)
            opcao.IsSelecionado = selecionados.Contains(opcao.DiaDaSemana);

        AtualizarGradeSemanal();
    }

    private void AtualizarGradeSemanal()
    {
        GradeSemanalPreview.Clear();

        var diasSelecionados = DiasDaSemanaOpcoes.Where(d => d.IsSelecionado).Select(d => d.DiaDaSemana).ToList();
        var hoje = DateOnly.FromDateTime(DateTime.Today);

        if (Dias.Count == 0 || diasSelecionados.Count == 0)
        {
            FrequenciaMediaLabel = string.Empty;
            OnPropertyChanged(nameof(FrequenciaMediaLabel));
            return;
        }

        var rotinaRascunho = ConstruirRotinaRascunho(diasSelecionados, hoje);

        var diasDesdeSegunda = ((int)hoje.DayOfWeek + 6) % 7;
        var segunda = hoje.AddDays(-diasDesdeSegunda);

        for (var i = 0; i < 7; i++)
        {
            var data = segunda.AddDays(i);
            string? letra = null;

            if (data >= hoje && diasSelecionados.Contains(data.DayOfWeek))
                letra = rotinaRascunho.ObterDiaDeTreinoPara(data).Nome;

            GradeSemanalPreview.Add(new CicloSemanalDiaViewModel(i, AbreviacoesSemana[i], letra, data == hoje));
        }

        var mediaDias = Math.Round((double)Dias.Count / MetaSemanal * 7, 1);
        FrequenciaMediaLabel =
            $"Com {MetaSemanal} treinos/semana em ciclo contínuo, cada grupamento será estimulado a cada {mediaDias:0.0} dias em média.";
        OnPropertyChanged(nameof(FrequenciaMediaLabel));
    }

    private Rotina ConstruirRotinaRascunho(IReadOnlyList<DayOfWeek> diasSelecionados, DateOnly dataInicio)
    {
        var divisaoRascunho = new DivisaoDeTreino(string.IsNullOrWhiteSpace(NomeDaRotina) ? "Rascunho" : NomeDaRotina);
        foreach (var dia in Dias)
            divisaoRascunho.AdicionarDia(new DiaDeTreino(dia.Letra, dia.GruposMusculares));

        return new Rotina(
            string.IsNullOrWhiteSpace(NomeDaRotina) ? "Rascunho" : NomeDaRotina,
            divisaoRascunho,
            dataInicio,
            diasSelecionados);
    }

    // ===================== COMANDOS — ETAPA 3 =====================

    [RelayCommand]
    private void SelecionarAba(DiaDraftViewModel dia)
    {
        foreach (var d in Dias)
            d.IsSelecionada = d == dia;

        AbaSelecionada = dia;
    }

    [RelayCommand]
    private void AbrirPickerExercicio()
    {
        FiltroExercicio = string.Empty;
        AtualizarFiltroExercicios();
        IsPickerExercicioAberto = true;
    }

    [RelayCommand]
    private void FecharPickerExercicio() => IsPickerExercicioAberto = false;

    partial void OnFiltroExercicioChanged(string value) => AtualizarFiltroExercicios();

    private void AtualizarFiltroExercicios()
    {
        ExerciciosFiltrados.Clear();
        var termo = FiltroExercicio.Trim();

        var filtrados = string.IsNullOrEmpty(termo)
            ? _catalogoExercicios
            : _catalogoExercicios.Where(e => e.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase));

        foreach (var exercicio in filtrados)
            ExerciciosFiltrados.Add(exercicio);
    }

    [RelayCommand]
    private void AdicionarExercicioAoDia(Exercicio exercicio)
    {
        if (AbaSelecionada is null)
            return;

        AbaSelecionada.Itens.Add(new ItemDraftViewModel(exercicio, 4, 10));
        IsPickerExercicioAberto = false;
    }

    [RelayCommand]
    private void RemoverItem(ItemDraftViewModel item) => AbaSelecionada?.Itens.Remove(item);

    [RelayCommand]
    private async Task CriarNovoExercicioAsync()
    {
        var nome = NovoExercicioNome.Trim();

        if (string.IsNullOrWhiteSpace(nome) || NovoExercicioGrupoSelecionado is not { } grupo)
            return;

        var exercicio = new Exercicio(nome, grupo);
        await _exercicioRepository.AdicionarAsync(exercicio);

        _catalogoExercicios.Add(exercicio);
        _catalogoExercicios = _catalogoExercicios.OrderBy(e => e.Nome).ToList();
        AtualizarFiltroExercicios();

        AdicionarExercicioAoDia(exercicio);

        NovoExercicioNome = string.Empty;
        NovoExercicioGrupoSelecionado = null;
    }

    // ===================== COMANDOS — NAVEGAÇÃO E SALVAMENTO =====================

    [RelayCommand]
    private async Task VoltarAsync()
    {
        if (CurrentStep > 1)
        {
            CurrentStep--;
            return;
        }

        if (Shell.Current is not null)
            await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task AvancarAsync()
    {
        if (!PodeAvancar())
            return;

        if (CurrentStep < 4)
        {
            CurrentStep++;
            return;
        }

        await SalvarAsync();
    }

    private bool PodeAvancar() => CurrentStep switch
    {
        1 => !string.IsNullOrWhiteSpace(NomeDaRotina) && Dias.Count > 0,
        2 => DiasDaSemanaOpcoes.Any(d => d.IsSelecionado),
        _ => true,
    };

    private void AtualizarResumoFinal()
    {
        ResumoDivisaoFrequenciaLabel = $"{TituloDivisaoAtual.Split('(')[0].Trim()} Contínuo • {MetaSemanal}x por semana";

        var diasSelecionados = DiasDaSemanaOpcoes.Where(d => d.IsSelecionado).Select(d => d.DiaDaSemana).ToList();
        var hoje = DateOnly.FromDateTime(DateTime.Today);

        if (Dias.Count > 0 && diasSelecionados.Count > 0)
        {
            var rascunho = ConstruirRotinaRascunho(diasSelecionados, hoje);
            ResumoInicioLabel = diasSelecionados.Contains(hoje.DayOfWeek)
                ? $"Início sugerido: Hoje ({DiaDaSemanaDisplay.ObterNome(hoje.DayOfWeek)} — Treino {rascunho.ObterDiaDeTreinoPara(hoje).Nome})"
                : $"Início sugerido: Hoje ({DiaDaSemanaDisplay.ObterNome(hoje.DayOfWeek)} — Descanso)";
        }
        else
        {
            ResumoInicioLabel = "Início sugerido: Hoje";
        }

        var totalExercicios = Dias.Sum(d => d.Itens.Count);
        var totalSeries = Dias.Sum(d => d.SeriesTotal);
        ResumoTotaisLabel = $"{Dias.Count} blocos • {totalExercicios} exercícios • {totalSeries} séries/semana";

        OnPropertyChanged(nameof(ResumoDivisaoFrequenciaLabel));
        OnPropertyChanged(nameof(ResumoInicioLabel));
        OnPropertyChanged(nameof(ResumoTotaisLabel));
    }

    private async Task SalvarAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            var divisao = new DivisaoDeTreino(NomeDaRotina);
            var mapaDias = new Dictionary<DiaDraftViewModel, DiaDeTreino>();

            foreach (var diaDraft in Dias)
            {
                var diaDeTreino = new DiaDeTreino(diaDraft.Letra, diaDraft.GruposMusculares);
                divisao.AdicionarDia(diaDeTreino);
                mapaDias[diaDraft] = diaDeTreino;
            }

            await _divisaoDeTreinoRepository.AdicionarAsync(divisao);

            var diasDaSemana = DiasDaSemanaOpcoes.Where(d => d.IsSelecionado).Select(d => d.DiaDaSemana).ToList();
            var hoje = DateOnly.FromDateTime(DateTime.Today);
            var rotina = new Rotina(NomeDaRotina, divisao, hoje, diasDaSemana);
            await _rotinaRepository.AdicionarAsync(_usuarioId, rotina);

            foreach (var diaDraft in Dias)
            {
                var diaDeTreino = mapaDias[diaDraft];
                var ordem = 1;
                foreach (var item in diaDraft.Itens)
                {
                    var itemDeRotina = new ItemDeRotina(diaDeTreino, item.Exercicio, item.Series, item.Repeticoes, ordem);
                    rotina.AdicionarItem(itemDeRotina);
                    await _rotinaRepository.AdicionarItemAsync(rotina.Id, itemDeRotina);
                    ordem++;
                }
            }

            if (Shell.Current is not null)
                await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
