using ShApp.Core.Entities;

namespace ShApp.Core.Tests;

public class SerieRegistradaTests
{
    [Fact]
    public void Construtor_DeveLancar_QuandoPesoNegativo()
    {
        Assert.Throws<ArgumentException>(() => new SerieRegistrada(-1, 10));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Construtor_DeveLancar_QuandoRepeticoesNaoPositivas(int repeticoes)
    {
        Assert.Throws<ArgumentException>(() => new SerieRegistrada(20, repeticoes));
    }

    [Fact]
    public void Construtor_DevePreencherPropriedades_QuandoValoresValidos()
    {
        var serie = new SerieRegistrada(82.5m, 8);

        Assert.Equal(82.5m, serie.Peso);
        Assert.Equal(8, serie.Repeticoes);
    }
}
