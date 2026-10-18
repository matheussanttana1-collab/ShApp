namespace ShApp.Mobile.ViewModels.Items;

public class ExercicioRowViewModel
{
    public int Ordem { get; }
    public string NumeroLabel { get; }
    public string NomeExercicio { get; }
    public string SeriesRepeticoesLabel { get; }
    public string PesoLabel { get; }

    public ExercicioRowViewModel(int ordem, string nomeExercicio, int seriesAlvo, int repeticoesAlvo, decimal? ultimoPeso)
    {
        Ordem = ordem;
        NumeroLabel = ordem.ToString("00");
        NomeExercicio = nomeExercicio;
        SeriesRepeticoesLabel = $"{seriesAlvo} × {repeticoesAlvo}";
        PesoLabel = ultimoPeso.HasValue ? $"{ultimoPeso.Value:0.#} kg" : "—";
    }
}
