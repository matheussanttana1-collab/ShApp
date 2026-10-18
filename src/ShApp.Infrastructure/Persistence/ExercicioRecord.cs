using SQLite;

namespace ShApp.Infrastructure.Persistence;

[Table("Exercicios")]
public class ExercicioRecord
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [NotNull]
    public string Nome { get; set; } = string.Empty;

    [NotNull]
    public int GrupoMuscular { get; set; }
}
