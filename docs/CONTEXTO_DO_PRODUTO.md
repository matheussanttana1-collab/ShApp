# ShApp — Contexto do Produto

Este documento registra a visão de produto do ShApp e o que, dessa visão, já existe de fato no código hoje. É o complemento "produto" da documentação técnica do repositório — não repete arquitetura ou convenções de código em detalhe, só situa onde cada peça da visão mora.

## Visão Geral

ShApp é um app mobile para academia e treino. O objetivo final é funcionar como um Personal Trainer e Nutricionista digital: acompanhar o desempenho do usuário nos exercícios, e permitir criar rotinas de treino e de alimentação personalizadas.

No futuro, terá uma IA especializada em duas frentes:

- **Educação Física** — treinos personalizados.
- **Nutrição** — dietas com base em preferências e na rotina do usuário.

Além do treino, o app vai cobrir:

- **Acompanhamento nutricional.**
- **Performance e consistência** — análise de desempenho semanal e mensal.
- **Evolução corporal** — peso corporal e percentual de gordura ao longo do tempo.

Haverá também um modo **"Foco Total"**, com mecânicas sociais e de gamificação:

- Competições contra amigos.
- Bolões baseados em performance.
- "Aposta contra si mesmo" — o usuário separa um valor e o recupera de acordo com sua frequência, performance e consistência.

O ShApp também vai funcionar como **rede social**: compartilhar fotos, competir com amigos. E, no futuro mais distante, como **app para a própria academia** gerenciar seus alunos — mostrando ao aluno a qual academia ele pertence, seu plano, suas rotinas/aulas, e uma rede de amigos baseada na academia e no horário que ele frequenta.

Dado o tamanho dessa visão, o projeto foi dividido em 5 fases. **Apenas a Fase 1 está em andamento.** As fases 2 a 5 ainda não foram detalhadas — existem só como visão futura (ver "Roadmap por Fases" abaixo).

## Arquitetura Atual

Solução .NET 10 em Clean Architecture, 100% offline/local (sem nenhuma chamada de rede), em 3 projetos:

- **`ShApp.Core`** — entidades e interfaces de domínio, nomes em português (`Usuario`, `Rotina`, `DiaDeTreino` etc.). Não depende de nenhum outro projeto da solução.
- **`ShApp.Infrastructure`** — implementa as interfaces do Core com repositórios SQLite (sqlite-net). Depende do Core.
- **`ShApp.Mobile`** — app .NET MAUI (Views/ViewModels com CommunityToolkit.Mvvm). Depende dos dois anteriores. Usa o design system "Obsidian" (tema escuro, violeta `#A78BFA`, tokens com prefixo `Ob` em `Resources/Styles`).

Convenções de código e de documentação técnica estão cobertas em outros materiais do projeto — esta seção só serve de mapa para a Fase 1 descrita abaixo.

## Roadmap por Fases

### Fase 1 — Acompanhamento de Treino (em andamento)

**Já implementado**

