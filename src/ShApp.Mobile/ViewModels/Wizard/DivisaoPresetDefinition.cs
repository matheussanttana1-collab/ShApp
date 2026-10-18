using ShApp.Core.Enums;

namespace ShApp.Mobile.ViewModels.Wizard;

public class DivisaoPresetDefinition
{
    public required string Chave { get; init; }
    public required string Titulo { get; init; }
    public string? Badge { get; init; }
    public required string Resumo { get; init; }
    public required string SessoesLabel { get; init; }
    public required string IdealLabel { get; init; }
    public required string Detalhe { get; init; }
    public required string NomeSugerido { get; init; }
    public required int MetaSemanalSugerida { get; init; }
    public required IReadOnlyList<(string Letra, GrupoMuscular[] Grupos)> Dias { get; init; }
}

public static class DivisaoPresets
{
    public const string ChaveCustom = "custom";

    public static readonly IReadOnlyList<DivisaoPresetDefinition> Catalogo =
    [
        new DivisaoPresetDefinition
        {
            Chave = "abc",
            Titulo = "ABC (3 Fichas)",
            Badge = "Recomendado",
            Resumo = "Peito & Tríceps • Costas & Bíceps • Pernas Completo",
            SessoesLabel = "3 a 6 sessões/sem",
            IdealLabel = "Ideal: 3 a 5 dias",
            Detalhe = "Volume balanceado com alta frequência por grupo muscular. Permite repetição cíclica (A-B-C-A-B-C).",
            NomeSugerido = "Hipertrofia ABC - Foco Costas",
            MetaSemanalSugerida = 4,
            Dias =
            [
                ("A", [GrupoMuscular.Peito, GrupoMuscular.Triceps]),
                ("B", [GrupoMuscular.Costas, GrupoMuscular.Biceps]),
                ("C", [GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos, GrupoMuscular.Panturrilhas]),
            ],
        },
        new DivisaoPresetDefinition
        {
            Chave = "abcd",
            Titulo = "ABCD (4 Fichas)",
            Resumo = "Peito & Tríceps • Costas • Pernas • Ombros & Bíceps",
            SessoesLabel = "4 sessões semanais",
            IdealLabel = "Foco em Deltoides",
            Detalhe = "Maior isolamento e tempo de regeneração muscular prolongado.",
            NomeSugerido = "Hipertrofia ABCD - Densidade",
            MetaSemanalSugerida = 4,
            Dias =
            [
                ("A", [GrupoMuscular.Peito, GrupoMuscular.Triceps]),
                ("B", [GrupoMuscular.Costas]),
                ("C", [GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos, GrupoMuscular.Panturrilhas]),
                ("D", [GrupoMuscular.Ombros, GrupoMuscular.Biceps]),
            ],
        },
        new DivisaoPresetDefinition
        {
            Chave = "abcde",
            Titulo = "ABCDE (5 Fichas)",
            Resumo = "Peito • Costas • Pernas • Ombros • Braços",
            SessoesLabel = "5 sessões semanais",
            IdealLabel = "Hipertrofia Máxima",
            Detalhe = "Alto volume diário focado. 1 dia inteiro de descanso por grupamento.",
            NomeSugerido = "Hipertrofia ABCDE - Volume Pro",
            MetaSemanalSugerida = 5,
            Dias =
            [
                ("A", [GrupoMuscular.Peito]),
                ("B", [GrupoMuscular.Costas]),
                ("C", [GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos, GrupoMuscular.Panturrilhas]),
                ("D", [GrupoMuscular.Ombros]),
                ("E", [GrupoMuscular.Biceps, GrupoMuscular.Triceps]),
            ],
        },
        new DivisaoPresetDefinition
        {
            Chave = "ul",
            Titulo = "Upper / Lower (2 a 4 Fichas)",
            Resumo = "Superior A & B • Inferior A & B",
            SessoesLabel = "2 a 4 dias semanais",
            IdealLabel = "Poder & Cargas",
            Detalhe = "Excelente para progressão de carga pura em compostos (Agacho, Terra, Supino).",
            NomeSugerido = "Upper / Lower - Força Pura",
            MetaSemanalSugerida = 4,
            Dias =
            [
                ("A", [GrupoMuscular.Peito, GrupoMuscular.Costas, GrupoMuscular.Ombros, GrupoMuscular.Biceps, GrupoMuscular.Triceps]),
                ("B", [GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos]),
                ("C", [GrupoMuscular.Peito, GrupoMuscular.Costas, GrupoMuscular.Ombros, GrupoMuscular.Biceps, GrupoMuscular.Triceps]),
                ("D", [GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos, GrupoMuscular.Panturrilhas]),
            ],
        },
        new DivisaoPresetDefinition
        {
            Chave = "ppl",
            Titulo = "Push / Pull / Legs (PPL)",
            Resumo = "Empurrar (Peito/Ombro/Tríceps) • Puxar • Pernas",
            SessoesLabel = "3 a 6 dias na semana",
            IdealLabel = "Padrões de Movimento",
            Detalhe = "Divisão biomecânica ideal para evitar sobreposição articular prematura.",
            NomeSugerido = "PPL - Push Pull Legs Clássico",
            MetaSemanalSugerida = 5,
            Dias =
            [
                ("A", [GrupoMuscular.Peito, GrupoMuscular.Ombros, GrupoMuscular.Triceps]),
                ("B", [GrupoMuscular.Costas, GrupoMuscular.Biceps]),
                ("C", [GrupoMuscular.Quadriceps, GrupoMuscular.Isquiotibiais, GrupoMuscular.Gluteos, GrupoMuscular.Panturrilhas]),
            ],
        },
    ];
}
