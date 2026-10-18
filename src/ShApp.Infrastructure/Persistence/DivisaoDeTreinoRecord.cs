using SQLite;

namespace ShApp.Infrastructure.Persistence;

[Table("DivisoesDeTreino")]
public class DivisaoDeTreinoRecord
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [NotNull]
    public string Nome { get; set; } = string.Empty;
}
