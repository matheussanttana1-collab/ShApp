namespace ShApp.Core.Entities;

public class ItemDeRotina
{
    public Guid Id { get; }
    public DiaDeTreino DiaDeTreino { get; }
    public Exercicio Exercicio { get; }
    public int SeriesAlvo { get; }
    public int RepeticoesAlvo { get; }
    public int Ordem { get; }

    public ItemDeRotina(DiaDeTreino diaDeTreino, Exercicio exercicio, int seriesAlvo, int repeticoesAlvo, int ordem)
    {
        ArgumentNullException.ThrowIfNull(diaDeTreino);
        ArgumentNullException.ThrowIfNull(exercicio);

        if (seriesAlvo <= 0)
            throw new ArgumentException("As séries alvo devem ser maiores que zero.", nameof(seriesAlvo));

        if (repeticoesAlvo <= 0)
            throw new ArgumentException("As repetições alvo devem ser maiores que zero.", nameof(repeticoesAlvo));

        Id = Guid.NewGuid();
        DiaDeTreino = diaDeTreino;
        Exercicio = exercicio;
        SeriesAlvo = seriesAlvo;
        RepeticoesAlvo = repeticoesAlvo;
        Ordem = ordem;
    }

    private ItemDeRotina(Guid id, DiaDeTreino diaDeTreino, Exercicio exercicio, int seriesAlvo, int repeticoesAlvo, int ordem)
    {
        Id = id;
        DiaDeTreino = diaDeTreino;
        Exercicio = exercicio;
        SeriesAlvo = seriesAlvo;
        RepeticoesAlvo = repeticoesAlvo;
        Ordem = ordem;
    }

    public static ItemDeRotina Reconstruir(Guid id, DiaDeTreino diaDeTreino, Exercicio exercicio, int seriesAlvo, int repeticoesAlvo, int ordem) =>
        new(id, diaDeTreino, exercicio, seriesAlvo, repeticoesAlvo, ordem);
}
