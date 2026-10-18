using SQLite;

namespace ShApp.Infrastructure.Persistence;

[Table("DiaDeTreinoGruposMusculares")]
public class DiaDeTreinoGrupoMuscularRecord
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string DiaDeTreinoId { get; set; } = string.Empty;

    [NotNull]
    public int GrupoMuscular { get; set; }
}
