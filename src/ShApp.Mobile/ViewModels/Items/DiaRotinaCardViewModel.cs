using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShApp.Mobile.ViewModels.Items;

public partial class DiaRotinaCardViewModel : ObservableObject
{
    public Guid DiaDeTreinoId { get; }
    public string Letra { get; }
    public string NomeGrupos { get; }
    public string ResumoExerciciosCurto { get; }
    public string ContagemLabel { get; }
    public bool IsToday { get; }

    public ObservableCollection<ExercicioRowViewModel> Exercicios { get; }

    [ObservableProperty]
    public partial bool IsExpanded { get; set; }

    public DiaRotinaCardViewModel(
        Guid diaDeTreinoId,
        string letra,
        string nomeGrupos,
        IReadOnlyList<ExercicioRowViewModel> exercicios,
        bool isToday)
    {
        DiaDeTreinoId = diaDeTreinoId;
        Letra = letra;
        NomeGrupos = nomeGrupos;
        IsToday = isToday;
        IsExpanded = isToday;

        Exercicios = new ObservableCollection<ExercicioRowViewModel>(exercicios);
        ResumoExerciciosCurto = string.Join(", ", exercicios.Select(e => e.NomeExercicio));
        ContagemLabel = $"{exercicios.Count} exer.";
    }

    [RelayCommand]
    private void ToggleExpand() => IsExpanded = !IsExpanded;
}
