using CommunityToolkit.Mvvm.ComponentModel;

namespace ShApp.Mobile.ViewModels.Wizard;

public partial class DiaDaSemanaToggleViewModel : ObservableObject
{
    public DayOfWeek DiaDaSemana { get; }
    public string Abreviacao { get; }

    [ObservableProperty]
    public partial bool IsSelecionado { get; set; }

    public DiaDaSemanaToggleViewModel(DayOfWeek diaDaSemana, string abreviacao, bool isSelecionado)
    {
        DiaDaSemana = diaDaSemana;
        Abreviacao = abreviacao;
        IsSelecionado = isSelecionado;
    }
}
