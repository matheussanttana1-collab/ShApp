using SQLite;

namespace ShApp.Infrastructure.Persistence;

[Table("RegistrosExercicio")]
public class RegistroExercicioRecord
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string SessaoDeTreinoId { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string ExercicioId { get; set; } = string.Empty;
}
