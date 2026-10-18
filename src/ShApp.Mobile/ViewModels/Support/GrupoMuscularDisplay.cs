using ShApp.Core.Enums;

namespace ShApp.Mobile.ViewModels.Support;

public static class GrupoMuscularDisplay
{
    private static readonly Dictionary<GrupoMuscular, string> Nomes = new()
    {
        [GrupoMuscular.Peito] = "Peito",
        [GrupoMuscular.Costas] = "Costas",
        [GrupoMuscular.Ombros] = "Ombros",
        [GrupoMuscular.Biceps] = "Bíceps",
        [GrupoMuscular.Triceps] = "Tríceps",
        [GrupoMuscular.Abdomen] = "Abdômen",
        [GrupoMuscular.Quadriceps] = "Quadríceps",
        [GrupoMuscular.Isquiotibiais] = "Isquiotibiais",
        [GrupoMuscular.Gluteos] = "Glúteos",
        [GrupoMuscular.Panturrilhas] = "Panturrilhas",
        [GrupoMuscular.Lombar] = "Lombar",
    };

    public static string ObterNome(GrupoMuscular grupoMuscular) =>
        Nomes.TryGetValue(grupoMuscular, out var nome) ? nome : grupoMuscular.ToString();

    public static string ObterNomes(IEnumerable<GrupoMuscular> gruposMusculares) =>
        string.Join(" & ", gruposMusculares.Select(ObterNome));
}
