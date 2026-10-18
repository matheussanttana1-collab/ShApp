using SQLite;

namespace ShApp.Infrastructure.Persistence;

[Table("Usuarios")]
public class UsuarioRecord
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [NotNull]
    public string Nome { get; set; } = string.Empty;
}
