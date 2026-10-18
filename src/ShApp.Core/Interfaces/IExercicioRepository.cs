using ShApp.Core.Entities;
using ShApp.Core.Enums;

namespace ShApp.Core.Interfaces;

public interface IExercicioRepository
{
    Task<IEnumerable<Exercicio>> ObterTodosAsync();
    Task<Exercicio?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Exercicio>> ObterPorGrupoMuscularAsync(GrupoMuscular grupoMuscular);
    Task AdicionarAsync(Exercicio exercicio);
    Task AtualizarAsync(Exercicio exercicio);
    Task RemoverAsync(Guid id);
}
