using SQLite;

namespace ShApp.Infrastructure.Persistence;

[Table("SessoesDeTreino")]
public class SessaoDeTreinoRecord
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string UsuarioId { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string DiaDeTreinoId { get; set; } = string.Empty;

    [NotNull]
    public DateTime Data { get; set; }
}
