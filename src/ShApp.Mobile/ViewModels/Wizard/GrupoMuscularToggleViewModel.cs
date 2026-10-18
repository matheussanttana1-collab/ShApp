using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShApp.Core.Enums;

namespace ShApp.Mobile.ViewModels.Wizard;

public partial class GrupoMuscularToggleViewModel : ObservableObject
{
    public GrupoMuscular Valor { get; }
    public string Nome { get; }

    [ObservableProperty]
    public partial bool IsSelecionado { get; set; }

    public GrupoMuscularToggleViewModel(GrupoMuscular valor, string nome, bool isSelecionado = false)
    {
        Valor = valor;
        Nome = nome;
        IsSelecionado = isSelecionado;
    }

    [RelayCommand]
    private void Toggle() => IsSelecionado = !IsSelecionado;
}
