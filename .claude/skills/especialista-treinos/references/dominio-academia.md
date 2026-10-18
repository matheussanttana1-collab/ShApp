# Domínio de academias — para quem constrói software

## Sumário
1. Como uma academia funciona
2. Glossário: termo da academia → nome no código
3. Modelo de domínio
4. Decisões de modelagem que costumam dar problema
5. Regras de negócio comuns
6. Legal e privacidade (Brasil)
7. Funcionalidades por perfil

---

## 1. Como uma academia funciona

**Jornada típica do aluno**

1. **Matrícula:** escolhe um plano comercial (mensal, trimestral, semestral, anual; muitos são recorrentes no cartão), assina o contrato e paga.
2. **Triagem:** responde anamnese e PAR-Q. Dependendo da academia e da legislação local, pode precisar de atestado médico.
3. **Avaliação física** (opcional, em geral oferecida): peso, altura, circunferências, dobras cutâneas ou bioimpedância, às vezes testes de força e flexibilidade.
4. **Ficha de treino:** o professor monta a ficha (treinos A, B, C...) conforme objetivo e nível.
5. **Rotina:** acessa a academia pela catraca (biometria, QR code, cartão ou app), segue a ficha na sequência das letras e participa de aulas coletivas.
6. **Troca de ficha:** a cada 4–12 semanas, ou após uma reavaliação.
7. **Renovação, congelamento ou cancelamento** do plano.

**Pessoas envolvidas**

- **Aluno.**
- **Professor / instrutor de sala:** monta fichas, orienta na musculação. Profissional de Educação Física com registro no CREF.
- **Personal trainer:** atende alunos de forma individual. Pode ser da casa ou externo; academias costumam cobrar taxa de personal externo.
- **Recepção / comercial:** matrículas, cobranças, atendimento.
- **Gestor:** indicadores, ocupação, inadimplência e evasão.

**Outros conceitos frequentes**

- **Aulas coletivas:** spinning, funcional, pilates de solo, dança, lutas. Têm grade de horários, professor, sala e lotação máxima.
- **Agregadores de benefícios** (ex.: Wellhub, TotalPass): o aluno entra por um parceiro; a academia valida o check-in e recebe por visita ou por acordo.
- **Evasão (churn):** o maior problema financeiro das academias. Queda de frequência por algumas semanas seguidas é um sinal de risco de cancelamento.

## 2. Glossário: termo da academia → nome no código

| Termo | Significado | Nome sugerido |
|---|---|---|
| Ficha de treino | Conjunto de treinos prescritos para um período | `FichaTreino` / `WorkoutPlan` |
| Treino A, B, C | Uma sessão dentro da ficha | `SessaoTreino` / `WorkoutSession` |
| Exercício (catálogo) | Movimento genérico (ex.: supino reto) | `Exercicio` / `Exercise` |
| Exercício prescrito | Exercício dentro de um treino, com séries e parâmetros | `ExercicioPrescrito` / `PrescribedExercise` |
| Série | Um bloco de repetições | `Serie` / `Set` |
| Repetição | Um movimento completo | `Reps` |
| Carga | Peso utilizado | `CargaKg` / `LoadKg` |
| Descanso | Intervalo entre séries | `DescansoSegundos` / `RestSeconds` |
| Cadência | Velocidade das fases do movimento | `Cadencia` / `Tempo` |
| Série de aquecimento / aproximação | Série leve que não conta como volume | `TipoSerie.Aquecimento` |
| Série válida | Série com esforço real | `TipoSerie.Valida` |
| Bi-set, tri-set | Exercícios agrupados sem descanso entre si | `GrupoSequencia` / `SupersetGroup` |
| Registro de treino | O que o aluno de fato fez numa sessão | `RegistroTreino` / `WorkoutLog` |
| Plano (comercial) | Produto vendido: mensal, anual... | `PlanoComercial` / `MembershipPlan` |
| Matrícula | Vínculo do aluno com um plano num período | `Matricula` / `Membership` |
| Check-in | Entrada registrada na academia | `CheckIn` |
| Avaliação física | Medidas e testes num dia | `AvaliacaoFisica` / `Assessment` |
| Anamnese / PAR-Q | Questionário de saúde | `Anamnese` / `HealthQuestionnaire` |
| Aula coletiva | Modalidade com horário e lotação | `Aula` / `GroupClass` |
| Congelamento / trancamento | Pausa temporária do plano | `Congelamento` / `Freeze` |

Atenção ao conflito de nomes: "plano" significa tanto o **plano comercial** quanto o **plano de treino**. Use nomes distintos no código (`PlanoComercial` e `FichaTreino`) para evitar confusão.

## 3. Modelo de domínio

