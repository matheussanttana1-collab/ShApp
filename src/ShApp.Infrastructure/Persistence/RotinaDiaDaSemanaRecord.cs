using SQLite;

namespace ShApp.Infrastructure.Persistence;

[Table("RotinaDiasDaSemana")]
public class RotinaDiaDaSemanaRecord
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string RotinaId { get; set; } = string.Empty;

    [NotNull]
    public int DiaDaSemana { get; set; }
}
