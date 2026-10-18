using SQLite;

namespace ShApp.Infrastructure.Persistence;

[Table("ItensDeRotina")]
public class ItemDeRotinaRecord
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string RotinaId { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string DiaDeTreinoId { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string ExercicioId { get; set; } = string.Empty;

    [NotNull]
    public int SeriesAlvo { get; set; }

    [NotNull]
    public int RepeticoesAlvo { get; set; }

    [NotNull]
    public int Ordem { get; set; }
}