```
Academia/Unidade ─1:N─ Aluno ─1:N─ Matricula ─N:1─ PlanoComercial
                          │
                          ├─1:N─ AvaliacaoFisica
                          ├─1:N─ CheckIn
                          ├─1:N─ FichaTreino ─1:N─ SessaoTreino (A, B, C)
                          │                          └─1:N─ ExercicioPrescrito ─N:1─ Exercicio (catálogo)
                          │                                   └─1:N─ SeriePrescrita
                          └─1:N─ RegistroTreino ─1:N─ SerieExecutada
```

Esboço em C# (ajuste aos padrões do projeto, por exemplo entidades do EF Core em vez de records):

```csharp
public enum GrupoMuscular { Peito, Costas, Trapezio, Ombros, Biceps, Triceps, Antebraco,
    Abdomen, Lombar, Gluteos, Quadriceps, Posterior, Adutores, Abdutores, Panturrilha }

public enum TipoMedida { Repeticoes, Tempo, Distancia }
public enum TipoSerie { Aquecimento, Valida, DropSet }

public record Exercicio(
    Guid Id,
    string Nome,
    IReadOnlyList<string> Apelidos,          // "serrote", "remada unilateral"
    GrupoMuscular GrupoPrincipal,
    IReadOnlyList<GrupoMuscular> GruposSecundarios,
    string Equipamento,
    string PadraoMovimento,
    TipoMedida Medida,
    bool Unilateral,
    Guid? CriadoPorAcademiaId);              // null = catálogo global

public record SeriePrescrita(
    int Ordem,
    TipoSerie Tipo,
    int? RepsMin, int? RepsMax,              // 8–12; iguais = valor fixo; nulos = até a falha
    int? DuracaoSegundos,                    // para TipoMedida.Tempo
    decimal? CargaKg,                        // null = peso corporal ou "a definir"
    int? RirAlvo,
    int DescansoSegundos);

public record ExercicioPrescrito(
    Guid Id, Guid ExercicioId, int Ordem,
    string? GrupoSequencia,                  // mesmo valor = bi-set/tri-set
    string? Cadencia,                        // "3-0-1-0"
    string? Observacao,
    IReadOnlyList<SeriePrescrita> Series);

public record SessaoTreino(Guid Id, string Rotulo, string? Nome,   // "A", "Peito e tríceps"
    IReadOnlyList<ExercicioPrescrito> Exercicios);

public record FichaTreino(
    Guid Id, Guid AlunoId, Guid? ProfessorId,
    int Versao, DateOnly Inicio, DateOnly? Validade,
    string Objetivo, IReadOnlyList<SessaoTreino> Sessoes);

public record SerieExecutada(
    Guid ExercicioPrescritoId, Guid ExercicioId, int Ordem,
    int? Reps, int? DuracaoSegundos, decimal? CargaKg, int? Rir, bool Concluida);

public record RegistroTreino(
    Guid Id, Guid AlunoId, Guid FichaId, int FichaVersao, Guid SessaoId,
    DateTimeOffset Inicio, DateTimeOffset? Fim,
    IReadOnlyList<SerieExecutada> Series);
```

## 4. Decisões de modelagem que costumam dar problema

- **Separe prescrição de execução.** A ficha diz o que *deveria* ser feito; o registro diz o que *foi* feito. Se o app sobrescrever a prescrição com o executado, o histórico, a progressão e os relatórios do professor deixam de fazer sentido.
- **Versione a ficha.** Quando o professor edita a ficha, os registros antigos precisam continuar apontando para a versão que o aluno seguiu. Use versão incremental ou torne a ficha imutável e crie uma nova a cada alteração.
- **Guarde o `ExercicioId` também na série executada.** Se o aluno trocar o exercício na hora (máquina ocupada), o registro reflete o que ele fez de verdade, e o histórico por exercício continua correto.
- **Séries podem ser diferentes entre si.** Pirâmide e drop-set exigem uma lista de séries, não só "4 × 10". Uma alternativa é oferecer a forma simples na interface e expandir para a lista no modelo.
- **Repetições são uma faixa.** Modele mínimo e máximo; valor fixo é o caso em que os dois são iguais. "Até a falha" (AMRAP) também precisa ser representável.
- **Nem tudo é repetição.** Prancha é por tempo, cardio por tempo ou distância, unilaterais são "por lado".
- **Carga é imprecisa no mundo real.** Muitos alunos anotam o número da placa da máquina, não os quilos. Aceite carga decimal opcional e um campo de observação; não force kg exatos.
- **Peso corporal e carga adicional.** Barra fixa com 10 kg no cinto é carga adicional; barra fixa assistida reduz a carga. Defina como representar os dois.
- **Valores derivados se calculam, não se guardam como verdade.** 1RM estimado, volume semanal e recordes pessoais (PR) devem ser calculados a partir dos registros, ou guardados como cache recalculável.
- **Data e fuso.** Grave o horário em UTC com offset (`DateTimeOffset`), mas agrupe "treinos do dia" pela data local do aluno. Treino que começa às 23h50 não pode cair em dois dias.
- **Funcionamento offline.** Sinal de celular dentro da academia costuma ser ruim. Registre o treino localmente e sincronize depois, e decida o que acontece se o professor alterar a ficha durante uma sessão em andamento.
- **Catálogo global e personalizado.** Ofereça um catálogo padrão e deixe academias e professores criarem exercícios próprios e apelidos, sem duplicar o exercício global. Vídeos e GIFs de exercícios têm direitos de imagem: use mídia própria ou licenciada.

