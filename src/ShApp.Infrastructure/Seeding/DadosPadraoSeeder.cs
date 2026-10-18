using ShApp.Core.Entities;
using ShApp.Core.Enums;
using ShApp.Core.Interfaces;

namespace ShApp.Infrastructure.Seeding;

/// <summary>
/// Resultado da semeadura de dados padrão: quantos itens foram inseridos, quantos já
/// existiam, e a lista completa das divisões de treino padrão (inseridas agora ou já
/// existentes), para quem chamou poder usá-las imediatamente.
/// </summary>
public record ResultadoSemeadura(
    int ExerciciosInseridos,
    int ExerciciosJaExistiam,
    int DivisoesInseridas,
    int DivisoesJaExistiam,
    IReadOnlyList<DivisaoDeTreino> Divisoes);

/// <summary>
/// Semeia o banco de dados com o catálogo padrão de exercícios e divisões de treino do
/// ShApp (81 exercícios e 6 divisões), de forma idempotente: só insere o que ainda não
/// existir, comparando por <c>Nome</c> sem diferenciar maiúsculas/minúsculas.
/// Usado tanto pelo ConsoleTester quanto pelo app Mobile no primeiro start.
/// </summary>
public class DadosPadraoSeeder
{
    private readonly IExercicioRepository _exercicioRepository;
    private readonly IDivisaoDeTreinoRepository _divisaoDeTreinoRepository;

    public DadosPadraoSeeder(
        IExercicioRepository exercicioRepository,
        IDivisaoDeTreinoRepository divisaoDeTreinoRepository)
    {
        _exercicioRepository = exercicioRepository;
        _divisaoDeTreinoRepository = divisaoDeTreinoRepository;
    }

