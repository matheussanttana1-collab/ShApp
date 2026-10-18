using ShApp.Core.Entities;

namespace ShApp.Core.Interfaces;

public interface IRotinaRepository
{
    Task<Rotina?> ObterAtivaPorUsuarioAsync(Guid usuarioId);
    Task<IEnumerable<Rotina>> ObterPorUsuarioAsync(Guid usuarioId);
    Task<Rotina?> ObterPorIdAsync(Guid id);
    Task AdicionarAsync(Guid usuarioId, Rotina rotina);
    Task AdicionarItemAsync(Guid rotinaId, ItemDeRotina item);
}
