using ShApp.Core.Entities;
using ShApp.Core.Enums;
using ShApp.Core.Interfaces;
using ShApp.Infrastructure.Data;
using ShApp.Infrastructure.Persistence;

namespace ShApp.Infrastructure.Repositories;

public class ExercicioRepository : IExercicioRepository
{
    private readonly AppDbConnectionFactory _connectionFactory;

    public ExercicioRepository(AppDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Exercicio>> ObterTodosAsync()
    {
        await GarantirTabelaCriadaAsync();

        var registros = await _connectionFactory.Connection.Table<ExercicioRecord>().ToListAsync();
        return registros.Select(ParaEntidade);
    }

    public async Task<Exercicio?> ObterPorIdAsync(Guid id)
    {
        await GarantirTabelaCriadaAsync();

        var idTexto = id.ToString();
        var registro = await _connectionFactory.Connection.Table<ExercicioRecord>()
            .Where(r => r.Id == idTexto)
            .FirstOrDefaultAsync();

        return registro is null ? null : ParaEntidade(registro);
    }

    public async Task<IEnumerable<Exercicio>> ObterPorGrupoMuscularAsync(GrupoMuscular grupoMuscular)
    {
        await GarantirTabelaCriadaAsync();

        var grupoMuscularValor = (int)grupoMuscular;
        var registros = await _connectionFactory.Connection.Table<ExercicioRecord>()
            .Where(r => r.GrupoMuscular == grupoMuscularValor)
            .ToListAsync();

        return registros.Select(ParaEntidade);
    }

    public async Task AdicionarAsync(Exercicio exercicio)
    {
        await GarantirTabelaCriadaAsync();
        await _connectionFactory.Connection.InsertAsync(ParaRegistro(exercicio));
    }

    public async Task AtualizarAsync(Exercicio exercicio)
    {
        await GarantirTabelaCriadaAsync();
        await _connectionFactory.Connection.UpdateAsync(ParaRegistro(exercicio));
    }

    public async Task RemoverAsync(Guid id)
    {
        await GarantirTabelaCriadaAsync();
        await _connectionFactory.Connection.DeleteAsync<ExercicioRecord>(id.ToString());
    }

    private Task GarantirTabelaCriadaAsync() =>
        _connectionFactory.Connection.CreateTableAsync<ExercicioRecord>();

    private static Exercicio ParaEntidade(ExercicioRecord registro) =>
        Exercicio.Reconstruir(Guid.Parse(registro.Id), registro.Nome, (GrupoMuscular)registro.GrupoMuscular);

    private static ExercicioRecord ParaRegistro(Exercicio exercicio) => new()
    {
        Id = exercicio.Id.ToString(),
        Nome = exercicio.Nome,
        GrupoMuscular = (int)exercicio.GrupoMuscular
    };
}
