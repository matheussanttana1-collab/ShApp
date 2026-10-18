namespace ShApp.Core.Entities;

public class Usuario
{
    private readonly List<Rotina> _rotinas = new();
    private readonly List<SessaoDeTreino> _sessoes = new();

    public Guid Id { get; }
    public string Nome { get; }
    public IReadOnlyCollection<Rotina> Rotinas => _rotinas;
    public IReadOnlyCollection<SessaoDeTreino> Sessoes => _sessoes;

    public Usuario(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do usuário é obrigatório.", nameof(nome));

        Id = Guid.NewGuid();
        Nome = nome;
    }

    private Usuario(Guid id, string nome)
    {
        Id = id;
        Nome = nome;
    }

    public static Usuario Reconstruir(Guid id, string nome) =>
        new(id, nome);

    public void AdicionarRotina(Rotina rotina)
    {
        ArgumentNullException.ThrowIfNull(rotina);
        _rotinas.Add(rotina);
    }

    public void RegistrarSessao(SessaoDeTreino sessao)
    {
        ArgumentNullException.ThrowIfNull(sessao);
        _sessoes.Add(sessao);
    }
}
