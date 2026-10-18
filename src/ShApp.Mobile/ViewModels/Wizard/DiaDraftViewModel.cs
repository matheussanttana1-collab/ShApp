using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ShApp.Core.Enums;
using ShApp.Mobile.ViewModels.Support;

namespace ShApp.Mobile.ViewModels.Wizard;

public partial class DiaDraftViewModel : ObservableObject
{
    public string Letra { get; }
    public IReadOnlyList<GrupoMuscular> GruposMusculares { get; }
    public string GruposLabel { get; }

    public ObservableCollection<ItemDraftViewModel> Itens { get; } = [];

    [ObservableProperty]
    public partial bool IsSelecionada { get; set; }

    public string ContagemLabel => Itens.Count == 1 ? "1 exercício" : $"{Itens.Count} exercícios";

    public int SeriesTotal => Itens.Sum(i => i.Series);

    public string SeriesTotalLabel => SeriesTotal == 1 ? "1 série" : $"{SeriesTotal} séries";

    public string TempoMedioLabel
    {
        get
        {
            var minutos = Math.Round(SeriesTotal * DescansoConfig.SegundosPadrao / 60.0);
            return $"~{minutos:0} min";
        }
    }

    public DiaDraftViewModel(string letra, IReadOnlyList<GrupoMuscular> gruposMusculares)
    {
        Letra = letra;
        GruposMusculares = gruposMusculares;
        GruposLabel = GrupoMuscularDisplay.ObterNomes(gruposMusculares);
        Itens.CollectionChanged += OnItensChanged;
    }

    private void OnItensChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
            foreach (ItemDraftViewModel item in e.OldItems)
                item.PropertyChanged -= OnItemPropertyChanged;

        if (e.NewItems is not null)
            foreach (ItemDraftViewModel item in e.NewItems)
                item.PropertyChanged += OnItemPropertyChanged;

        NotificarMetricas();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ItemDraftViewModel.Series))
            NotificarMetricas();
    }

    private void NotificarMetricas()
    {
        OnPropertyChanged(nameof(ContagemLabel));
        OnPropertyChanged(nameof(SeriesTotal));
        OnPropertyChanged(nameof(SeriesTotalLabel));
        OnPropertyChanged(nameof(TempoMedioLabel));
    }
}
