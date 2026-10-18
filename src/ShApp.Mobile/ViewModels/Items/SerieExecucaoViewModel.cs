using CommunityToolkit.Mvvm.ComponentModel;

namespace ShApp.Mobile.ViewModels.Items;

/// <summary>
/// Estado de uma série durante a execução de um treino: a meta (não editável) e o que o usuário
/// efetivamente registrou (editável até a série ser confirmada).
/// </summary>
public partial class SerieExecucaoViewModel : ObservableObject
{
    public int Numero { get; }
    public int RepeticoesMeta { get; }

    [ObservableProperty]
    public partial int RepeticoesFeitas { get; set; }

    [ObservableProperty]
    public partial decimal PesoUsado { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFutura))]
    public partial bool IsConcluida { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFutura))]
    public partial bool IsAtual { get; set; }

    /// <summary>Série ainda não alcançada: nem concluída, nem a atual (editável).</summary>
    public bool IsFutura => !IsConcluida && !IsAtual;

    public SerieExecucaoViewModel(int numero, int repeticoesMeta, decimal pesoInicial)
    {
        Numero = numero;
        RepeticoesMeta = repeticoesMeta;
        RepeticoesFeitas = repeticoesMeta;
        PesoUsado = pesoInicial;
    }
}
