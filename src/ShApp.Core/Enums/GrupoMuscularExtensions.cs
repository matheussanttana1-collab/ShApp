namespace ShApp.Core.Enums;

public static class GrupoMuscularExtensions
{
    public static GrupoCorporal ObterGrupoCorporal(this GrupoMuscular grupoMuscular) => grupoMuscular switch
    {
        GrupoMuscular.Peito or
        GrupoMuscular.Costas or
        GrupoMuscular.Ombros or
        GrupoMuscular.Biceps or
        GrupoMuscular.Triceps or
        GrupoMuscular.Abdomen => GrupoCorporal.MembrosSuperiores,

        GrupoMuscular.Quadriceps or
        GrupoMuscular.Isquiotibiais or
        GrupoMuscular.Gluteos or
        GrupoMuscular.Panturrilhas or
        GrupoMuscular.Lombar => GrupoCorporal.MembrosInferiores,

        _ => throw new ArgumentOutOfRangeException(nameof(grupoMuscular))
    };
}
