using ShApp.Core.Entities;

namespace ShApp.Mobile.ViewModels.Support;

public static class RotacaoHelper
{
    private const int LimiteDiasVarredura = 60;

    /// <summary>
    /// Varre os próximos dias a partir de <paramref name="aPartirDe"/> (inclusive) procurando
    /// dias de treino configurados na rotina, pulando os dias de descanso, até reunir
    /// <paramref name="quantidade"/> ocorrências ou atingir o limite de varredura.
    /// </summary>
    public static IReadOnlyList<(DateOnly Data, DiaDeTreino Dia)> ObterProximasOcorrencias(
        Rotina rotina, DateOnly aPartirDe, int quantidade)
    {
        var resultado = new List<(DateOnly, DiaDeTreino)>();

        var data = aPartirDe < rotina.DataInicio ? rotina.DataInicio : aPartirDe;
        var limite = data.AddDays(LimiteDiasVarredura);

        while (resultado.Count < quantidade && data <= limite)
        {
            if (rotina.DiasDaSemana.Contains(data.DayOfWeek))
                resultado.Add((data, rotina.ObterDiaDeTreinoPara(data)));

            data = data.AddDays(1);
        }

        return resultado;
    }
}
