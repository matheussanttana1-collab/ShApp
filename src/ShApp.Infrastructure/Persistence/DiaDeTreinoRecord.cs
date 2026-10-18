using SQLite;

namespace ShApp.Infrastructure.Persistence;

[Table("DiasDeTreino")]
public class DiaDeTreinoRecord
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string DivisaoDeTreinoId { get; set; } = string.Empty;

    [NotNull]
    public string Nome { get; set; } = string.Empty;

    [NotNull]
    public int Ordem { get; set; }
}
