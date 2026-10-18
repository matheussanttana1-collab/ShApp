using ShApp.Core.Entities;

namespace ShApp.Core.Interfaces;

public interface ISessaoDeTreinoRepository
{
    Task AdicionarAsync(Guid usuarioId, SessaoDeTreino sessao);
    Task<decimal?> ObterUltimoPesoUtilizadoAsync(Guid usuarioId, Guid exercicioId);
}
