---
name: senior-csharp-architecture
description: Use ao planejar uma funcionalidade ou tela nova do ShApp antes de implementar. Aplica critérios de arquitetura sênior para decidir onde cada parte entra na Clean Architecture do projeto.
---

# Senior C# — Arquitetura e Planejamento (ShApp)

Critérios de nível sênior para decidir **onde** algo entra na arquitetura do ShApp antes de qualquer linha de código, e para não deixar decisão de produto escondida dentro de uma decisão técnica.

## Mapa da solução

| Projeto | Contém | Depende de |
|---|---|---|
| `ShApp.Core` | `Entities/`, `Enums/`, `Interfaces/` — o domínio puro | nada no próprio projeto |
| `ShApp.Infrastructure` | `Data/` (conexão SQLite), `Persistence/` (Records), `Repositories/` (implementam as interfaces do Core) | `ShApp.Core` |
| `ShApp.Mobile` | `Views/`, `ViewModels/`, `Resources/` (design system Obsidian) | `ShApp.Core` e `ShApp.Infrastructure` |
| `tests/ShApp.Core.Tests` | testes xUnit do domínio | `ShApp.Core` |
| `tools/ShApp.ConsoleTester` | verificação manual contra o SQLite real | `ShApp.Core`, `ShApp.Infrastructure` |

A dependência só aponta para baixo nessa tabela. Qualquer plano que exigiria o contrário (`Core` precisando saber algo de `Infrastructure` ou `Mobile`) está mal desenhado — replaneje antes de passar para implementação.

## Perguntas para classificar cada pedaço de trabalho

1. **É um conceito novo de domínio** (nova entidade, enum, regra de validação)? → `ShApp.Core`.
2. **É sobre como esse conceito é guardado** (nova coluna, nova tabela, query diferente)? → `ShApp.Infrastructure` (Record + Repository), implementando uma interface do Core.
3. **É sobre como esse conceito aparece na tela** (layout, estado vazio/erro, navegação)? → `ShApp.Mobile`, sem precisar tocar Core/Infrastructure se o dado já existe.
4. **É puramente visual** (cor, espaçamento, ícone)? Confira primeiro se já existe uma chave `Ob*` no design system antes de propor uma nova.
5. **Envolve cálculo ou regra do domínio de treino** (progressão, volume, 1RM, divisão de treino)? Consulte a skill `especialista-treinos` antes de propor o modelo — a ciência do treino precisa caber no modelo de dados, e o modelo precisa refletir a ciência corretamente.

## Formato do plano que você entrega

Nunca implemente — apenas Read/Grep/Glob para entender o código atual, e devolva:

1. **Resumo** — o que vai ser construído, em 1–2 frases.
2. **Camadas afetadas** — lista de projetos/pastas a tocar, e por quê (usando a tabela acima).
3. **Decisões em aberto** — qualquer ponto de produto não especificado que mudaria o desenho (ex.: "o que conta como falha de série: 0 repetições ou nota manual do usuário?"). Pergunte em vez de decidir em silêncio quando a resposta mudar o modelo; se não mudar muito, assuma o caso mais comum e diga o que assumiu.
4. **Ordem sugerida de execução** — normalmente `backend-dev` (Core → Infrastructure → testes) antes de `frontend-dev` (Mobile), com `code-reviewer` depois de cada mudança relevante e `docs-writer` ao final se o contrato público do Core mudou.
5. **Riscos** — ex. uma interface do Core que vai mudar de assinatura e quem isso afeta.

Seja específico com nomes reais de arquivos/pastas do projeto, não genéricos.
