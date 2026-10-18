using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShApp.Core.Entities;
using ShApp.Core.Interfaces;
using ShApp.Mobile.ViewModels.Items;
using ShApp.Mobile.ViewModels.Support;

namespace ShApp.Mobile.ViewModels;

public partial class SessaoDeTreinoViewModel : ViewModelBase
{
    private enum EstadoExecucao
    {
        SeriePendente,
        Descansando,
        AvancarExercicio,
        Finalizar,
    }

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRotinaRepository _rotinaRepository;
    private readonly ISessaoDeTreinoRepository _sessaoDeTreinoRepository;

    private bool _inicializado;
    private bool _sessaoSalva;
    private Guid _usuarioId;
    private SessaoDeTreino? _sessao;
    private IDispatcherTimer? _timerDescanso;

    [ObservableProperty]
    public partial bool IsEmptyState { get; set; }

    [ObservableProperty]
    public partial bool HasTreino { get; set; }

    [ObservableProperty]
    public partial string EmptyStateMensagem { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ExercicioAtual))]
    [NotifyPropertyChangedFor(nameof(ProximoExercicio))]
    [NotifyPropertyChangedFor(nameof(ProximoExercicioLabel))]
    [NotifyPropertyChangedFor(nameof(BotaoPrincipalLabel))]
    public partial int ExercicioAtualIndex { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BotaoPrincipalLabel))]
    public partial bool IsDescansoAtivo { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DescansoLabel))]
    [NotifyPropertyChangedFor(nameof(DescansoProgresso))]
    public partial int SegundosRestantes { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasErro))]
    public partial string? ErroSerieLabel { get; set; }

    public ObservableCollection<ExercicioExecucaoViewModel> Exercicios { get; } = [];

    public ExercicioExecucaoViewModel? ExercicioAtual =>
        ExercicioAtualIndex >= 0 && ExercicioAtualIndex < Exercicios.Count ? Exercicios[ExercicioAtualIndex] : null;

    public ExercicioExecucaoViewModel? ProximoExercicio =>
        ExercicioAtualIndex + 1 < Exercicios.Count ? Exercicios[ExercicioAtualIndex + 1] : null;

    public string? ProximoExercicioLabel => ProximoExercicio is not null ? $"Próximo: {ProximoExercicio.NomeExercicio}" : null;

    public bool HasErro => !string.IsNullOrEmpty(ErroSerieLabel);

    public string DescansoLabel => TimeSpan.FromSeconds(Math.Max(SegundosRestantes, 0)).ToString(@"mm\:ss");

    public double DescansoProgresso => DescansoConfig.SegundosPadrao == 0
        ? 0
        : Math.Clamp((double)SegundosRestantes / DescansoConfig.SegundosPadrao, 0, 1);

    public string BotaoPrincipalLabel => ObterEstado() switch
    {
        EstadoExecucao.Descansando => "Pular Descanso",
        EstadoExecucao.AvancarExercicio => $"Avançar para: {ProximoExercicio?.NomeExercicio}",
        EstadoExecucao.Finalizar => "Finalizar Treino",
        _ => "Concluir Série",
    };

    public SessaoDeTreinoViewModel(
        IUsuarioRepository usuarioRepository,
        IRotinaRepository rotinaRepository,
        ISessaoDeTreinoRepository sessaoDeTreinoRepository)
    {
        _usuarioRepository = usuarioRepository;
        _rotinaRepository = rotinaRepository;
        _sessaoDeTreinoRepository = sessaoDeTreinoRepository;

        IsEmptyState = true;
        EmptyStateMensagem = "Nenhuma rotina ativa.";
        SegundosRestantes = DescansoConfig.SegundosPadrao;
    }

    // Carrega a sessão de hoje uma única vez: recarregar destruiria séries já confirmadas
    // se o usuário navegar para fora e voltar à tela (mesmo padrão de RotinaWizardViewModel.InicializarAsync).
    public async Task InicializarAsync()
    {
        if (_inicializado)
            return;
        _inicializado = true;

        IsBusy = true;
        try
        {
            var usuario = await _usuarioRepository.ObterPrimeiroAsync();
            if (usuario is null)
            {
                DefinirEmptyState("Nenhum usuário cadastrado ainda.");
                return;
            }
            _usuarioId = usuario.Id;

            var rotina = await _rotinaRepository.ObterAtivaPorUsuarioAsync(usuario.Id);
            if (rotina is null)
            {
                DefinirEmptyState("Nenhuma rotina ativa.");
                return;
            }

            var hoje = DateOnly.FromDateTime(DateTime.Today);
            var ehDiaDeTreino = hoje >= rotina.DataInicio && rotina.DiasDaSemana.Contains(hoje.DayOfWeek);
            if (!ehDiaDeTreino)
            {
                DefinirEmptyState("Hoje é dia de descanso.");
                return;
            }

            var diaHoje = rotina.ObterDiaDeTreinoPara(hoje);
            var itensHoje = rotina.Itens
                .Where(i => i.DiaDeTreino.Id == diaHoje.Id)
                .OrderBy(i => i.Ordem)
                .ToList();

            if (itensHoje.Count == 0)
            {
                DefinirEmptyState("Este dia não tem exercícios configurados.");
                return;
            }

            var sessao = new SessaoDeTreino(hoje, diaHoje);

            Exercicios.Clear();
            var ordem = 1;
            foreach (var item in itensHoje)
            {
                var ultimoPeso = await _sessaoDeTreinoRepository.ObterUltimoPesoUtilizadoAsync(_usuarioId, item.Exercicio.Id);
                var pesoInicial = ultimoPeso ?? 0;

                var registro = new RegistroExercicio(item.Exercicio);
                sessao.AdicionarRegistro(registro);

                var exercicioVm = new ExercicioExecucaoViewModel(registro, item.Exercicio.Nome, item.Exercicio.GrupoMuscular, ordem, itensHoje.Count);
                for (var numero = 1; numero <= item.SeriesAlvo; numero++)
                    exercicioVm.Series.Add(new SerieExecucaoViewModel(numero, item.RepeticoesAlvo, pesoInicial));

                Exercicios.Add(exercicioVm);
                ordem++;
            }

            Exercicios[0].Series[0].IsAtual = true;
            _sessao = sessao;
            ExercicioAtualIndex = 0;

            IsEmptyState = false;
            HasTreino = true;
            NotificarEstadoAtual();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void DefinirEmptyState(string mensagem)
    {
        IsEmptyState = true;
        HasTreino = false;
        EmptyStateMensagem = mensagem;
    }

    private EstadoExecucao ObterEstado()
    {
        if (IsDescansoAtivo)
            return EstadoExecucao.Descansando;

        if (ExercicioAtual is null || !ExercicioAtual.TodasSeriesConcluidas)
            return EstadoExecucao.SeriePendente;

        return ProximoExercicio is not null ? EstadoExecucao.AvancarExercicio : EstadoExecucao.Finalizar;
    }

    // ExercicioAtual/ProximoExercicio/BotaoPrincipalLabel são computados sob demanda; nada os notifica
    // automaticamente quando apenas o estado de uma série (dentro da coleção) muda, ou quando
    // ExercicioAtualIndex permanece 0 na carga inicial. Chamar explicitamente nesses casos.
    private void NotificarEstadoAtual()
    {
        OnPropertyChanged(nameof(ExercicioAtual));
        OnPropertyChanged(nameof(ProximoExercicio));
        OnPropertyChanged(nameof(ProximoExercicioLabel));
        OnPropertyChanged(nameof(BotaoPrincipalLabel));
    }

    [RelayCommand]
    private async Task AcaoPrincipalAsync()
    {
        switch (ObterEstado())
        {
            case EstadoExecucao.SeriePendente:
                ConcluirSerieAtual();
                break;
            case EstadoExecucao.Descansando:
                EncerrarDescanso();
                break;
            case EstadoExecucao.AvancarExercicio:
                AvancarExercicio();
                break;
            case EstadoExecucao.Finalizar:
                await SalvarEFecharAsync();
                break;
        }
    }

    private void ConcluirSerieAtual()
    {
        var exercicio = ExercicioAtual;
        var serie = exercicio?.Series.FirstOrDefault(s => s.IsAtual);
        if (exercicio is null || serie is null)
            return;

        // Validação ANTES de construir o objeto de domínio: SerieRegistrada lança ArgumentException
        // para repetições <= 0 ou peso negativo, e essa exceção não deve estourar na UI.
        if (serie.RepeticoesFeitas < 1)
        {
            ErroSerieLabel = "Informe ao menos 1 repetição para confirmar a série.";
            return;
        }

        if (serie.PesoUsado < 0)
        {
            ErroSerieLabel = "O peso não pode ser negativo.";
            return;
        }

        ErroSerieLabel = null;

        exercicio.Registro.AdicionarSerie(new SerieRegistrada(serie.PesoUsado, serie.RepeticoesFeitas));
        serie.IsConcluida = true;
        serie.IsAtual = false;

        var indice = exercicio.Series.IndexOf(serie);
        var proximaSerie = indice + 1 < exercicio.Series.Count ? exercicio.Series[indice + 1] : null;
        if (proximaSerie is not null)
        {
            // Peso inicial da próxima série = peso confirmado agora, não o último peso consultado no repositório.
            proximaSerie.PesoUsado = serie.PesoUsado;
            proximaSerie.IsAtual = true;
        }

        var ultimaSerieDoUltimoExercicio = proximaSerie is null && ProximoExercicio is null;
        if (!ultimaSerieDoUltimoExercicio)
            IniciarDescanso();

        NotificarEstadoAtual();
    }

    private void AvancarExercicio()
    {
        if (ProximoExercicio is null)
            return;

        ErroSerieLabel = null;
        ExercicioAtualIndex++;

        var novo = ExercicioAtual;
        if (novo is not null && novo.Series.Count > 0)
            novo.Series[0].IsAtual = true;
    }

    private void IniciarDescanso()
    {
        PararTimerInterno();

        SegundosRestantes = DescansoConfig.SegundosPadrao;
        IsDescansoAtivo = true;

        _timerDescanso = Application.Current!.Dispatcher.CreateTimer();
        _timerDescanso.Interval = TimeSpan.FromSeconds(1);
        _timerDescanso.Tick += OnTimerTick;
        _timerDescanso.Start();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        SegundosRestantes--;
        if (SegundosRestantes <= 0)
            EncerrarDescanso();
    }

    private void EncerrarDescanso()
    {
        PararTimerInterno();
        IsDescansoAtivo = false;
        SegundosRestantes = 0;
    }

    private void PararTimerInterno()
    {
        if (_timerDescanso is null)
            return;

        _timerDescanso.Stop();
        _timerDescanso.Tick -= OnTimerTick;
        _timerDescanso = null;
    }

    /// <summary>Para o timer de descanso. Chamado do OnDisappearing da Page para não deixá-lo rodando em segundo plano.</summary>
    public void Pausar() => PararTimerInterno();

    private async Task SalvarEFecharAsync()
    {
        if (_sessaoSalva || _sessao is null || IsBusy)
            return;

        IsBusy = true;
        try
        {
            PararTimerInterno();
            IsDescansoAtivo = false;
            _sessaoSalva = true;

            // Não persiste uma sessão em que nenhuma série foi de fato confirmada (ex.: "Encerrar
            // Treino" tocado antes de registrar qualquer série) — evita sessões vazias poluindo
            // futuras métricas de frequência/streak.
            var algumaSerieConfirmada = _sessao.Registros.Any(r => r.Series.Count > 0);
            if (algumaSerieConfirmada)
                await _sessaoDeTreinoRepository.AdicionarAsync(_usuarioId, _sessao);

            if (Shell.Current is not null)
                await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EncerrarTreinoAsync() => await SalvarEFecharAsync();

    [RelayCommand]
    private async Task VoltarAsync()
    {
        if (Shell.Current is not null)
            await Shell.Current.GoToAsync("..");
    }
}
