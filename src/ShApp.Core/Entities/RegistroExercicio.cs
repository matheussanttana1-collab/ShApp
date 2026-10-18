namespace ShApp.Core.Entities;

public class RegistroExercicio
{
    private readonly List<SerieRegistrada> _series = new();

    public Guid Id { get; }
    public Exercicio Exercicio { get; }
    public IReadOnlyCollection<SerieRegistrada> Series => _series;

    public RegistroExercicio(Exercicio exercicio)
    {
        ArgumentNullException.ThrowIfNull(exercicio);

        Id = Guid.NewGuid();
        Exercicio = exercicio;
    }

    private RegistroExercicio(Guid id, Exercicio exercicio)
    {
        Id = id;
        Exercicio = exercicio;
    }

    public static RegistroExercicio Reconstruir(Guid id, Exercicio exercicio) =>
        new(id, exercicio);

    public void AdicionarSerie(SerieRegistrada serie)
    {
        ArgumentNullException.ThrowIfNull(serie);
        _series.Add(serie);
    }
}
