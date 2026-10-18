---
name: senior-csharp-maui
description: Use ao implementar ou revisar código em ShApp.Mobile (Views, ViewModels, XAML, navegação, estilos). Aplica padrões de C#/.NET MAUI sênior e as convenções visuais já estabelecidas no ShApp.
---

# Senior C# — .NET MAUI (ShApp.Mobile)

Você aplica padrões de nível sênior especificamente à camada de UI do ShApp: .NET MAUI 10, MVVM com `CommunityToolkit.Mvvm`, design system "Obsidian" próprio do projeto.

## MVVM com CommunityToolkit.Mvvm

- ViewModels são `partial class` — os geradores de código de `[ObservableProperty]` e `[RelayCommand]` precisam disso para funcionar. Nunca remova `partial` nem escreva manualmente o backing field de uma `[ObservableProperty]`.
- Comandos que fazem I/O (acesso a repositório, navegação async) usam `[RelayCommand]` sobre um método `async Task`, nunca `async void` solto num `Command` manual.
- Toda View busca dados através da sua ViewModel, nunca direto de `ShApp.Core`/`ShApp.Infrastructure`.

## Fronteira com Core/Infrastructure

`ShApp.Mobile` consome `ShApp.Core` (contratos/interfaces) e `ShApp.Infrastructure` (implementações) por injeção de dependência, nunca abrindo uma conexão SQLite diretamente numa ViewModel. Se a tela precisa de um dado que a interface do repositório não expõe, isso é trabalho para o `backend-dev` mudar o contrato — não contorne acessando `Infrastructure` por um caminho lateral.

## Design system Obsidian — reuse, não reinvente

Antes de criar uma cor, espaçamento ou estilo novo, procure em `src/ShApp.Mobile/Resources/Styles/Colors.xaml` e `Styles.xaml` por uma chave `Ob*` existente (`ObCard`, `ObPill`, `ObPrimaryButton`, `ObEyebrow`, etc.) e reutilize. Só crie uma chave nova quando nenhuma existente cobrir o caso, e siga a mesma convenção de nome `Ob`-prefixado.

Ícones usam Material Symbols Outlined via codepoint (ligatures, ex. `&#xeb43;`). Confirme o codepoint no arquivo oficial de codepoints do Google antes de usar — nunca adivinhe um glyph.

## Build como rede de segurança

Com `MauiXamlInflator=SourceGen`, um `{Binding}` com caminho errado quebra o **build**, não só o runtime. Rode `dotnet build src/ShApp.Mobile/ShApp.Mobile.csproj -f net10.0-windows10.0.19041.0` depois de qualquer mudança em XAML ou ViewModel — é o alvo local mais rápido para pegar erro de binding sem precisar de tooling Android/iOS.

## Estados de tela

Toda tela que busca dados trata explicitamente carregando, vazio e erro — nunca assume que a lista vem sempre preenchida. Mantenha a área de toque adequada para mobile e rótulos acessíveis nos controles interativos.

## App 100% local

ShApp não faz nenhuma chamada de rede ("Nenhum dado sai deste aparelho"). Nunca introduza `HttpClient`, telemetria ou qualquer chamada externa.

## Terminologia e textos

Para o texto de uma tela (nomes de exercícios, rótulos de séries/repetições, mensagens sobre treino), e se o significado não estiver claro, consulte a skill `especialista-treinos` em vez de inventar — ela tem o glossário e o tom certo para o domínio de academia.

## Antes de terminar

Rode o build do projeto Mobile (comando acima) e, se houver testes de UI, rode-os. Corrija o que quebrar antes de reportar a tarefa como concluída.
