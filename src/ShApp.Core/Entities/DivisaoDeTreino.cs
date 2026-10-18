namespace ShApp.Core.Entities;

public class DivisaoDeTreino
{
    private readonly List<DiaDeTreino> _dias = new();

    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public IReadOnlyCollection<DiaDeTreino> Dias => _dias;

    public DivisaoDeTreino(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome da divisão de treino é obrigatório.", nameof(nome));

        Id = Guid.NewGuid();
        Nome = nome;
    }

    private DivisaoDeTreino(Guid id, string nome)
    {
        Id = id;
        Nome = nome;
    }

    public static DivisaoDeTreino Reconstruir(Guid id, string nome) =>
        new(id, nome);

    public void AdicionarDia(DiaDeTreino dia)
    {
        ArgumentNullException.ThrowIfNull(dia);
        _dias.Add(dia);
    }
}
