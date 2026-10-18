namespace ShApp.Core.Entities;

public class SessaoDeTreino
{
    private readonly List<RegistroExercicio> _registros = new();

    public Guid Id { get; }
    public DateOnly Data { get; }
    public DiaDeTreino DiaDeTreino { get; }
    public IReadOnlyCollection<RegistroExercicio> Registros => _registros;

    public SessaoDeTreino(DateOnly data, DiaDeTreino diaDeTreino)
    {
        ArgumentNullException.ThrowIfNull(diaDeTreino);

        Id = Guid.NewGuid();
        Data = data;
        DiaDeTreino = diaDeTreino;
    }

    private SessaoDeTreino(Guid id, DateOnly data, DiaDeTreino diaDeTreino)
    {
        Id = id;
        Data = data;
        DiaDeTreino = diaDeTreino;
    }

    public static SessaoDeTreino Reconstruir(Guid id, DateOnly data, DiaDeTreino diaDeTreino) =>
        new(id, data, diaDeTreino);

    public void AdicionarRegistro(RegistroExercicio registro)
    {
        ArgumentNullException.ThrowIfNull(registro);
        _registros.Add(registro);
    }
}
