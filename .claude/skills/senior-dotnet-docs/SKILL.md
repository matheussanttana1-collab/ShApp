---
name: senior-dotnet-docs
description: Use ao escrever ou atualizar documentação (README, docs/, CHANGELOG) para o ShApp. Aplica convenções de documentação sênior para uma solução .NET em camadas (Core/Infrastructure/Mobile).
---

# Senior .NET — Documentação (ShApp)

Convenções de nível sênior para documentar o ShApp: uma solução .NET 10 em Clean Architecture (`ShApp.Core`, `ShApp.Infrastructure`, `ShApp.Mobile`), mais `tests/ShApp.Core.Tests` e `tools/ShApp.ConsoleTester`, sem nenhuma API HTTP.

## Não existe "endpoint" — existe contrato de Core

Este projeto não tem API REST. O equivalente ao "contrato de endpoint" que outro projeto documentaria é uma **interface pública em `ShApp.Core/Interfaces`** (ex.: `IRotinaRepository`). Para cada uma, documente:

- Assinatura completa do método e o que cada parâmetro representa.
- O que a implementação em `ShApp.Infrastructure/Repositories` efetivamente faz (ex.: quais tabelas SQLite toca).
- Casos de borda já tratados (registro inexistente, lista vazia) e o que lança excessão vs. o que retorna nulo/vazio.

Use comentários XML (`/// <summary>`, `<param>`, `<returns>`) na interface como fonte da verdade, e sincronize a prosa do `docs/` com eles — nunca deixe os dois contarem histórias diferentes.

## Nomenclatura

O domínio é nomeado em português no próprio código (`Usuario`, `Rotina`, `DiaDeTreino`, `GrupoMuscular`). Mantenha esses nomes em português na documentação também — não traduza "Rotina" para "Routine" em prosa enquanto o código diz `Rotina`; isso cria um glossário fantasma que não existe no código.

## Setup que você documenta precisa ser copiável e real

Comandos de exemplo devem corresponder ao que este projeto realmente usa:

```bash
dotnet build
dotnet test
dotnet build src/ShApp.Mobile/ShApp.Mobile.csproj -f net10.0-windows10.0.19041.0
dotnet run --project tools/ShApp.ConsoleTester
```

Antes de publicar um comando no docs, confirme contra o `.slnx`/`.csproj` real que o caminho e o target framework ainda são esses — não copie de memória.

## Estrutura do projeto, para quem nunca viu

Ao documentar a arquitetura, explique a direção da dependência em uma frase: `ShApp.Core` não depende de nada do projeto; `ShApp.Infrastructure` depende de `ShApp.Core`; `ShApp.Mobile` depende dos dois. Isso poupa o próximo desenvolvedor de inferir isso lendo `.csproj` um por um.

## Escopo

Você só edita arquivos de documentação (`README.md`, `docs/`, `CHANGELOG.md`) — nunca código de produção, mesmo que a correção pareça trivial.
