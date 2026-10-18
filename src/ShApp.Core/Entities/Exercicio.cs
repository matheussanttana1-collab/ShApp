using ShApp.Core.Enums;

namespace ShApp.Core.Entities;

public class Exercicio
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public GrupoMuscular GrupoMuscular { get; private set; }

    public Exercicio(string nome, GrupoMuscular grupoMuscular)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do exercício é obrigatório.", nameof(nome));

        Id = Guid.NewGuid();
        Nome = nome;
        GrupoMuscular = grupoMuscular;
    }

    private Exercicio(Guid id, string nome, GrupoMuscular grupoMuscular)
    {
        Id = id;
        Nome = nome;
        GrupoMuscular = grupoMuscular;
    }

    public static Exercicio Reconstruir(Guid id, string nome, GrupoMuscular grupoMuscular) =>
        new(id, nome, grupoMuscular);
}
