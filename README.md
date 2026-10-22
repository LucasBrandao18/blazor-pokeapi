# BlazorPokeApi

Aplicação em **C# e Blazor WebAssembly (.NET 10)** desenvolvida a partir da lista `23.lista_net_ex.pdf`, do professor Daniel Henrique Matos de Paiva.

## O que foi solicitado

- **Item a:** criar um aplicativo Blazor WebAssembly que peça o nome de um Pokémon e consulte `https://pokeapi.co/api/v2/pokemon/{nome}`. O projeto usa o nome sugerido, `BlazorPokeApi`.
- **Item b:** mostrar na tela imagem, ID, nome, altura e peso do Pokémon retornado.
- Usar obrigatoriamente a imagem de `sprites.other["official-artwork"].front_default`, conforme a página 2 do PDF.
- Garantir a consulta pela internet e a apresentação dos dados ao usuário.
- Incluir README, `.gitignore` do Visual Studio e licença MIT.

Os arquivos estão diretamente na raiz solicitada. A criação/publicação do repositório GitHub e a entrega no Google Classroom serão feitas manualmente pelo aluno. O nome de repositório solicitado no PDF é `blazor-pokeapi`.

## Requisitos

- SDK do **.NET 10** instalado.
- Navegador atualizado.
- Internet para restaurar os pacotes, consultar a PokéAPI e carregar as imagens.

Não é necessária chave de API nem banco de dados.

## Como executar

No PowerShell:

```powershell
cd "C:\Users\lucas\OneDrive\Documentos\vs code uni\homework-BlazorPokeApi"
dotnet restore
dotnet run
```

