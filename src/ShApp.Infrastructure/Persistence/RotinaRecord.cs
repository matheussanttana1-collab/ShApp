using SQLite;

namespace ShApp.Infrastructure.Persistence;

[Table("Rotinas")]
public class RotinaRecord
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string UsuarioId { get; set; } = string.Empty;

    [NotNull]
    public string Nome { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string DivisaoDeTreinoId { get; set; } = string.Empty;

    [NotNull]
    public DateTime DataInicio { get; set; }
}
