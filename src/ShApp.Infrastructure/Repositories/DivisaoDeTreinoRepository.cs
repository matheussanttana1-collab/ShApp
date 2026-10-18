using ShApp.Core.Entities;
using ShApp.Core.Enums;
using ShApp.Core.Interfaces;
using ShApp.Infrastructure.Data;
using ShApp.Infrastructure.Persistence;

namespace ShApp.Infrastructure.Repositories;

public class DivisaoDeTreinoRepository : IDivisaoDeTreinoRepository
{
    private readonly AppDbConnectionFactory _connectionFactory;

    public DivisaoDeTreinoRepository(AppDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DivisaoDeTreino?> ObterPorIdAsync(Guid id)
    {
        await GarantirTabelaCriadaAsync();

        var idTexto = id.ToString();
        var registro = await _connectionFactory.Connection.Table<DivisaoDeTreinoRecord>()
            .Where(r => r.Id == idTexto)
            .FirstOrDefaultAsync();

        return registro is null ? null : await HidratarAsync(registro);
    }

    public async Task<IEnumerable<DivisaoDeTreino>> ObterTodasAsync()
    {
        await GarantirTabelaCriadaAsync();

        var registros = await _connectionFactory.Connection.Table<DivisaoDeTreinoRecord>().ToListAsync();

        var divisoes = new List<DivisaoDeTreino>();
        foreach (var registro in registros)
            divisoes.Add(await HidratarAsync(registro));

        return divisoes;
    }

    public async Task AdicionarAsync(DivisaoDeTreino divisaoDeTreino)
    {
        await GarantirTabelaCriadaAsync();

        await _connectionFactory.Connection.InsertAsync(new DivisaoDeTreinoRecord
        {
            Id = divisaoDeTreino.Id.ToString(),
            Nome = divisaoDeTreino.Nome
        });

        var ordem = 0;
        foreach (var dia in divisaoDeTreino.Dias)
        {
            await _connectionFactory.Connection.InsertAsync(new DiaDeTreinoRecord
            {
                Id = dia.Id.ToString(),
                DivisaoDeTreinoId = divisaoDeTreino.Id.ToString(),
                Nome = dia.Nome,
                Ordem = ordem
            });

            foreach (var grupoMuscular in dia.GruposMusculares)
            {
                await _connectionFactory.Connection.InsertAsync(new DiaDeTreinoGrupoMuscularRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    DiaDeTreinoId = dia.Id.ToString(),
                    GrupoMuscular = (int)grupoMuscular
                });
            }

            ordem++;
        }
    }

    private async Task<DivisaoDeTreino> HidratarAsync(DivisaoDeTreinoRecord registro)
    {
        var divisao = DivisaoDeTreino.Reconstruir(Guid.Parse(registro.Id), registro.Nome);

        var diasRegistros = await _connectionFactory.Connection.Table<DiaDeTreinoRecord>()
            .Where(r => r.DivisaoDeTreinoId == registro.Id)
            .ToListAsync();

        foreach (var diaRegistro in diasRegistros.OrderBy(r => r.Ordem))
        {
            var gruposRegistros = await _connectionFactory.Connection.Table<DiaDeTreinoGrupoMuscularRecord>()
                .Where(r => r.DiaDeTreinoId == diaRegistro.Id)
                .ToListAsync();

            var grupos = gruposRegistros.Select(g => (GrupoMuscular)g.GrupoMuscular);
            var dia = DiaDeTreino.Reconstruir(Guid.Parse(diaRegistro.Id), diaRegistro.Nome, grupos);
            divisao.AdicionarDia(dia);
        }

        return divisao;
    }

    private async Task GarantirTabelaCriadaAsync()
    {
        await _connectionFactory.Connection.CreateTableAsync<DivisaoDeTreinoRecord>();
        await _connectionFactory.Connection.CreateTableAsync<DiaDeTreinoRecord>();
        await _connectionFactory.Connection.CreateTableAsync<DiaDeTreinoGrupoMuscularRecord>();
    }
}
