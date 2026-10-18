using ShApp.Core.Entities;
using ShApp.Core.Interfaces;
using ShApp.Infrastructure.Data;
using ShApp.Infrastructure.Persistence;

namespace ShApp.Infrastructure.Repositories;

public class SessaoDeTreinoRepository : ISessaoDeTreinoRepository
{
    private readonly AppDbConnectionFactory _connectionFactory;

    public SessaoDeTreinoRepository(AppDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AdicionarAsync(Guid usuarioId, SessaoDeTreino sessao)
    {
        await GarantirTabelaCriadaAsync();

        await _connectionFactory.Connection.RunInTransactionAsync(db =>
        {
            db.Insert(new SessaoDeTreinoRecord
            {
                Id = sessao.Id.ToString(),
                UsuarioId = usuarioId.ToString(),
                DiaDeTreinoId = sessao.DiaDeTreino.Id.ToString(),
                Data = sessao.Data.ToDateTime(TimeOnly.MinValue)
            });

            foreach (var registro in sessao.Registros)
            {
                db.Insert(new RegistroExercicioRecord
                {
                    Id = registro.Id.ToString(),
                    SessaoDeTreinoId = sessao.Id.ToString(),
                    ExercicioId = registro.Exercicio.Id.ToString()
                });

                var ordem = 0;
                foreach (var serie in registro.Series)
                {
                    db.Insert(new SerieRegistradaRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        RegistroExercicioId = registro.Id.ToString(),
                        Peso = serie.Peso,
                        Repeticoes = serie.Repeticoes,
                        Ordem = ordem
                    });
                    ordem++;
                }
            }
        });
    }

    public async Task<decimal?> ObterUltimoPesoUtilizadoAsync(Guid usuarioId, Guid exercicioId)
    {
        await GarantirTabelaCriadaAsync();

        var usuarioIdTexto = usuarioId.ToString();
        var exercicioIdTexto = exercicioId.ToString();

        var sessoes = await _connectionFactory.Connection.Table<SessaoDeTreinoRecord>()
            .Where(s => s.UsuarioId == usuarioIdTexto)
            .ToListAsync();

        var sessoesOrdenadas = sessoes.OrderByDescending(s => s.Data).ToList();

        foreach (var sessao in sessoesOrdenadas)
        {
            var registro = await _connectionFactory.Connection.Table<RegistroExercicioRecord>()
                .Where(r => r.SessaoDeTreinoId == sessao.Id && r.ExercicioId == exercicioIdTexto)
                .FirstOrDefaultAsync();

            if (registro is null)
                continue;

            var series = await _connectionFactory.Connection.Table<SerieRegistradaRecord>()
                .Where(s => s.RegistroExercicioId == registro.Id)
                .ToListAsync();

            if (series.Count == 0)
                continue;

            return series.OrderByDescending(s => s.Ordem).First().Peso;
        }

        return null;
    }

    private async Task GarantirTabelaCriadaAsync()
    {
        await _connectionFactory.Connection.CreateTableAsync<SessaoDeTreinoRecord>();
        await _connectionFactory.Connection.CreateTableAsync<RegistroExercicioRecord>();
        await _connectionFactory.Connection.CreateTableAsync<SerieRegistradaRecord>();
    }
}
