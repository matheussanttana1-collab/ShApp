namespace ShApp.Core.Entities;

public class SerieRegistrada
{
    public decimal Peso { get; }
    public int Repeticoes { get; }

    public SerieRegistrada(decimal peso, int repeticoes)
    {
        if (peso < 0)
            throw new ArgumentException("O peso não pode ser negativo.", nameof(peso));

        if (repeticoes <= 0)
            throw new ArgumentException("As repetições devem ser maiores que zero.", nameof(repeticoes));

        Peso = peso;
        Repeticoes = repeticoes;
    }
}
