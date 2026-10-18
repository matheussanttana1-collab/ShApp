namespace ShApp.Mobile.ViewModels.Items;

public class CicloSemanalDiaViewModel
{
    public int ColumnIndex { get; }
    public string Abreviacao { get; }
    public string LetraOuOff { get; }
    public bool IsDiaDeTreino { get; }
    public bool IsHoje { get; }

    public CicloSemanalDiaViewModel(int columnIndex, string abreviacao, string? letra, bool isHoje)
    {
        ColumnIndex = columnIndex;
        Abreviacao = abreviacao;
        IsDiaDeTreino = letra is not null;
        LetraOuOff = letra ?? "OFF";
        IsHoje = isHoje;
    }
}
