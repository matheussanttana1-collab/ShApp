using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ShApp.Core.Entities;
using ShApp.Core.Enums;
using ShApp.Mobile.ViewModels.Support;

namespace ShApp.Mobile.ViewModels.Items;

/// <summary>
/// Estado de um exercício durante a execução de um treino: identifica o exercício, mantém as séries
/// (<see cref="SerieExecucaoViewModel"/>) sendo executadas e referencia o <see cref="RegistroExercicio"/>
/// de domínio que vai recebendo cada série confirmada via <see cref="RegistroExercicio.AdicionarSerie"/>.
/// </summary>
public partial class ExercicioExecucaoViewModel : ObservableObject
{
    public RegistroExercicio Registro { get; }
    public string NomeExercicio { get; }
    public string GrupoMuscularLabel { get; }
    public string OrdemLabel { get; }

    public ObservableCollection<SerieExecucaoViewModel> Series { get; } = [];

    public bool TodasSeriesConcluidas => Series.Count > 0 && Series.All(s => s.IsConcluida);

    public ExercicioExecucaoViewModel(
        RegistroExercicio registro,
        string nomeExercicio,
        GrupoMuscular grupoMuscular,
        int ordem,
        int totalExercicios)
    {
        Registro = registro;
        NomeExercicio = nomeExercicio;
        GrupoMuscularLabel = GrupoMuscularDisplay.ObterNome(grupoMuscular);
        OrdemLabel = $"Exercício {ordem} de {totalExercicios}";
    }
}
