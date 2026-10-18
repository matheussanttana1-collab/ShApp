---
name: senior-csharp-review
description: Use ao revisar código C#/.NET do ShApp (Core, Infrastructure ou Mobile) antes de aprovar uma mudança. Checklist de revisão sênior específico para Clean Architecture + MAUI + sqlite-net.
---

# Senior C# — Checklist de Revisão (ShApp)

Checklist adicional de nível sênior para revisar C#/.NET no ShApp, além dos pontos genéricos de correção/segurança/legibilidade já cobertos na revisão padrão.

## Fronteiras da arquitetura

- `ShApp.Core` não referencia `ShApp.Infrastructure` nem `ShApp.Mobile`, e não tem usings de SQLite/sqlite-net/MAUI. Qualquer violação disso é crítico, não sugestão.
- ViewModels em `ShApp.Mobile` dependem de interfaces de `ShApp.Core`, não de classes concretas de `ShApp.Infrastructure`.
- Mudança de assinatura numa interface em `ShApp.Core/Interfaces` — como não existe camada de contrato versionada entre Mobile e Core (é referência direta de projeto), confirme que **toda** implementação (`Infrastructure/Repositories`) e **todo** consumidor (ViewModels) foram atualizados, não só o lado que motivou a mudança.

## Padrões C#/.NET

- Async de ponta a ponta: nenhum `.Result`, `.Wait()` ou `async void` em código que não seja um event handler de UI.
- Nullable reference types: nenhum `!` (null-forgiving) sem um comentário explicando por que é seguro; de resto, trate warning de nulidade como bug.
- LINQ sobre resultado de query SQLite: atenção a enumeração múltipla do mesmo `IEnumerable` (cada enumeração pode ser uma query nova) e a buscar em loop (`N` chamadas ao repositório dentro de um `foreach`) quando uma única chamada bastaria.
- `IDisposable`: conexões/`SqliteConnection` abertas em `AppDbConnectionFactory` ou repositórios são descartadas corretamente (`using`/`await using`), mesmo em caminho de exceção.

## Padrões MVVM/MAUI

- `[ObservableProperty]`/`[RelayCommand]` só funcionam em classe `partial` — se alguém removeu `partial` ou voltou a escrever backing field manual, é regressão.
- Handlers de evento assinados em código (não via binding) têm `-=` correspondente para não vazar memória (comum em `ContentPage`/`CollectionView`).
- Nenhuma cor, espaçamento ou ícone hard-coded quando já existe uma chave `Ob*` ou um codepoint de Material Symbols equivalente em `Resources/Styles/`.
- Nenhuma chamada de rede introduzida — o app é 100% local.

## Regras de negócio do domínio de treino

Ao revisar lógica que calcula ou valida algo do domínio de treino (progressão, volume, séries/repetições, divisão de treino), confira se o cálculo bate com o que a skill `especialista-treinos` documenta antes de aprovar — um erro aqui é um erro de produto, não só de código.

## Formato

Classifique cada achado como já definido no seu prompt de revisor (Críticos / Avisos / Sugestões), com arquivo, linha e exemplo de correção.
