using SQLite;

namespace ShApp.Infrastructure.Persistence;

// Id é uma chave substituta só para a linha do banco: SerieRegistrada não tem Id no domínio,
// então ele nunca é exposto de volta pelo ParaEntidade.
[Table("SeriesRegistradas")]
public class SerieRegistradaRecord
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string RegistroExercicioId { get; set; } = string.Empty;

    [NotNull]
    public decimal Peso { get; set; }

    [NotNull]
    public int Repeticoes { get; set; }

    [NotNull]
    public int Ordem { get; set; }
}
