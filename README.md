![Build Status](https://github.com/RafaelMuniz94/wake_commerce_crud_api/actions/workflows/ScriptBuildTest.yml/badge.svg) [![Coverage Status](https://coveralls.io/repos/github/RafaelMuniz94/wake_commerce_crud_api/badge.svg?branch=main)](https://coveralls.io/github/RafaelMuniz94/wake_commerce_crud_api?branch=main) ![Versão do .NET](https://img.shields.io/badge/.NET-7.x-blueviolet) ![Última Atualização](https://img.shields.io/github/last-commit/RafaelMuniz94/wake_commerce_crud_api/main)

---

# Projeto Wake Commerce
Desenvolva uma API para realizar o CRUD de Produtos.


---
## Requisitos
### Funcionais
- [x] Criar um produto
- - [x] O valor do produto não pode ser negativo
- [x] Atualizar um produto
- [x] Deletar um produto
- [x] Listar os produtos
- - [x] Visualizar um produto específico
- - [x] Ordenar os produtos por diferentes campos
- - [x] Buscar produto pelo nome

### Não Funcionais
- [x] Subir o código em um repositório público no github
- [x] README no projeto com breve explicação dos projetos incluídos em sua Solution
- [x] Utilizar .NET 6 ou superior
- [x] Utilizar o Entity Framework
- - [x] Pode usar code-first ou db-first, mas o uso deve estar detalhado no README
- - [x] Popular a base de dados com 5 produtos ao iniciar a aplicação
- [x] A entidade de Produto deve conter pelo menos: Nome, Estoque e Valor
- [x] Incluir projeto de testes unitários

### Bonus
- [x] Utilizar padrões de projeto
- [x] Utilizar github actions para os testes
- [x] Incluir testes de integração


# Estrutura do projeto

A solution foi dividida em 4 projetos, sendo eles:

## Produto_api

Projeto principal, onde de fato a execução da aplicação ocorre, nele esta toda a parte de requisições e processamento da api e temos a pipeline principal (definida em **program.cs**). 

Temos a definição das viewmodels nesse projeto, elas são utilizadas como porta de entrada para os dados recebidos de outras aplicações, dessa forma não é necessário que os clients possuam conhecimento das entidades e da estrutura dos dados.

Esse projeto utiliza um middleware para gerenciar os erros que possam ocorrer durante o tempo de processamento, dessa forma evitando que o cliente receba uma exception e possua informações do funcionamento interno da aplicação. A utilização desse middleware corresponde ao padrão de projetos **Chain of Responsibility**, um exemplo desse padrão:

` using (LogContext.PushProperty("RequestId", Guid.NewGuid()))`

Essa instrução envolve a execução das demais etapas do código, dessa forma todos os logs que ocorrerem a seguir irão utilizar o mesmo `Guid` para identificar o contexto da execução. 

Outra estrutura que é encontrada nesse projeto é o mapper dos DTOS da aplicação, esse componente é responsável por transformar a entidade, que deve ser conhecida apenas pela aplicação, em um modelo de dados que seja útil para o cliente sem que a estrutura da tabela seja exposta.

## Produto_api.DataBase

Essa biblioteca tem como finalidade prover as configurações necessárias para a utilização do banco de dados, assim como as migrations necessárias para estabelecer e popular a base de dados.

Para enfrentar o desafio proposto, utilizei a abordagem `code-first`, gerando inicialmente a entidade `Produto`,presento no projeto `Produto_api.Domain`, em seguida com a utilização do Entity Framework as tabelas foram criadas por meio de migrations.

A model que será usada para gerar a migration deve possuir algumas `DataAnnotations` para que o Entity Framework saiba como definir as tabelas e os campos que cada registro deve possuir, sendo assim devemos utilizar minimamente:

- `[Table("produtos")]`: Define que essa entidade será uma tabela e define o nome da tabela.
- `[Key]`: Define qual campo será a PK do registro.

Os comandos usados para gerar as migrations pode ser visto a seguir:

`dotnet ef migrations add PrimeiraMigration --startup-project ../Produtos_api/Produtos_api.csproj --project ../Produtos_api.Database/Produtos_api.DataBase.csproj`

e

`dotnet ef migrations add InserindoProdutosIniciaisMigration --startup-project ../Produtos_api/Produtos_api.csproj --project ../Produtos_api.Database/Produtos_api.DataBase.csproj`

Eles apresentam a estrutura de pastas pois as migrations foram criadas com base nas entidades presentes no projeto `Produto_api.Domain` porém o projeto que de fato utiliza o banco de dados (startup) é o projeto `Produto_api`.

Uma vez criadas, é necessário aplicar as migrations ao banco para que ele funcione corretamente. Portanto, toda vez que a pipeline for reiniciada, elas deverão ser aplicadas ou atualizadas. Essa tarefa pode ser observada na classe `Program.cs` no seguinte trecho de código:

```c#
using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProdutoDbContext>();
    context.Database.Migrate();
}
```
Nessa biblioteca encontramos o contexto que será utilizado pelo entity framework para utilizar o banco de dados, ele é criado na classe `ProdutoDbContext`, e é inserido como dependência (padrão de projeto: **Injeção de dependência**) através do `Program.cs` na instrução:

`builder.Services.AddDbContext<ProdutoDbContext>();`

Outro elemento presente nessa biblioteca é a implementação do `ProdutoRepository` (padrão de projeto: **Repository**), com a utilização dessa classe temos uma camada de abstração entre a lógica de negócios de uma aplicação e os detalhes de acesso a dados, dessa forma a responsabilidade é separada, facilitando manutenções, padronização da implementação em diversos pontos do código e facilitando a criação de testes. Esse repository é inserido como dependência através do `Program.cs` na instrução:
`builder.Services.AddTransient<IProdutoRepository, ProdutoRepository>();`

## Produto_api.Domain

Nessa biblioteca estão presentes as models e DTOS utilizados pela aplicação, também encontramos a definição da `Interface` que define o `Repository`.

Os comandos utilizados para gerar as `migrations` deve ser executado na raiz desse projeto, pois assim o utilitário do Entity Framework conseguirá definir quais models devem ser utilizadas.

A utilização de uma biblioteca própria para entidades permite que, em caso de necessidade de manutenção, o código seja fácil de entender e que todas as implementações sejam atualizadas de maneira homogênea.

## Produto_api.test

Esse projeto é responsável pela definição dos testes unitários e de integração da solução, utilizando `Xunit` e `Moq`.

Os teste unitários forma feitos em cima da controller principal do projeto `ProdutoController`, e estão presentes da classe `ProdutosControllerTest`, para que pudessem ser executados e definidos com clareza foi necessário a utilização de **Injeção de dependênci (DI)**, como podemos ver em objetos como `produtoRepository` e `_mapper`.

Os testes de integração, estão presentes na classe `ChamadaApiTest` e eles consistem basicamente em realizar chamadas **http** na api já construida, essa classe apresenta a necessidade da utilização de uma estrutura `WebApplicationFactory`, pois dessa forma o próprio projeto consegue criar um host da api durante o tempo de execução, para que isso funcione é necessário que a classe `Program.cs` se torne pública, naturalmente ela é definida como **internal**, para isso é necessário incluir em `Program.cs` a instrução:

`public partial class Program { } `

Para que os testes de integração ocorram foi necessário criar dois cenários onde a aplicação seria executada, em um deles `WebApplicationTest`, nós temos as definições do banco de dados original porém em memória e possiblitamos que a aplicação utilize os componentes originais para executar, dessa forma temos o comportamente esperado. No outro cenário ,`WebApplicationTestInvalido`, temos as mesmas definições de bancos de dados porém utilizamos **DI** para forçar erros no `Repository` e com isso testar o comportamente do **middleware** em cenários de **exception**, dessa forma podendo testar comportamentos adversos.

# .github/workflows

Apesar de não estar diretamente ligada ao projeto, essa estrutura esta presente pois é onde devemos criar os steps da **Pipeline CI/CD** do **Github Actions**.

A pipeline `Cobertura de teste e Build`, presente no script `ScriptBuildTest.yml` é configurada para acionar em cada **push** na **branch principal (main)**. 
No primeiro estágio, "**Compilar**", o código é clonado, o ambiente .NET é configurado, dependências são baixadas, e a compilação é executada,os artefatos são salvos. O segundo estágio, "**Testar**", é acionado após a compilação e realiza testes unitários usando os artefatos gerados anteriormente. Por fim, o estágio "**Gerar Assets**" depende dos estágios anteriores e baixa os artefatos da API, mantendo-os para possível deploy. 

## Dependências utilizadas:

- AutoMapper: Utilizada para mapeamento de DTOs
- Entity Framework Core: Gerenciamento e acesso ao banco de dados
- Entity Framework Core SQLite: Ferramentas utilitarias para conexão com SQLite
- Serilog: Utilitário para criação de logs
- Serilog.AspNetCore: Utilitários necessários para a configuração da bibiliteca Serilog em aplicações AspNet.
- Serilog.Sinks.Console: Conector para gerar log na console.
- Swagger: Utilizado para gerar live doc da aplicação e para auxiliar na execução de testes.
- MOQ: Utilitário para criação de objetos Mockados, facilitando criação de instância de alguns objetos e forçar comportamentos para testes mais específicos.

## SQLite

O banco de dados escolhido para essa solução foi o SQLite, devido a sua simplicidade, não necessitar de configurações para executar e ser multiplataforma, importante para execução dos testes de integração em **VMs** do **GitHub Actions**.