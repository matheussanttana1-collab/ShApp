using System.Globalization;
using ShApp.Core.Entities;
using ShApp.Core.Enums;
using ShApp.Core.Interfaces;
using ShApp.Infrastructure.Data;
using ShApp.Infrastructure.Repositories;
using ShApp.Infrastructure.Seeding;

var caminhoBancoDeDados = args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "shapp_console.db3");
Console.WriteLine($"Banco de dados: {caminhoBancoDeDados}");

var connectionFactory = new AppDbConnectionFactory(caminhoBancoDeDados);
IExercicioRepository exercicioRepository = new ExercicioRepository(connectionFactory);
IDivisaoDeTreinoRepository divisaoDeTreinoRepository = new DivisaoDeTreinoRepository(connectionFactory);
IUsuarioRepository usuarioRepository = new UsuarioRepository(connectionFactory);
IRotinaRepository rotinaRepository = new RotinaRepository(connectionFactory, divisaoDeTreinoRepository, exercicioRepository);
ISessaoDeTreinoRepository sessaoDeTreinoRepository = new SessaoDeTreinoRepository(connectionFactory);

var divisoes = new List<DivisaoDeTreino>();
Usuario? usuarioAtual = null;

while (true)
{
    ExibirMenu();
    var opcao = Console.ReadLine()?.Trim();

    try
    {
        switch (opcao)
        {
            case "1": await CadastrarExercicioAsync(); break;
            case "2": await ListarExerciciosAsync(); break;
            case "3": await CriarDivisaoDeTreinoAsync(); break;
            case "4": ListarDivisoes(); break;
            case "5": await CriarUsuarioAsync(); break;
            case "6": await CriarRotinaAsync(); break;
            case "7": await AdicionarItemRotinaAsync(); break;
            case "8": ConsultarDiaDeTreino(); break;
            case "9": await RegistrarSessaoAsync(); break;
            case "10": VerHistoricoSessoes(); break;
            case "11": await ConsultarUltimoPesoAsync(); break;
            case "12": await SemearDadosPadraoAsync(); break;
            case "0": return;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }

    Console.WriteLine();
}

void ExibirMenu()
{
    Console.WriteLine("==== ShApp - Console de Testes ====");
    Console.WriteLine($"Usuário atual: {usuarioAtual?.Nome ?? "(nenhum)"}");
    Console.WriteLine("1  - Cadastrar exercício");
    Console.WriteLine("2  - Listar exercícios");
    Console.WriteLine("3  - Criar divisão de treino");
    Console.WriteLine("4  - Listar divisões de treino");
    Console.WriteLine("5  - Criar usuário");
    Console.WriteLine("6  - Criar rotina para o usuário atual");
    Console.WriteLine("7  - Adicionar item à rotina (exercício em um dia)");
    Console.WriteLine("8  - Consultar o dia de treino de uma data");
    Console.WriteLine("9  - Registrar sessão de treino (diário)");
    Console.WriteLine("10 - Ver histórico de sessões");
    Console.WriteLine("11 - Consultar último peso utilizado num exercício");
    Console.WriteLine("12 - Semear banco com dados padrão (exercícios + divisões)");
    Console.WriteLine("0  - Sair");
    Console.Write("Escolha uma opção: ");
}

async Task CadastrarExercicioAsync()
{
    Console.Write("Nome do exercício: ");
    var nome = Console.ReadLine() ?? string.Empty;

    var grupoMuscular = EscolherGrupoMuscular();

    var exercicio = new Exercicio(nome, grupoMuscular);
    await exercicioRepository.AdicionarAsync(exercicio);

    Console.WriteLine($"Exercício '{exercicio.Nome}' cadastrado ({exercicio.GrupoMuscular}).");
}

async Task ListarExerciciosAsync()
{
    var exercicios = (await exercicioRepository.ObterTodosAsync()).ToList();

    if (exercicios.Count == 0)
    {
        Console.WriteLine("Nenhum exercício cadastrado.");
        return;
    }

    foreach (var exercicio in exercicios)
        Console.WriteLine($"- {exercicio.Nome} ({exercicio.GrupoMuscular} / {exercicio.GrupoMuscular.ObterGrupoCorporal()})");
}

GrupoMuscular EscolherGrupoMuscular()
{
    var grupos = Enum.GetValues<GrupoMuscular>();

    Console.WriteLine("Grupos musculares disponíveis:");
    for (var i = 0; i < grupos.Length; i++)
        Console.WriteLine($"  {i} - {grupos[i]}");

    Console.Write("Escolha o grupo muscular (número): ");
    var indice = LerInteiro(Console.ReadLine());

    if (indice < 0 || indice >= grupos.Length)
        throw new InvalidOperationException("Grupo muscular inválido.");

    return grupos[indice];
}

List<GrupoMuscular> EscolherGruposMusculares()
{
    var grupos = Enum.GetValues<GrupoMuscular>();

    Console.WriteLine("Grupos musculares disponíveis:");
    for (var i = 0; i < grupos.Length; i++)
        Console.WriteLine($"  {i} - {grupos[i]}");

    Console.Write("Escolha os grupos musculares (números separados por vírgula): ");
    var entrada = Console.ReadLine() ?? string.Empty;

    var selecionados = new List<GrupoMuscular>();
    foreach (var parte in entrada.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
    {
        if (int.TryParse(parte, out var indice) && indice >= 0 && indice < grupos.Length)
            selecionados.Add(grupos[indice]);
    }

    return selecionados;
}

async Task CriarDivisaoDeTreinoAsync()
{
    Console.Write("Nome da divisão (ex.: ABC, Upper/Lower): ");
    var nome = Console.ReadLine() ?? string.Empty;

    var divisao = new DivisaoDeTreino(nome);

    while (true)
    {
        Console.Write("Nome do dia de treino (ex.: A) ou vazio para terminar: ");
        var nomeDia = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(nomeDia))
            break;

        var gruposDoDia = EscolherGruposMusculares();
        divisao.AdicionarDia(new DiaDeTreino(nomeDia, gruposDoDia));

        Console.WriteLine($"Dia '{nomeDia}' adicionado.");
    }

    if (divisao.Dias.Count == 0)
    {
        Console.WriteLine("Divisão descartada: nenhum dia foi adicionado.");
        return;
    }

    divisoes.Add(divisao);
    await divisaoDeTreinoRepository.AdicionarAsync(divisao);
    Console.WriteLine($"Divisão '{divisao.Nome}' criada e salva com {divisao.Dias.Count} dia(s).");
}

void ListarDivisoes()
{
    if (divisoes.Count == 0)
    {
        Console.WriteLine("Nenhuma divisão cadastrada.");
        return;
    }

    for (var i = 0; i < divisoes.Count; i++)
    {
        var divisao = divisoes[i];
        Console.WriteLine($"{i} - {divisao.Nome}");
        foreach (var dia in divisao.Dias)
            Console.WriteLine($"    {dia.Nome}: {string.Join(", ", dia.GruposMusculares)}");
    }
}

async Task SemearDadosPadraoAsync()
{
    var seeder = new DadosPadraoSeeder(exercicioRepository, divisaoDeTreinoRepository);
    var resultado = await seeder.SemearSeNecessarioAsync();

    var idsExistentes = new HashSet<Guid>(divisoes.Select(d => d.Id));
    foreach (var divisao in resultado.Divisoes)
    {
        if (idsExistentes.Add(divisao.Id))
            divisoes.Add(divisao);
    }

    Console.WriteLine("Semeadura concluída.");
    Console.WriteLine($"Exercícios: {resultado.ExerciciosInseridos} inserido(s), {resultado.ExerciciosJaExistiam} já existia(m).");
    Console.WriteLine($"Divisões de treino: {resultado.DivisoesInseridas} inserida(s), {resultado.DivisoesJaExistiam} já existia(m).");
}

async Task CriarUsuarioAsync()
{
    Console.Write("Nome do usuário: ");
    var nome = Console.ReadLine() ?? string.Empty;

    usuarioAtual = new Usuario(nome);
    await usuarioRepository.AdicionarAsync(usuarioAtual);
    Console.WriteLine($"Usuário '{usuarioAtual.Nome}' criado, salvo e selecionado como usuário atual.");
}

DivisaoDeTreino EscolherDivisao()
{
    if (divisoes.Count == 0)
        throw new InvalidOperationException("Nenhuma divisão cadastrada. Crie uma divisão primeiro (opção 3).");

    ListarDivisoes();
    Console.Write("Escolha a divisão (número): ");
    var indice = LerInteiro(Console.ReadLine());

    if (indice < 0 || indice >= divisoes.Count)
        throw new InvalidOperationException("Divisão inválida.");

    return divisoes[indice];
}

async Task CriarRotinaAsync()
{
    GarantirUsuarioSelecionado();

    var divisao = EscolherDivisao();

    Console.Write("Nome da rotina: ");
    var nome = Console.ReadLine() ?? string.Empty;

    Console.Write("Data de início (dd/MM/yyyy, vazio para hoje): ");
    var dataInicio = LerData(Console.ReadLine());

    var diasDaSemana = EscolherDiasDaSemana();

    var rotina = new Rotina(nome, divisao, dataInicio, diasDaSemana);
    usuarioAtual!.AdicionarRotina(rotina);
    await rotinaRepository.AdicionarAsync(usuarioAtual.Id, rotina);

    Console.WriteLine($"Rotina '{rotina.Nome}' criada e salva para {usuarioAtual.Nome} com {diasDaSemana.Count} dia(s) por semana.");
}

List<DayOfWeek> EscolherDiasDaSemana()
{
    var mapa = new Dictionary<string, DayOfWeek>(StringComparer.OrdinalIgnoreCase)
    {
        ["seg"] = DayOfWeek.Monday,
        ["ter"] = DayOfWeek.Tuesday,
        ["qua"] = DayOfWeek.Wednesday,
        ["qui"] = DayOfWeek.Thursday,
        ["sex"] = DayOfWeek.Friday,
        ["sab"] = DayOfWeek.Saturday,
        ["dom"] = DayOfWeek.Sunday,
    };

    Console.WriteLine("Dias da semana: seg, ter, qua, qui, sex, sab, dom");
    Console.Write("Escolha os dias de treino (separados por vírgula, ex.: seg,ter,qua,sex): ");
    var entrada = Console.ReadLine() ?? string.Empty;

    var dias = new List<DayOfWeek>();
    foreach (var parte in entrada.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
    {
        if (mapa.TryGetValue(parte, out var dia))
            dias.Add(dia);
    }

    return dias;
}

void GarantirUsuarioSelecionado()
{
    if (usuarioAtual is null)
        throw new InvalidOperationException("Crie um usuário primeiro (opção 5).");
}

Rotina EscolherRotina()
{
    GarantirUsuarioSelecionado();

    var rotinas = usuarioAtual!.Rotinas.ToList();
    if (rotinas.Count == 0)
        throw new InvalidOperationException("O usuário ainda não tem rotinas. Crie uma rotina primeiro (opção 6).");

    for (var i = 0; i < rotinas.Count; i++)
        Console.WriteLine($"{i} - {rotinas[i].Nome} ({rotinas[i].DivisaoDeTreino.Nome})");

    Console.Write("Escolha a rotina (número): ");
    var indice = LerInteiro(Console.ReadLine());

    if (indice < 0 || indice >= rotinas.Count)
        throw new InvalidOperationException("Rotina inválida.");

    return rotinas[indice];
}

DiaDeTreino EscolherDiaDaDivisao(DivisaoDeTreino divisao)
{
    var dias = divisao.Dias.ToList();
    for (var i = 0; i < dias.Count; i++)
        Console.WriteLine($"{i} - {dias[i].Nome} ({string.Join(", ", dias[i].GruposMusculares)})");

    Console.Write("Escolha o dia (número): ");
    var indice = LerInteiro(Console.ReadLine());

    if (indice < 0 || indice >= dias.Count)
        throw new InvalidOperationException("Dia inválido.");

    return dias[indice];
}

async Task<Exercicio> EscolherExercicioAsync()
{
    var exercicios = (await exercicioRepository.ObterTodosAsync()).ToList();
    if (exercicios.Count == 0)
        throw new InvalidOperationException("Nenhum exercício cadastrado. Cadastre um exercício primeiro (opção 1).");

    for (var i = 0; i < exercicios.Count; i++)
        Console.WriteLine($"{i} - {exercicios[i].Nome} ({exercicios[i].GrupoMuscular})");

    Console.Write("Escolha o exercício (número): ");
    var indice = LerInteiro(Console.ReadLine());

    if (indice < 0 || indice >= exercicios.Count)
        throw new InvalidOperationException("Exercício inválido.");

    return exercicios[indice];
}

async Task AdicionarItemRotinaAsync()
{
    var rotina = EscolherRotina();

    Console.WriteLine("Escolha o dia da divisão para este item:");
    var dia = EscolherDiaDaDivisao(rotina.DivisaoDeTreino);

    var exercicio = await EscolherExercicioAsync();

    Console.Write("Séries alvo: ");
    var series = LerInteiro(Console.ReadLine());

    Console.Write("Repetições alvo: ");
    var repeticoes = LerInteiro(Console.ReadLine());

    Console.Write("Ordem no dia: ");
    var ordem = LerInteiro(Console.ReadLine());

    var item = new ItemDeRotina(dia, exercicio, series, repeticoes, ordem);
    rotina.AdicionarItem(item);
    await rotinaRepository.AdicionarItemAsync(rotina.Id, item);

    Console.WriteLine($"'{exercicio.Nome}' adicionado e salvo no dia '{dia.Nome}' da rotina '{rotina.Nome}'.");
}

void ConsultarDiaDeTreino()
{
    var rotina = EscolherRotina();

    Console.Write("Data (dd/MM/yyyy): ");
    var data = LerData(Console.ReadLine(), permitirVazio: false);

    var dia = rotina.ObterDiaDeTreinoPara(data);
    Console.WriteLine($"Em {data:dd/MM/yyyy} o treino é o dia '{dia.Nome}': {string.Join(", ", dia.GruposMusculares)}.");
}

async Task RegistrarSessaoAsync()
{
    var rotina = EscolherRotina();

    Console.Write("Data da sessão (dd/MM/yyyy, vazio para hoje): ");
    var data = LerData(Console.ReadLine());

    var dia = rotina.ObterDiaDeTreinoPara(data);
    Console.WriteLine($"Dia de treino: '{dia.Nome}' ({string.Join(", ", dia.GruposMusculares)}).");

    var sessao = new SessaoDeTreino(data, dia);

    while (PerguntarSimNao("Adicionar exercício a esta sessão?"))
    {
        var exercicio = await EscolherExercicioAsync();
        var registro = new RegistroExercicio(exercicio);

        while (PerguntarSimNao("Adicionar série?"))
        {
            Console.Write("Peso (kg): ");
            var peso = LerDecimal(Console.ReadLine());

            Console.Write("Repetições: ");
            var repeticoes = LerInteiro(Console.ReadLine());

            registro.AdicionarSerie(new SerieRegistrada(peso, repeticoes));
        }

        sessao.AdicionarRegistro(registro);
    }

    usuarioAtual!.RegistrarSessao(sessao);
    await sessaoDeTreinoRepository.AdicionarAsync(usuarioAtual.Id, sessao);
    Console.WriteLine($"Sessão de {data:dd/MM/yyyy} registrada e salva com {sessao.Registros.Count} exercício(s).");
}

async Task ConsultarUltimoPesoAsync()
{
    GarantirUsuarioSelecionado();

    var exercicio = await EscolherExercicioAsync();
    var peso = await sessaoDeTreinoRepository.ObterUltimoPesoUtilizadoAsync(usuarioAtual!.Id, exercicio.Id);

    Console.WriteLine(peso.HasValue
        ? $"Último peso registrado para '{exercicio.Nome}': {peso.Value} kg."
        : $"Nenhum peso registrado ainda para '{exercicio.Nome}'.");
}

void VerHistoricoSessoes()
{
    GarantirUsuarioSelecionado();

    var sessoes = usuarioAtual!.Sessoes.OrderBy(s => s.Data).ToList();
    if (sessoes.Count == 0)
    {
        Console.WriteLine("Nenhuma sessão registrada.");
        return;
    }

    foreach (var sessao in sessoes)
    {
        Console.WriteLine($"{sessao.Data:dd/MM/yyyy} - Dia '{sessao.DiaDeTreino.Nome}'");
        foreach (var registro in sessao.Registros)
        {
            var series = string.Join(" | ", registro.Series.Select(s => $"{s.Peso}kg x {s.Repeticoes}"));
            Console.WriteLine($"    {registro.Exercicio.Nome}: {series}");
        }
    }
}

bool PerguntarSimNao(string pergunta)
{
    Console.Write($"{pergunta} (s/n): ");
    return string.Equals(Console.ReadLine()?.Trim(), "s", StringComparison.OrdinalIgnoreCase);
}

int LerInteiro(string? texto)
{
    if (!int.TryParse(texto, out var valor))
        throw new InvalidOperationException("Valor numérico inválido.");

    return valor;
}

decimal LerDecimal(string? texto)
{
    var normalizado = (texto ?? string.Empty).Replace(',', '.');
    if (!decimal.TryParse(normalizado, CultureInfo.InvariantCulture, out var valor))
        throw new InvalidOperationException("Valor decimal inválido.");

    return valor;
}

DateOnly LerData(string? texto, bool permitirVazio = true)
{
    if (string.IsNullOrWhiteSpace(texto))
    {
        if (permitirVazio)
            return DateOnly.FromDateTime(DateTime.Today);

        throw new InvalidOperationException("Data obrigatória.");
    }

    if (!DateOnly.TryParseExact(texto, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
        throw new InvalidOperationException("Data inválida. Use o formato dd/MM/yyyy.");

    return data;
}
