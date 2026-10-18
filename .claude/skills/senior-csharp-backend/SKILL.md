---
name: senior-csharp-backend
description: Use ao implementar ou revisar código em ShApp.Core ou ShApp.Infrastructure (entidades, interfaces, repositórios, acesso a SQLite). Aplica padrões de C#/.NET sênior para a camada de domínio e dados do ShApp.
---

# Senior C# — Domínio e Dados (ShApp.Core / ShApp.Infrastructure)

Você aplica padrões de C#/.NET de nível sênior especificamente às camadas de domínio e dados do ShApp, que seguem Clean Architecture em três projetos (`ShApp.Core`, `ShApp.Infrastructure`, `ShApp.Mobile`).

## Regra de dependência (não negociável)

`ShApp.Core` não pode referenciar `ShApp.Infrastructure` nem `ShApp.Mobile`, e não pode conter nada específico de SQLite, sqlite-net ou MAUI. Ele define o quê (entidades em `Entities/`, enums em `Enums/`, contratos em `Interfaces/`), nunca o como. `ShApp.Infrastructure` implementa esses contratos e é o único lugar que conhece o banco. Se uma mudança parece exigir que `Core` saiba de algo em `Infrastructure`, o desenho está errado — inverta a dependência ou mova o conceito.

## Entidades vs. Records

`ShApp.Core/Entities` (ex.: `Usuario`, `Rotina`, `DiaDeTreino`, `SessaoDeTreino`) modelam o domínio. `ShApp.Infrastructure/Persistence` (ex.: `UsuarioRecord`, `RotinaRecord`) modelam o formato de armazenamento no SQLite. Não colapse os dois: ao adicionar um campo, decida explicitamente se ele pertence à entidade, ao record, ou aos dois, e escreva o mapeamento entre eles. Repositórios em `ShApp.Infrastructure/Repositories` implementam as interfaces de `ShApp.Core/Interfaces` e fazem esse mapeamento.

## Práticas obrigatórias

- **Async de ponta a ponta.** Toda chamada ao SQLite é `async`/`await`; nunca `.Result` ou `.Wait()`. Repositórios retornam `Task`/`Task<T>`.
- **Nullable reference types.** Trate warnings de nulidade como erro de lógica a resolver, não como ruído a silenciar com `!`.
- **Nomes em português do domínio.** O projeto usa nomes de domínio em PT-BR (`Usuario`, `Treino`, `Rotina`, `GrupoMuscular`). Siga essa convenção em entidades e interfaces novas — não traduza para inglês a meio caminho.
- **Validação no domínio.** Regras de negócio (ex.: uma `Rotina` precisa de pelo menos um `ItemDeRotina`, uma `SerieRegistrada` não pode ter repetições negativas) pertencem a `ShApp.Core`, não espalhadas pela UI.
- **Testes em `tests/ShApp.Core.Tests`.** Comportamento novo de domínio ganha teste xUnit (padrão Arrange-Act-Assert, um conceito por teste). Use `tools/ShApp.ConsoleTester` para verificação manual rápida contra o SQLite real quando fizer sentido.
- **Sem segredos no código.** Nenhuma connection string ou caminho sensível hard-coded fora de `AppDbConnectionFactory`.

## Ao tocar regras de negócio de treino

Sempre que a tarefa envolver cálculo, validação ou modelagem ligada ao domínio de treino/academia (progressão de carga, volume por grupo muscular, divisão de treino, séries/repetições), use também a skill `especialista-treinos` antes de decidir o modelo — ela documenta o domínio de academia e tem um glossário termo → código em `references/dominio-academia.md`.

## Antes de terminar

Rode `dotnet build` e `dotnet test` (ao menos o projeto/solução afetado) e confirme que ambos passam antes de reportar a tarefa como concluída.
