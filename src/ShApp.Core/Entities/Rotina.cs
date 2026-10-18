namespace ShApp.Core.Entities;

public class Rotina
{
    private readonly HashSet<DayOfWeek> _diasDaSemana;
    private readonly List<ItemDeRotina> _itens = new();

    public Guid Id { get; }
    public string Nome { get; }
    public DivisaoDeTreino DivisaoDeTreino { get; }
    public DateOnly DataInicio { get; }
    public IReadOnlyCollection<DayOfWeek> DiasDaSemana => _diasDaSemana;
    public IReadOnlyCollection<ItemDeRotina> Itens => _itens;

    public Rotina(string nome, DivisaoDeTreino divisaoDeTreino, DateOnly dataInicio, IEnumerable<DayOfWeek> diasDaSemana)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome da rotina é obrigatório.", nameof(nome));

        ArgumentNullException.ThrowIfNull(divisaoDeTreino);

        _diasDaSemana = new HashSet<DayOfWeek>(diasDaSemana ?? Enumerable.Empty<DayOfWeek>());
        if (_diasDaSemana.Count == 0)
            throw new ArgumentException("A rotina deve ter ao menos um dia da semana para treinar.", nameof(diasDaSemana));

        Id = Guid.NewGuid();
        Nome = nome;
        DivisaoDeTreino = divisaoDeTreino;
        DataInicio = dataInicio;
    }

    private Rotina(Guid id, string nome, DivisaoDeTreino divisaoDeTreino, DateOnly dataInicio, IEnumerable<DayOfWeek> diasDaSemana)
    {
        Id = id;
        Nome = nome;
        DivisaoDeTreino = divisaoDeTreino;
        DataInicio = dataInicio;
        _diasDaSemana = new HashSet<DayOfWeek>(diasDaSemana);
    }

    public static Rotina Reconstruir(Guid id, string nome, DivisaoDeTreino divisaoDeTreino, DateOnly dataInicio, IEnumerable<DayOfWeek> diasDaSemana) =>
        new(id, nome, divisaoDeTreino, dataInicio, diasDaSemana);

    public void AdicionarItem(ItemDeRotina item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (!DivisaoDeTreino.Dias.Contains(item.DiaDeTreino))
            throw new ArgumentException("O dia do item não pertence à divisão de treino desta rotina.", nameof(item));

        _itens.Add(item);
    }

    // A rotação não reinicia por semana: continua de onde parou (ex.: ABC 5x/semana termina em A na sexta e retoma em B na segunda seguinte).
    public DiaDeTreino ObterDiaDeTreinoPara(DateOnly data)
    {
        if (data < DataInicio)
            throw new ArgumentException("A data não pode ser anterior ao início da rotina.", nameof(data));

        if (!_diasDaSemana.Contains(data.DayOfWeek))
            throw new ArgumentException("A data informada não é um dia de treino configurado nesta rotina.", nameof(data));

        var diasDaDivisao = DivisaoDeTreino.Dias.ToList();

        var ocorrenciasAntes = 0;
        for (var dia = DataInicio; dia < data; dia = dia.AddDays(1))
        {
            if (_diasDaSemana.Contains(dia.DayOfWeek))
                ocorrenciasAntes++;
        }

        var indice = ocorrenciasAntes % diasDaDivisao.Count;
        return diasDaDivisao[indice];
    }
}