- **Catálogo de Exercícios** (`Exercicio`, `IExercicioRepository`/`ExercicioRepository`): CRUD completo — listar todos, buscar por id, buscar por grupo muscular, adicionar, atualizar, remover. Persistido em SQLite.
- **Divisão de Treino** (`DivisaoDeTreino`, `DiaDeTreino`, `IDivisaoDeTreinoRepository`): uma divisão tem vários dias, cada dia com um ou mais grupos musculares (enum `GrupoMuscular`, agrupável em `GrupoCorporal` — membros superiores/inferiores).
- **Rotina** (`Rotina`, `ItemDeRotina`, `IRotinaRepository`): rotina criada a partir de uma divisão, com dias da semana escolhidos e itens por dia (exercício + séries-alvo + repetições-alvo + ordem). A rotação entre os dias da divisão é contínua — não reinicia por semana (`Rotina.ObterDiaDeTreinoPara`). Criação/edição via wizard de 4 etapas (`RotinaWizardViewModel`: Estrutura → Frequência e Dias → Exercícios → Revisão), com presets prontos (ABC, ABCD, ABCDE, Upper/Lower, PPL) ou divisão livre.
- **Telas em uso**: Dashboard (treino do dia + prévia dos próximos treinos na rotação), Rotinas (estrutura completa da rotina ativa, ciclo semanal, criar/editar rotina), Perfil (dados do usuário e da rotina ativa). A tela de Rotinas já mostra, por exercício, o último peso usado (`ISessaoDeTreinoRepository.ObterUltimoPesoUtilizadoAsync`).
- **Acompanhamento do Treino ("Começar Treino")** (`SessaoDeTreinoPage`/`SessaoDeTreinoViewModel`, rota `sessao-treino`): resolve o treino de hoje a partir da rotina ativa, guia exercício por exercício e série por série (peso inicial sugerido = último peso usado), com um timer de descanso único global (`DescansoConfig.SegundosPadrao` = 90s, consolidando a constante que antes estava duplicada em `DashboardViewModel`/`DiaDraftViewModel`). Ao finalizar (ou ao usar "Encerrar Treino"/voltar pelo sistema), persiste a sessão via `ISessaoDeTreinoRepository.AdicionarAsync` — chamada exatamente uma vez, agora dentro de uma transação sqlite-net (`SessaoDeTreinoRepository.AdicionarAsync`), e só se pelo menos uma série foi de fato confirmada (sessões "vazias" não são salvas). O botão/gesto de voltar do sistema foi explicitamente tratado (`OnBackButtonPressed`) para não descartar o treino em silêncio. 3 testes novos cobrindo a validação de `SerieRegistrada` em `tests/ShApp.Core.Tests/SerieRegistradaTests.cs` (primeiro teste real do projeto). Revisado por code-reviewer em 18/10/2026.
  - Pendência conhecida, não bloqueante: `ObterUltimoPesoUtilizadoAsync` é N+1 (uma sessão de treino inteira busca o último peso exercício por exercício) — aceitável com o histórico atual, mas vai piorar conforme o histórico cresce; candidato a um método em lote no futuro.

**Planejado / em construção**

- **Esquema de volume (Alto/Médio/Baixo/Pirâmide) configurável na Rotina** — não existe. `ItemDeRotina` só guarda `SeriesAlvo`/`RepeticoesAlvo` como números livres, sem nenhuma categorização. Não há enum nem seletor desse tipo no wizard (`RotinaWizardViewModel` / `ViewModels/Wizard`) — hoje o usuário digita séries e repetições livremente por exercício (valor inicial ao adicionar um exercício: 4 séries / 10 repetições).
- **Dashboard de frequência com intensidade de cor refletindo performance** — parcialmente desenhado, não conectado a dados reais. O heatmap visual já existe em `DashboardPage.xaml`/`.xaml.cs`, com 5 níveis de cor (`ObHeat0` a `ObHeat4`) mais uma cor de célula vazia — ou seja, o conceito de "nível/intensidade" (e não só presença/ausência) já está montado visualmente. Mas os valores vêm de uma matriz fixa no código (`FrequencyWeeks`), com um TODO explícito confirmando isso: *"heatmap de frequência ainda não é alimentado por dados reais (ver DashboardViewModel)"*. `ISessaoDeTreinoRepository` hoje só tem duas operações (`AdicionarAsync` e `ObterUltimoPesoUtilizadoAsync`) — não há nenhuma consulta agregada (contagem de sessões por dia, streak, "completou tudo", "subiu carga/reps") que permita alimentar esse heatmap com dados reais. Isso exigiria pelo menos um novo método no Core/Infrastructure antes de a tela poder consumir dados reais.

**Decisões do Matheus (confirmadas em 18/10/2026)**

