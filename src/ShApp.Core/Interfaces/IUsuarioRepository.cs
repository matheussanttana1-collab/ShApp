using ShApp.Core.Entities;

namespace ShApp.Core.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPrimeiroAsync();
    Task<Usuario?> ObterPorIdAsync(Guid id);
    Task AdicionarAsync(Usuario usuario);
}
