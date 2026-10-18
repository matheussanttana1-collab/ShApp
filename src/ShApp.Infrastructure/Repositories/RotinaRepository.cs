using ShApp.Core.Entities;
using ShApp.Core.Interfaces;
using ShApp.Infrastructure.Data;
using ShApp.Infrastructure.Persistence;

namespace ShApp.Infrastructure.Repositories;

public class RotinaRepository : IRotinaRepository
{
    private readonly AppDbConnectionFactory _connectionFactory;
    private readonly IDivisaoDeTreinoRepository _divisaoDeTreinoRepository;
    private readonly IExercicioRepository _exercicioRepository;

    public RotinaRepository(
        AppDbConnectionFactory connectionFactory,
        IDivisaoDeTreinoRepository divisaoDeTreinoRepository,
        IExercicioRepository exercicioRepository)
    {
        _connectionFactory = connectionFactory;
        _divisaoDeTreinoRepository = divisaoDeTreinoRepository;
        _exercicioRepository = exercicioRepository;
    }

    public async Task<Rotina?> ObterAtivaPorUsuarioAsync(Guid usuarioId)
    {
        await GarantirTabelaCriadaAsync();

        var usuarioIdTexto = usuarioId.ToString();
        var registros = await _connectionFactory.Connection.Table<RotinaRecord>()
            .Where(r => r.UsuarioId == usuarioIdTexto)
            .ToListAsync();

        var maisRecente = registros.OrderByDescending(r => r.DataInicio).FirstOrDefault();
        return maisRecente is null ? null : await HidratarAsync(maisRecente);
    }

    public async Task<IEnumerable<Rotina>> ObterPorUsuarioAsync(Guid usuarioId)
    {
        await GarantirTabelaCriadaAsync();

        var usuarioIdTexto = usuarioId.ToString();
        var registros = await _connectionFactory.Connection.Table<RotinaRecord>()
            .Where(r => r.UsuarioId == usuarioIdTexto)
            .ToListAsync();

        var rotinas = new List<Rotina>();
        foreach (var registro in registros)
            rotinas.Add(await HidratarAsync(registro));

        return rotinas;
    }

    public async Task<Rotina?> ObterPorIdAsync(Guid id)
    {
        await GarantirTabelaCriadaAsync();

        var idTexto = id.ToString();
        var registro = await _connectionFactory.Connection.Table<RotinaRecord>()
            .Where(r => r.Id == idTexto)
            .FirstOrDefaultAsync();

        return registro is null ? null : await HidratarAsync(registro);
    }

    public async Task AdicionarAsync(Guid usuarioId, Rotina rotina)
    {
        await GarantirTabelaCriadaAsync();

        await _connectionFactory.Connection.InsertAsync(new RotinaRecord
        {
            Id = rotina.Id.ToString(),
            UsuarioId = usuarioId.ToString(),
            Nome = rotina.Nome,
            DivisaoDeTreinoId = rotina.DivisaoDeTreino.Id.ToString(),
            DataInicio = rotina.DataInicio.ToDateTime(TimeOnly.MinValue)
        });

        foreach (var diaDaSemana in rotina.DiasDaSemana)
        {
            await _connectionFactory.Connection.InsertAsync(new RotinaDiaDaSemanaRecord
            {
                Id = Guid.NewGuid().ToString(),
                RotinaId = rotina.Id.ToString(),
                DiaDaSemana = (int)diaDaSemana
            });
        }

        foreach (var item in rotina.Itens)
            await InserirItemAsync(rotina.Id, item);
    }

    public async Task AdicionarItemAsync(Guid rotinaId, ItemDeRotina item)
    {
        await GarantirTabelaCriadaAsync();
        await InserirItemAsync(rotinaId, item);
    }

    private async Task InserirItemAsync(Guid rotinaId, ItemDeRotina item)
    {
        await _connectionFactory.Connection.InsertAsync(new ItemDeRotinaRecord
        {
            Id = item.Id.ToString(),
            RotinaId = rotinaId.ToString(),
            DiaDeTreinoId = item.DiaDeTreino.Id.ToString(),
            ExercicioId = item.Exercicio.Id.ToString(),
            SeriesAlvo = item.SeriesAlvo,
            RepeticoesAlvo = item.RepeticoesAlvo,
            Ordem = item.Ordem
        });
    }

    private async Task<Rotina> HidratarAsync(RotinaRecord registro)
    {
        var divisao = await _divisaoDeTreinoRepository.ObterPorIdAsync(Guid.Parse(registro.DivisaoDeTreinoId))
            ?? throw new InvalidOperationException($"Divisão de treino {registro.DivisaoDeTreinoId} não encontrada para a rotina {registro.Id}.");

        var diasDaSemanaRegistros = await _connectionFactory.Connection.Table<RotinaDiaDaSemanaRecord>()
            .Where(r => r.RotinaId == registro.Id)
            .ToListAsync();
        var diasDaSemana = diasDaSemanaRegistros.Select(r => (DayOfWeek)r.DiaDaSemana);

        var rotina = Rotina.Reconstruir(
            Guid.Parse(registro.Id),
            registro.Nome,
            divisao,
            DateOnly.FromDateTime(registro.DataInicio),
            diasDaSemana);

        var itensRegistros = await _connectionFactory.Connection.Table<ItemDeRotinaRecord>()
            .Where(r => r.RotinaId == registro.Id)
            .ToListAsync();

        foreach (var itemRegistro in itensRegistros.OrderBy(r => r.Ordem))
        {
            var diaDeTreinoId = Guid.Parse(itemRegistro.DiaDeTreinoId);
            var diaDeTreino = divisao.Dias.First(d => d.Id == diaDeTreinoId);

            var exercicio = await _exercicioRepository.ObterPorIdAsync(Guid.Parse(itemRegistro.ExercicioId))
                ?? throw new InvalidOperationException($"Exercício {itemRegistro.ExercicioId} não encontrado para o item {itemRegistro.Id}.");

            var item = ItemDeRotina.Reconstruir(
                Guid.Parse(itemRegistro.Id),
                diaDeTreino,
                exercicio,
                itemRegistro.SeriesAlvo,
                itemRegistro.RepeticoesAlvo,
                itemRegistro.Ordem);

            rotina.AdicionarItem(item);
        }

        return rotina;
    }

    private async Task GarantirTabelaCriadaAsync()
    {
        await _connectionFactory.Connection.CreateTableAsync<RotinaRecord>();
        await _connectionFactory.Connection.CreateTableAsync<RotinaDiaDaSemanaRecord>();
        await _connectionFactory.Connection.CreateTableAsync<ItemDeRotinaRecord>();
    }
}
