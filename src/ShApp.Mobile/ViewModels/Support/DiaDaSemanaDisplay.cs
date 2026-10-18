namespace ShApp.Mobile.ViewModels.Support;

public static class DiaDaSemanaDisplay
{
    private static readonly Dictionary<DayOfWeek, string> Nomes = new()
    {
        [DayOfWeek.Monday] = "Segunda",
        [DayOfWeek.Tuesday] = "Terça",
        [DayOfWeek.Wednesday] = "Quarta",
        [DayOfWeek.Thursday] = "Quinta",
        [DayOfWeek.Friday] = "Sexta",
        [DayOfWeek.Saturday] = "Sábado",
        [DayOfWeek.Sunday] = "Domingo",
    };

    public static string ObterNome(DayOfWeek diaDaSemana) => Nomes[diaDaSemana];

    public static string FormatarDataCurta(DateOnly data)
    {
        var dt = data.ToDateTime(TimeOnly.MinValue);
        var mes = FormatarMesAbreviado(dt);
        return $"{dt:dd} {mes} {dt:yyyy}";
    }

    public static string FormatarMesAno(DateOnly data)
    {
        var dt = data.ToDateTime(TimeOnly.MinValue);
        return $"{FormatarMesAbreviado(dt)} {dt:yyyy}";
    }

    private static string FormatarMesAbreviado(DateTime dt)
    {
        var mes = dt.ToString("MMM", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")).TrimEnd('.');
        return char.ToUpperInvariant(mes[0]) + mes[1..];
    }
}
