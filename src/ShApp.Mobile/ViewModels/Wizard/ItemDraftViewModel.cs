using CommunityToolkit.Mvvm.ComponentModel;
using ShApp.Core.Entities;

namespace ShApp.Mobile.ViewModels.Wizard;

public partial class ItemDraftViewModel : ObservableObject
{
    public Exercicio Exercicio { get; }
    public string NomeExercicio { get; }

    [ObservableProperty]
    public partial int Series { get; set; }

    [ObservableProperty]
    public partial int Repeticoes { get; set; }

    public ItemDraftViewModel(Exercicio exercicio, int series, int repeticoes)
    {
        Exercicio = exercicio;
        NomeExercicio = exercicio.Nome;
        Series = series;
        Repeticoes = repeticoes;
    }
}
