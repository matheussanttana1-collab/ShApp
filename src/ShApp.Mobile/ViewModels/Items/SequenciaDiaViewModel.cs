namespace ShApp.Mobile.ViewModels.Items;

public class SequenciaDiaViewModel
{
    public int ColumnIndex { get; }
    public string Letra { get; }
    public string NomeGrupo { get; }
    public string EstadoLabel { get; }
    public string RelativoLabel { get; }
    public bool IsHoje { get; }
    public bool IsProximo { get; }
    public bool IsASeguir { get; }

    public SequenciaDiaViewModel(string letra, string nomeGrupo, DateOnly data, DateOnly hoje, int posicao)
    {
        ColumnIndex = posicao;
        Letra = letra;
        NomeGrupo = nomeGrupo;

        IsHoje = posicao == 0;
        IsProximo = posicao == 1;
        IsASeguir = posicao >= 2;

        EstadoLabel = posicao switch
        {
            0 => "HOJE",
            1 => "PRÓXIMO",
            _ => "A SEGUIR",
        };

        var diasDeDiferenca = data.DayNumber - hoje.DayNumber;
        RelativoLabel = diasDeDiferenca switch
        {
            0 => "Ativo",
            1 => "Amanhã",
            _ => data.ToString("ddd", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
        };
    }
}
