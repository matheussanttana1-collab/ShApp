using ShApp.Core.Entities;
using ShApp.Core.Interfaces;
using ShApp.Infrastructure.Data;
using ShApp.Infrastructure.Persistence;

namespace ShApp.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbConnectionFactory _connectionFactory;

    public UsuarioRepository(AppDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Usuario?> ObterPrimeiroAsync()
    {
        await GarantirTabelaCriadaAsync();

        var registro = await _connectionFactory.Connection.Table<UsuarioRecord>().FirstOrDefaultAsync();
        return registro is null ? null : ParaEntidade(registro);
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        await GarantirTabelaCriadaAsync();

        var idTexto = id.ToString();
        var registro = await _connectionFactory.Connection.Table<UsuarioRecord>()
            .Where(r => r.Id == idTexto)
            .FirstOrDefaultAsync();

        return registro is null ? null : ParaEntidade(registro);
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        await GarantirTabelaCriadaAsync();
        await _connectionFactory.Connection.InsertAsync(ParaRegistro(usuario));
    }

    private Task GarantirTabelaCriadaAsync() =>
        _connectionFactory.Connection.CreateTableAsync<UsuarioRecord>();

    private static Usuario ParaEntidade(UsuarioRecord registro) =>
        Usuario.Reconstruir(Guid.Parse(registro.Id), registro.Nome);

    private static UsuarioRecord ParaRegistro(Usuario usuario) => new()
    {
        Id = usuario.Id.ToString(),
        Nome = usuario.Nome
    };
}
