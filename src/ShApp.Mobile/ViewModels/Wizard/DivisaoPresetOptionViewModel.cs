using CommunityToolkit.Mvvm.ComponentModel;

namespace ShApp.Mobile.ViewModels.Wizard;

public partial class DivisaoPresetOptionViewModel : ObservableObject
{
    public DivisaoPresetDefinition Definicao { get; }

    public bool TemBadge => Definicao.Badge is not null;

    [ObservableProperty]
    public partial bool IsSelecionado { get; set; }

    public DivisaoPresetOptionViewModel(DivisaoPresetDefinition definicao)
    {
        Definicao = definicao;
    }
}