- **Timer de descanso**: será um padrão único/global do app por enquanto — não configurável por exercício nem por rotina nesta primeira versão.
- **Esquema de volume (Alto/Médio/Baixo/Pirâmide)**: escolhido para a rotina inteira, não por dia nem por exercício individual.
- **Cor do heatmap de performance**: critério combinado — completude do treino planejado + progressão de carga/repetições, cada um pesando uma parte da intensidade da cor (fórmula exata de ponderação ainda a definir).

### Fase 2 — a definir
- Sistema de "Aposta Contra Si Mesmo".
- Rotina de Alimentação.
- Acompanhamento de Alimentação.

### Fase 3 — a definir
- IA para criar/acompanhar treinos e dietas personalizadas.
- Nutricionista (IA).
- Personal (IA).

### Fase 4 — a definir
- Rede Social.
- Competições entre amigos.
- Bolão.

### Fase 5 — a definir
- ShApp para Academias (gestão de alunos, planos e rotinas pela própria academia).

## Perguntas / Pontos a Confirmar com o Matheus

- `Usuario.Rotinas` e `Usuario.Sessoes` existem como coleções no Core, mas `UsuarioRepository` nunca as popula ao carregar um usuário do banco (`ParaEntidade` só reconstrói id + nome). Hoje isso não trava nada porque as ViewModels sempre usam `IRotinaRepository`/`ISessaoDeTreinoRepository` diretamente — mas vale confirmar se essas coleções devem continuar existindo "vazias por padrão" no Core, ou se o repositório deveria hidratá-las.
- A constante de 90 segundos por série (duplicada em `DashboardViewModel` e `DiaDraftViewModel`) é hoje só uma estimativa de duração total do treino. Vale confirmar se esse é o número que deve virar o "descanso padrão sugerido" citado na Fase 1, ou se são conceitos diferentes (duração estimada vs. timer de descanso entre séries). **Decisão:** o app vai ter sim um descanso padrão global — falta só definir se reaproveita esse valor de 90s ou outro.
- `SerieRegistrada` (entidade do Core) não tem campo de ordem, mas o registro de persistência (`SerieRegistradaRecord`) tem — a ordem só é usada internamente por `ObterUltimoPesoUtilizadoAsync`. Vale confirmar se a futura tela "Começar Treino" vai precisar que `SerieRegistrada` modele também ordem (ou horário) de cada série, ou se a posição na lista basta.
- `ISessaoDeTreinoRepository` não tem nenhum método para listar sessões passadas (por usuário, por período, por dia) — só gravar e buscar o último peso de um exercício. Isso parece ser um pré-requisito tanto para o heatmap de performance real quanto para qualquer tela futura de histórico de treinos. Vale confirmar se esse é o próximo passo planejado no Core/Infrastructure. **Decisão:** a cor do heatmap vai combinar completude + progressão — essa combinação depende diretamente das consultas agregadas citadas aqui.
- As abas "Exercícios" e "Evolução" já existem no `AppShell`, mas ambas são hoje telas estáticas "Em breve", sem ViewModel e sem registro em `MauiProgram`. Hoje o catálogo de exercícios só é gerenciado indiretamente dentro do wizard de Rotina (opção "criar novo exercício"). Vale confirmar se essas abas são só reserva de espaço na navegação para funcionalidades futuras (ex.: tela dedicada de catálogo de exercícios, e a futura tela de Evolução/biometria mencionada no Perfil).
- `IRotinaRepository.ObterAtivaPorUsuarioAsync` sempre retorna a rotina com `DataInicio` mais recente do usuário — não existe conceito explícito de rotina "arquivada" ou "encerrada". Ou seja, criar uma nova rotina automaticamente passa a ser a "ativa", sem nenhum fluxo de arquivamento. Vale confirmar se esse comportamento deve continuar assim conforme o produto evolui (ex.: histórico de rotinas antigas, múltiplas rotinas em paralelo).
