using ShApp.Core.Entities;

namespace ShApp.Core.Interfaces;

public interface IDivisaoDeTreinoRepository
{
    Task<DivisaoDeTreino?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<DivisaoDeTreino>> ObterTodasAsync();
    Task AdicionarAsync(DivisaoDeTreino divisaoDeTreino);
}
