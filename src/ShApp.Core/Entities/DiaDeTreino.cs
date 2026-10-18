using ShApp.Core.Enums;

namespace ShApp.Core.Entities;

public class DiaDeTreino
{
    private readonly HashSet<GrupoMuscular> _gruposMusculares;

    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public IReadOnlyCollection<GrupoMuscular> GruposMusculares => _gruposMusculares;

    public DiaDeTreino(string nome, IEnumerable<GrupoMuscular> gruposMusculares)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do dia de treino é obrigatório.", nameof(nome));

        _gruposMusculares = new HashSet<GrupoMuscular>(gruposMusculares ?? Enumerable.Empty<GrupoMuscular>());

        if (_gruposMusculares.Count == 0)
            throw new ArgumentException("O dia de treino deve ter ao menos um grupo muscular.", nameof(gruposMusculares));

        Id = Guid.NewGuid();
        Nome = nome;
    }

    private DiaDeTreino(Guid id, string nome, IEnumerable<GrupoMuscular> gruposMusculares)
    {
        Id = id;
        Nome = nome;
        _gruposMusculares = new HashSet<GrupoMuscular>(gruposMusculares);
    }

    public static DiaDeTreino Reconstruir(Guid id, string nome, IEnumerable<GrupoMuscular> gruposMusculares) =>
        new(id, nome, gruposMusculares);
}