    public async Task<ResultadoSemeadura> SemearSeNecessarioAsync()
    {
        var exerciciosPadrao = new List<(string Nome, GrupoMuscular Grupo)>
        {
            ("Supino reto com barra", GrupoMuscular.Peito),
            ("Supino reto com halteres", GrupoMuscular.Peito),
            ("Supino inclinado com barra", GrupoMuscular.Peito),
            ("Supino inclinado com halteres", GrupoMuscular.Peito),
            ("Supino declinado", GrupoMuscular.Peito),
            ("Supino máquina", GrupoMuscular.Peito),
            ("Crucifixo com halteres", GrupoMuscular.Peito),
            ("Crossover", GrupoMuscular.Peito),
            ("Voador (peck deck)", GrupoMuscular.Peito),
            ("Flexão de braço", GrupoMuscular.Peito),
            ("Mergulho nas paralelas", GrupoMuscular.Peito),

            ("Puxada frontal", GrupoMuscular.Costas),
            ("Barra fixa pronada", GrupoMuscular.Costas),
            ("Barra fixa supinada", GrupoMuscular.Costas),
            ("Remada curvada com barra", GrupoMuscular.Costas),
            ("Remada baixa sentada", GrupoMuscular.Costas),
            ("Remada unilateral (serrote)", GrupoMuscular.Costas),
            ("Remada cavalinho", GrupoMuscular.Costas),
            ("Remada máquina", GrupoMuscular.Costas),
            ("Pulldown braços estendidos", GrupoMuscular.Costas),
            ("Pullover", GrupoMuscular.Costas),
            ("Levantamento terra", GrupoMuscular.Costas),
            ("Encolhimento", GrupoMuscular.Costas),

            ("Desenvolvimento com halteres", GrupoMuscular.Ombros),
            ("Desenvolvimento com barra", GrupoMuscular.Ombros),
            ("Desenvolvimento máquina", GrupoMuscular.Ombros),
            ("Elevação lateral", GrupoMuscular.Ombros),
            ("Elevação frontal", GrupoMuscular.Ombros),
            ("Crucifixo inverso", GrupoMuscular.Ombros),
            ("Face pull", GrupoMuscular.Ombros),
            ("Remada alta", GrupoMuscular.Ombros),

            ("Rosca direta", GrupoMuscular.Biceps),
            ("Rosca alternada", GrupoMuscular.Biceps),
            ("Rosca martelo", GrupoMuscular.Biceps),
            ("Rosca Scott", GrupoMuscular.Biceps),
            ("Rosca concentrada", GrupoMuscular.Biceps),
            ("Rosca na polia", GrupoMuscular.Biceps),
            ("Rosca inversa", GrupoMuscular.Biceps),
            ("Rosca de punho", GrupoMuscular.Biceps),

            ("Tríceps na polia", GrupoMuscular.Triceps),
            ("Tríceps testa", GrupoMuscular.Triceps),
            ("Tríceps francês", GrupoMuscular.Triceps),
            ("Tríceps coice", GrupoMuscular.Triceps),
            ("Supino fechado", GrupoMuscular.Triceps),
            ("Mergulho no banco", GrupoMuscular.Triceps),

            ("Prancha", GrupoMuscular.Abdomen),
            ("Prancha lateral", GrupoMuscular.Abdomen),
            ("Abdominal supra", GrupoMuscular.Abdomen),
            ("Abdominal na polia", GrupoMuscular.Abdomen),
            ("Elevação de pernas", GrupoMuscular.Abdomen),
            ("Roda abdominal", GrupoMuscular.Abdomen),
            ("Pallof press", GrupoMuscular.Abdomen),
            ("Dead bug", GrupoMuscular.Abdomen),
            ("Caminhada do fazendeiro", GrupoMuscular.Abdomen),

            ("Agachamento livre", GrupoMuscular.Quadriceps),
            ("Agachamento frontal", GrupoMuscular.Quadriceps),
            ("Agachamento goblet", GrupoMuscular.Quadriceps),
            ("Agachamento no Smith", GrupoMuscular.Quadriceps),
            ("Leg press 45°", GrupoMuscular.Quadriceps),
            ("Hack", GrupoMuscular.Quadriceps),
            ("Cadeira extensora", GrupoMuscular.Quadriceps),
            ("Afundo (passada)", GrupoMuscular.Quadriceps),
            ("Agachamento búlgaro", GrupoMuscular.Quadriceps),
            ("Subida no banco", GrupoMuscular.Quadriceps),

            ("Stiff", GrupoMuscular.Isquiotibiais),
            ("Levantamento terra romeno (RDL)", GrupoMuscular.Isquiotibiais),
            ("Mesa flexora", GrupoMuscular.Isquiotibiais),
            ("Cadeira flexora", GrupoMuscular.Isquiotibiais),
            ("Flexora em pé", GrupoMuscular.Isquiotibiais),
            ("Flexão nórdica", GrupoMuscular.Isquiotibiais),

            ("Elevação pélvica (hip thrust)", GrupoMuscular.Gluteos),
            ("Ponte de glúteo", GrupoMuscular.Gluteos),
            ("Coice na polia", GrupoMuscular.Gluteos),
            ("Cadeira abdutora", GrupoMuscular.Gluteos),
            ("Cadeira adutora", GrupoMuscular.Gluteos),
            ("Abdução com elástico", GrupoMuscular.Gluteos),

            ("Panturrilha em pé", GrupoMuscular.Panturrilhas),
            ("Panturrilha sentado", GrupoMuscular.Panturrilhas),
            ("Panturrilha no leg press", GrupoMuscular.Panturrilhas),

            ("Hiperextensão lombar", GrupoMuscular.Lombar),
            ("Bom dia", GrupoMuscular.Lombar),
        };

        var divisoesPadrao = new List<(string Nome, List<(string Dia, GrupoMuscular[] Grupos)> Dias)>
        {
            ("Full Body", new List<(string Dia, GrupoMuscular[] Grupos)>
            {
                ("A", new[] { GrupoMuscular.Peito, GrupoMuscular.Costas, GrupoMuscular.Ombros, GrupoMuscular.Biceps, GrupoMuscular.Triceps, GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos, GrupoMuscular.Abdomen }),
                ("B", new[] { GrupoMuscular.Peito, GrupoMuscular.Costas, GrupoMuscular.Ombros, GrupoMuscular.Biceps, GrupoMuscular.Triceps, GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos, GrupoMuscular.Abdomen }),
            }),
            ("Upper/Lower", new List<(string Dia, GrupoMuscular[] Grupos)>
            {
                ("A", new[] { GrupoMuscular.Peito, GrupoMuscular.Costas, GrupoMuscular.Ombros, GrupoMuscular.Biceps, GrupoMuscular.Triceps, GrupoMuscular.Abdomen }),
                ("B", new[] { GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos, GrupoMuscular.Panturrilhas, GrupoMuscular.Lombar, GrupoMuscular.Abdomen }),
            }),
            ("Push/Pull/Legs", new List<(string Dia, GrupoMuscular[] Grupos)>
            {
                ("A", new[] { GrupoMuscular.Peito, GrupoMuscular.Ombros, GrupoMuscular.Triceps }),
                ("B", new[] { GrupoMuscular.Costas, GrupoMuscular.Biceps, GrupoMuscular.Lombar }),
                ("C", new[] { GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos, GrupoMuscular.Panturrilhas, GrupoMuscular.Abdomen }),
            }),
            ("ABC Clássico", new List<(string Dia, GrupoMuscular[] Grupos)>
            {
                ("A", new[] { GrupoMuscular.Peito, GrupoMuscular.Triceps }),
                ("B", new[] { GrupoMuscular.Costas, GrupoMuscular.Biceps, GrupoMuscular.Lombar }),
                ("C", new[] { GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos, GrupoMuscular.Panturrilhas, GrupoMuscular.Ombros, GrupoMuscular.Abdomen }),
            }),
            ("ABCD", new List<(string Dia, GrupoMuscular[] Grupos)>
            {
                ("A", new[] { GrupoMuscular.Peito, GrupoMuscular.Triceps }),
                ("B", new[] { GrupoMuscular.Costas, GrupoMuscular.Biceps }),
                ("C", new[] { GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos, GrupoMuscular.Panturrilhas }),
                ("D", new[] { GrupoMuscular.Ombros, GrupoMuscular.Abdomen, GrupoMuscular.Lombar }),
            }),
            ("ABCDE", new List<(string Dia, GrupoMuscular[] Grupos)>
            {
                ("A", new[] { GrupoMuscular.Peito }),
                ("B", new[] { GrupoMuscular.Costas, GrupoMuscular.Lombar }),
                ("C", new[] { GrupoMuscular.Ombros }),
                ("D", new[] { GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos, GrupoMuscular.Panturrilhas }),
                ("E", new[] { GrupoMuscular.Biceps, GrupoMuscular.Triceps, GrupoMuscular.Abdomen }),
            }),
        };

        var exerciciosExistentes = (await _exercicioRepository.ObterTodosAsync()).ToList();
        var nomesExercicioExistentes = new HashSet<string>(
            exerciciosExistentes.Select(e => e.Nome),
            StringComparer.OrdinalIgnoreCase);

        var divisoesExistentes = (await _divisaoDeTreinoRepository.ObterTodasAsync()).ToList();
        var divisoesExistentesPorNome = new Dictionary<string, DivisaoDeTreino>(
            StringComparer.OrdinalIgnoreCase);
        foreach (var divisaoExistente in divisoesExistentes)
            divisoesExistentesPorNome[divisaoExistente.Nome] = divisaoExistente;

        var exerciciosInseridos = 0;
        var exerciciosJaExistiam = 0;

        foreach (var (nome, grupo) in exerciciosPadrao)
        {
            if (nomesExercicioExistentes.Contains(nome))
            {
                exerciciosJaExistiam++;
                continue;
            }

            var exercicio = new Exercicio(nome, grupo);
            await _exercicioRepository.AdicionarAsync(exercicio);
            nomesExercicioExistentes.Add(nome);
            exerciciosInseridos++;
        }

        var divisoesInseridas = 0;
        var divisoesJaExistiam = 0;
        var divisoesResultado = new List<DivisaoDeTreino>();

        foreach (var (nomeDivisao, dias) in divisoesPadrao)
        {
            if (divisoesExistentesPorNome.TryGetValue(nomeDivisao, out var divisaoExistente))
            {
                divisoesJaExistiam++;
                divisoesResultado.Add(divisaoExistente);
                continue;
            }

            var divisao = new DivisaoDeTreino(nomeDivisao);
            foreach (var (nomeDia, grupos) in dias)
                divisao.AdicionarDia(new DiaDeTreino(nomeDia, grupos));

            await _divisaoDeTreinoRepository.AdicionarAsync(divisao);
            divisoesResultado.Add(divisao);
            divisoesExistentesPorNome[nomeDivisao] = divisao;
            divisoesInseridas++;
        }

        return new ResultadoSemeadura(
            exerciciosInseridos,
            exerciciosJaExistiam,
            divisoesInseridas,
            divisoesJaExistiam,
            divisoesResultado);
    }
}