## 5. Regras de negócio comuns

**Validações sugeridas**

| Campo | Faixa | Observação |
|---|---|---|
| Repetições | 1–100 | Acima disso, provavelmente é tempo ou erro de digitação |
| Descanso | 0–600 s | |
| Carga | ≥ 0, até 1–2 casas decimais | Halteres de 1 kg e anilhas de 1,25 kg são comuns |
| RIR | 0–5 | |
| RPE | 1–10 | Aceite meio ponto (ex.: 8,5) |
| Validade da ficha | 4–12 semanas | Avise o aluno e o professor quando estiver vencendo |

**Treino e ficha**

- **Próximo treino sugerido:** a letra seguinte à última sessão concluída (A → B → C → A), não o dia da semana. É assim que a maioria das academias brasileiras funciona.
- **Pré-preenchimento:** ao registrar uma série, sugira a carga e as repetições da última vez que o aluno fez aquele exercício.
- **Progressão sugerida:** use dupla progressão (ver `ciencia-do-treino.md`) e mostre como sugestão. Nunca altere a ficha sozinho.
- **Volume semanal por grupo:** conte só séries válidas. Decida e documente como contar grupos secundários (convenção comum: 0,5 série).
- **Aderência:** frequência real dividida pela frequência planejada. É útil para o professor e é um bom sinal de risco de evasão.

**Acesso e planos**

- **Check-in:** bloqueie se a matrícula estiver vencida ou inadimplente, com uma tolerância configurável de dias. Limite check-ins por dia para evitar compartilhamento de acesso.
- **Congelamento:** prorroga o vencimento pelo período congelado. Normalmente há um limite de dias por ano.
- **Renovação e cancelamento:** seguem o contrato. Mostre com clareza datas, multa e renovação automática.

**Aulas coletivas**

- Reserva com antecedência, lotação máxima, fila de espera e cancelamento até X horas antes.
- Registro de faltas (no-show), com possível bloqueio temporário de reservas.

**Avaliação física**

- Guarde o método usado (dobras, bioimpedância, fita) e o avaliador. Só compare avaliações feitas com o mesmo método.
- Mostre a evolução em gráficos, sem linguagem de julgamento sobre o corpo do aluno.

## 6. Legal e privacidade (Brasil)

Isto não substitui orientação jurídica; use como checklist para conversar com quem cuida do assunto.

- **LGPD — dados sensíveis:** anamnese, PAR-Q, lesões, avaliação física, medidas e fotos de evolução são dados de saúde, portanto dados pessoais sensíveis (Lei 13.709/2018, art. 5º, II). O tratamento segue o art. 11, que exige consentimento específico e destacado ou outra hipótese prevista ali.
  - Colete só o necessário.
  - Restrinja o acesso: o professor responsável vê, a recepção não precisa ver.
  - Criptografe em trânsito e em repouso.
  - Ofereça exclusão e portabilidade dos dados.
- **Menores de idade:** o tratamento de dados de crianças e adolescentes segue o art. 14 da LGPD, com consentimento de um dos pais ou do responsável quando se trata de criança.
- **Exercício da profissão:** prescrever e orientar exercícios é atividade do profissional de Educação Física (Lei 9.696/1998). Em treinos gerados automaticamente, deixe claro que são sugestões e permita revisão profissional. Guarde o CREF do profissional responsável.
- **Atestado médico x PAR-Q:** a exigência varia por estado e município. Deixe isso configurável por unidade, em vez de fixar no código.
- **Contratos e cobrança:** renovação automática, multa de cancelamento e reajuste precisam ser apresentados com clareza ao consumidor.

## 7. Funcionalidades por perfil

**Aluno**
- Treino do dia com o próximo treino sugerido
- Registro rápido de séries com a carga da última vez preenchida
- Timer de descanso com notificação
- Vídeo ou instruções do exercício e troca por um substituto
- Histórico, recordes pessoais e gráficos de evolução
- Check-in pelo app, grade e reserva de aulas
- Aviso de ficha vencendo e de reavaliação

**Professor**
- Montar fichas a partir de modelos, duplicar e ajustar
- Ver a aderência dos alunos e quem está há X dias sem treinar
- Reavaliações pendentes
- Comentários na execução do aluno

**Gestão**
- Matrículas, inadimplência e renovações
- Ocupação por horário (picos da sala de musculação)
- Evasão e alunos em risco
- Lotação e frequência das aulas coletivas