Abra [http://localhost:5261](http://localhost:5261). Digite um nome e clique em **Buscar Pokémon**, ou pressione **Enter** dentro do campo. Exemplos: `pikachu`, `ditto`, `bulbasaur` e `mr-mime`.

O aplicativo aceita maiúsculas e espaços nas extremidades: ` PIKACHU ` é consultado como `pikachu`. Use os nomes reconhecidos pela API; nomes compostos podem conter hífen. A primeira abertura não busca um Pokémon automaticamente.

Para encerrar a execução, pressione `Ctrl+C` no terminal. Para iniciar novamente, execute `dotnet run`.

No VS Code, abra esta pasta raiz e use **Terminal > Executar Tarefa > executar BlazorPokeApi**. Há também uma tarefa de compilação. As extensões C# e C# Dev Kit são recomendadas em `.vscode/extensions.json`.

Se a porta já estiver ocupada, encerre a execução anterior ou use outra porta:

```powershell
dotnet run --urls http://localhost:5262
```

Nesse caso, abra a porta 5262 no navegador.

## Como o código funciona

### Configuração HTTP

`Program.cs` inicializa o aplicativo e registra o `HttpClient` na injeção de dependências. A URL base é `https://pokeapi.co/api/v2/`, com tempo limite de 15 segundos. O componente recebe esse cliente com `@inject HttpClient Http`.

O método `BuscarPokemon` monta o caminho `pokemon/{nome}` com o nome normalizado. Não existe personagem fixo no código: cada busca usa a entrada atual do usuário.

### Modelos e desserialização

`Models/Pokemon.cs` contém as classes que representam os campos usados da resposta:

| Classe/propriedade | Campo JSON | Finalidade |
| --- | --- | --- |
| `Pokemon.Id` | `id` | Identificador |
| `Pokemon.Nome` | `name` | Nome |
| `Pokemon.Altura` | `height` | Altura em decímetros |
| `Pokemon.Peso` | `weight` | Peso em hectogramas |
| `Pokemon.Sprites` | `sprites` | Objeto de imagens |
| `PokemonSprites.Outras` | `other` | Outras imagens |
| `OutrasImagens.ArteOficial` | `official-artwork` | Arte oficial |
| `ArteOficial.Frente` | `front_default` | URL exigida no exercício |

Os atributos `JsonPropertyName` associam os nomes do JSON às propriedades C#, incluindo `official-artwork`, que contém hífen. Campos não utilizados da resposta são ignorados na desserialização.

### Busca e apresentação

`Pages/Home.razor` reúne o formulário, o resultado e a lógica da consulta:

1. `@bind` com evento `oninput` atualiza o nome enquanto o usuário digita.
2. Ao enviar o formulário, o método limpa o resultado anterior, remove espaços externos, converte o nome para minúsculas e valida a entrada.
3. `GetAsync` consulta a API e verifica o status HTTP.
4. `ReadFromJsonAsync<Pokemon>` converte o JSON para os modelos.
5. A página exibe ID, nome, altura, peso e a imagem oficial retornada.

A API fornece altura em **decímetros** e peso em **hectogramas**. A divisão por `10m` usa aritmética decimal para converter para metros e quilogramas, preservando casas decimais. A formatação usa vírgula explicitamente, independentemente do idioma do navegador. Por exemplo, para Pikachu, `height: 4` vira **0,4 m** e `weight: 60` vira **6 kg**.

### Imagem obrigatória

A URL do elemento `<img>` vem exclusivamente de:

```text
sprites.other["official-artwork"].front_default
```

Não são usadas imagens locais, URLs de Pokémon construídas manualmente, o sprite simples `sprites.front_default` nem a variante `front_shiny`. A URL retornada precisa ser HTTPS.

Se a arte oficial estiver ausente, a página informa isso e mantém os demais dados. Se ocorrer erro ao carregar a imagem, o evento `@onerror` apresenta uma mensagem. Uma nova busca permite tentar o carregamento novamente.

### Tratamento de erros

- Entrada vazia ou fora do formato: mensagem de orientação, sem requisição.
- HTTP 404: Pokémon não encontrado.
- Outros erros HTTP: serviço indisponível.
- Falha de rede e tempo limite: mensagens próprias e possibilidade de tentar novamente.
- JSON inválido, resultado nulo ou sem ID/nome válido: resultado rejeitado.
- Altura/peso ausentes ou negativos: **Não informado pela API**.
- Imagem ausente ou com falha: aviso, mantendo os dados recebidos.

O campo e o botão ficam desabilitados durante a consulta. Uma proteção adicional evita requisições simultâneas. A requisição pendente é cancelada quando o componente é encerrado.

## Organização dos arquivos

```text
BlazorPokeApi.csproj         Configuração do .NET e dependências
Program.cs                  Inicialização e HttpClient
Models/Pokemon.cs           Modelos do Pokémon e da arte oficial
Pages/Home.razor             Busca, resultado e tratamento de erros
Pages/Home.razor.css         Estilos do formulário e da imagem
Layout/                     Estrutura visual e menu
App.razor                   Rotas
_Imports.razor              Importações comuns
Properties/launchSettings.json  Endereço de execução local
wwwroot/                    HTML, Bootstrap e arquivos estáticos
.vscode/                    Tarefas e recomendações de extensões
tests/BlazorPokeApi.Tests/   Testes automatizados
.gitignore                  Exclusão de arquivos gerados
LICENSE                     Licença MIT
README.md                   Documentação
```

As páginas de exemplo Counter e Weather foram removidas para manter o foco no exercício. O C# roda no navegador via WebAssembly; o servidor iniciado por `dotnet run` entrega os arquivos do aplicativo.

## Compilação e testes

Execute na raiz:

```powershell
dotnet build
dotnet test "tests\BlazorPokeApi.Tests\BlazorPokeApi.Tests.csproj"
```

Foram implementados **16 testes automatizados com bUnit e xUnit**, cobrindo normalização do nome, conversão das medidas, seleção do campo correto da imagem, entradas inválidas, HTTP 404/503, falha de rede, tempo limite, JSON inválido, resultado nulo, imagem ausente, erro de carregamento da imagem, nova tentativa e bloqueio de consultas duplicadas.

Os testes usam respostas HTTP simuladas para não depender da disponibilidade da API. A pasta `tests` é excluída do aplicativo principal pelo `.csproj`, não sendo distribuída ao navegador.

A consulta real também foi conferida no navegador. Nenhum Pokémon de teste está fixado na aplicação; os exemplos das respostas simuladas existem somente no projeto de testes.

## Entrega manual e licença

O `.gitignore` usa o modelo do Visual Studio/.NET e ignora `bin/`, `obj/` e outros arquivos gerados. Essas pastas são recriadas durante a compilação e não precisam ser enviadas ao repositório.

O arquivo `LICENSE` contém a licença MIT solicitada para o código deste projeto. Os dados, imagens e marcas de Pokémon pertencem aos respectivos titulares; a licença do código não transfere direitos sobre esses conteúdos.

Nenhum repositório foi criado e nada foi publicado no GitHub ou no Google Classroom.

## Referências

- Material fornecido: `23.lista_net_ex.pdf`.
- [Documentação oficial da PokéAPI](https://pokeapi.co/docs/v2#pokemon).
- [Endpoint de exemplo: Ditto](https://pokeapi.co/api/v2/pokemon/ditto).
