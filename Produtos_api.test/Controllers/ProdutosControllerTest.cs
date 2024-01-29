using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Produtos_api.Application.Mappers;
using Produtos_api.Application.VIewModels;
using Produtos_api.Controllers;
using Produtos_api.DBContext;
using Produtos_api.Domain.Dtos;
using Produtos_api.Domain.Models;

namespace Produtos_api.test;

public class ProdutosControllerTest
{
    private ProdutosContext produtosDbContext_Valido;
    private CriarProdutoViewModel criarProduto_Completo;
    private AtualizarProdutoViewModel atualizarProduto_completo;
    ProdutosController controller_valido;
    private IMapper _mapper;

    public ProdutosControllerTest()
    {
        produtosDbContext_Valido = new Mock<ProdutosContext>().Object;

        produtosDbContext_Valido.Produtos.Add(new Produto() { ID = new Guid("8b182530-6ded-47e9-874e-ed451c842de3"), Estoque = 2, Nome = "Produto A", Valor = 25.2 });
        produtosDbContext_Valido.Produtos.Add(new Produto() { ID = new Guid("c0408ffa-ec92-420d-b6c5-6bd04a1c5058"), Estoque = 5, Nome = "Produto B", Valor = 30 });
        produtosDbContext_Valido.Produtos.Add(new Produto() { ID = new Guid("81dac77a-70af-4a62-803c-543365d56b1c"), Estoque = 4, Nome = "Produto C", Valor = 40.2 });
        produtosDbContext_Valido.Produtos.Add(new Produto() { ID = new Guid("0e315285-bcd2-48ae-b84f-eca618faca14"), Estoque = 1, Nome = "Produto D", Valor = 125.2 });
        produtosDbContext_Valido.Produtos.Add(new Produto() { ID = new Guid("2b2164a4-9ce1-4185-923a-f107143e045a"), Estoque = 0, Nome = "Produto E", Valor = 100 });


        var config = new MapperConfiguration(config => config.AddProfile(new ProdutoMapper()));
        _mapper = new Mapper(config);

        criarProduto_Completo = new CriarProdutoViewModel()
        {
            nomeProduto = "Produto F",
            quantidadeEstoque = 15,
            valorProduto = 150.30
        };

        atualizarProduto_completo = new AtualizarProdutoViewModel()
        {
            nomeProduto = "Produto G",
            quantidadeEstoque = 14,
            valorProduto = 200
        };

        controller_valido = new ProdutosController(produtosDbContext_Valido, _mapper);
    }

    #region Testes Positivos
    [Fact]
    public void Deve_Retornar_Lista_De_Produtos()
    {

        var resultado = controller_valido.RetornarProdutos();

        //Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
        Assert.IsType<OkObjectResult>(resultado);

        Assert.NotNull(resultado);

        OkObjectResult objetoRetornado = resultado as OkObjectResult;

        Assert.NotNull(objetoRetornado);

        //Validando se o objeto apresenta o dto correto
        Assert.IsType<List<ProdutoDTO>>(objetoRetornado.Value);

        var lista = objetoRetornado.Value as List<ProdutoDTO>;

        Assert.NotNull(lista);

        //Validando se a Lista possui a quantidade correta de dados
        Assert.Equal(5, lista.Count());

        //Validando se os valores estao corretos
        Assert.Equal(new Guid("8b182530-6ded-47e9-874e-ed451c842de3"), lista[0].id);
        Assert.Equal(5, lista[1].quantidadeEstoque);
        Assert.Equal("Produto C", lista[2].nomeProduto);
        Assert.Equal(125.2, lista[3].valorProduto);
        Assert.Equal(new Guid("2b2164a4-9ce1-4185-923a-f107143e045a"), lista[4].id);



    }

    [Fact]
    public void Deve_Retornar_Produto_Especifico_Quando_Receber_ID()
    {
        var resultado = controller_valido.RetornarProdutoPorID(new Guid("81dac77a-70af-4a62-803c-543365d56b1c"));

        //Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
        Assert.IsType<OkObjectResult>(resultado);

        Assert.NotNull(resultado);

        OkObjectResult objetoRetornado = resultado as OkObjectResult;

        Assert.NotNull(objetoRetornado);

        //Validando se o objeto apresenta o dto correto
        Assert.IsType<ProdutoDTO>(objetoRetornado.Value);

        var produto = objetoRetornado.Value as ProdutoDTO;


        //Validando se os valores estao corretos
        Assert.Equal(new Guid("81dac77a-70af-4a62-803c-543365d56b1c"), produto.id);
        Assert.Equal(4, produto.quantidadeEstoque);
        Assert.Equal("Produto C", produto.nomeProduto);
        Assert.Equal(40.2, produto.valorProduto);
    }

    [Fact]
    public void Deve_Criar_Produto_Quando_Receber_Todos_Campos()
    {
        var resultado = controller_valido.CriarProduto(criarProduto_Completo);

        Assert.NotNull(resultado);

        //Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
        Assert.IsType<CreatedResult>(resultado);

        CreatedResult objetoRetornado = resultado as CreatedResult;

        Assert.NotNull(objetoRetornado);

        //Validando se o objeto apresenta o dto correto
        Assert.IsType<ProdutoDTO>(objetoRetornado.Value);
        var produto = objetoRetornado.Value as ProdutoDTO;

        Assert.NotNull(produto);

        //Validando se os valores estao corretos
        Assert.NotEqual(Guid.Empty, produto.id);
        Assert.Equal(15, produto.quantidadeEstoque);
        Assert.Equal("Produto F", produto.nomeProduto);
        Assert.Equal(150.3, produto.valorProduto);

    }

    [Fact]
    public void Deve_Criar_Produto_Quando_Nao_Receber_Campo_Quantidade_Estoque()
    {
        CriarProdutoViewModel criar_Produto_SemQuantidadeEstoque = criarProduto_Completo;

        criar_Produto_SemQuantidadeEstoque.quantidadeEstoque = null;

        var resultado = controller_valido.CriarProduto(criar_Produto_SemQuantidadeEstoque);

        Assert.NotNull(resultado);

        //Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
        Assert.IsType<CreatedResult>(resultado);

        CreatedResult objetoRetornado = resultado as CreatedResult;

        Assert.NotNull(objetoRetornado);

        //Validando se o objeto apresenta o dto correto
        Assert.IsType<ProdutoDTO>(objetoRetornado.Value);
        var produto = objetoRetornado.Value as ProdutoDTO;

        Assert.NotNull(produto);

        //Validando se os valores estao corretos
        Assert.NotEqual(Guid.Empty, produto.id);
        Assert.Equal(0, produto.quantidadeEstoque);
        Assert.Equal("Produto F", produto.nomeProduto);
        Assert.Equal(150.3, produto.valorProduto);

    }

    [Fact]
    public void Deve_Atualizar_Produto_Quando_Receber_Todos_Campos()
    {
        Guid guidProdutoAlterado = new Guid("2b2164a4-9ce1-4185-923a-f107143e045a");
        var resultado = controller_valido.AtualizarProduto(guidProdutoAlterado, atualizarProduto_completo);

        Assert.NotNull(resultado);

        //Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
        Assert.IsType<AcceptedResult>(resultado);

        AcceptedResult objetoRetornado = resultado as AcceptedResult;

        Assert.NotNull(objetoRetornado);

        //Validando se o objeto apresenta o dto correto
        Assert.IsType<ProdutoDTO>(objetoRetornado.Value);
        var produto = objetoRetornado.Value as ProdutoDTO;

        Assert.NotNull(produto);

        //Validando se os valores estao corretos
        Assert.Equal(guidProdutoAlterado, produto.id); // Deve permarnecer o mesmo
        Assert.Equal(14, produto.quantidadeEstoque);
        Assert.Equal("Produto G", produto.nomeProduto);
        Assert.Equal(200, produto.valorProduto);
    }

    [Fact]
    public void Deve_Atualizar_Produto_Quando_Nao_Receber_Campo_Nome()
    {
        Guid guidProdutoAlterado = new Guid("8b182530-6ded-47e9-874e-ed451c842de3");

        //Criando instancia para garantir que a original nao seja alterada
        AtualizarProdutoViewModel atualizarProduto_SemNome = atualizarProduto_completo;
        //Removendo campo nome para que o teste ocorra corretamente
        atualizarProduto_SemNome.nomeProduto = null;


        var resultado = controller_valido.AtualizarProduto(guidProdutoAlterado, atualizarProduto_SemNome);

        Assert.NotNull(resultado);

        //Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
        Assert.IsType<AcceptedResult>(resultado);

        AcceptedResult objetoRetornado = resultado as AcceptedResult;

        Assert.NotNull(objetoRetornado);

        //Validando se o objeto apresenta o dto correto
        Assert.IsType<ProdutoDTO>(objetoRetornado.Value);
        var produto = objetoRetornado.Value as ProdutoDTO;

        Assert.NotNull(produto);

        //Validando se os valores estao corretos
        Assert.Equal(guidProdutoAlterado, produto.id); // Deve permarnecer o mesmo
        Assert.Equal(14, produto.quantidadeEstoque);
        Assert.Equal("Produto A", produto.nomeProduto);
        Assert.Equal(200, produto.valorProduto);
    }

    [Fact]
    public void Deve_Atualizar_Produto_Quando_Nao_Receber_Campo_Quantidade_Estoque()
    {
        Guid guidProdutoAlterado = new Guid("c0408ffa-ec92-420d-b6c5-6bd04a1c5058");

        //Criando instancia para garantir que a original nao seja alterada
        AtualizarProdutoViewModel atualizarProduto_SemQuantidadeEstoque = atualizarProduto_completo;
        //Removendo campo nome para que o teste ocorra corretamente
        atualizarProduto_SemQuantidadeEstoque.quantidadeEstoque = null;


        var resultado = controller_valido.AtualizarProduto(guidProdutoAlterado, atualizarProduto_SemQuantidadeEstoque);

        Assert.NotNull(resultado);

        //Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
        Assert.IsType<AcceptedResult>(resultado);

        AcceptedResult objetoRetornado = resultado as AcceptedResult;

        Assert.NotNull(objetoRetornado);

        //Validando se o objeto apresenta o dto correto
        Assert.IsType<ProdutoDTO>(objetoRetornado.Value);
        var produto = objetoRetornado.Value as ProdutoDTO;

        Assert.NotNull(produto);

        //Validando se os valores estao corretos
        Assert.Equal(guidProdutoAlterado, produto.id); // Deve permarnecer o mesmo
        Assert.Equal(5, produto.quantidadeEstoque);
        Assert.Equal("Produto G", produto.nomeProduto);
        Assert.Equal(200, produto.valorProduto);
    }

    [Fact]
    public void Deve_Atualizar_Produto_Quando_Nao_Receber_Campo_Valor()
    {
        Guid guidProdutoAlterado = new Guid("81dac77a-70af-4a62-803c-543365d56b1c");

        //Criando instancia para garantir que a original nao seja alterada
        AtualizarProdutoViewModel atualizarProduto_SemValor = atualizarProduto_completo;
        //Removendo campo nome para que o teste ocorra corretamente
        atualizarProduto_SemValor.valorProduto = null;


        var resultado = controller_valido.AtualizarProduto(guidProdutoAlterado, atualizarProduto_SemValor);

        Assert.NotNull(resultado);

        //Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
        Assert.IsType<AcceptedResult>(resultado);

        AcceptedResult objetoRetornado = resultado as AcceptedResult;

        Assert.NotNull(objetoRetornado);

        //Validando se o objeto apresenta o dto correto
        Assert.IsType<ProdutoDTO>(objetoRetornado.Value);
        var produto = objetoRetornado.Value as ProdutoDTO;

        Assert.NotNull(produto);

        //Validando se os valores estao corretos
        Assert.Equal(guidProdutoAlterado, produto.id); // Deve permarnecer o mesmo
        Assert.Equal(14, produto.quantidadeEstoque);
        Assert.Equal("Produto G", produto.nomeProduto);
        Assert.Equal(40.2, produto.valorProduto);
    }

    [Fact]
    public void Deve_Deletar_Quando_Receber_ID()
    {
        Guid guidProdutoDeletado = new Guid("81dac77a-70af-4a62-803c-543365d56b1c");

        int quantidadeInicial = produtosDbContext_Valido.Produtos.Count();

        var resultado = controller_valido.DeletarProduto(guidProdutoDeletado);

        Assert.NotNull(resultado);

        //Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
        Assert.IsType<NoContentResult>(resultado);

        NoContentResult objetoRetornado = resultado as NoContentResult;

        Assert.NotNull(objetoRetornado);


        int quantidadeFinal = produtosDbContext_Valido.Produtos.Count();

        Assert.NotEqual(quantidadeInicial, quantidadeFinal);
        Assert.Equal(quantidadeInicial - 1, quantidadeFinal);

        // Validando se o produto correto foi removido

        var resultadoBuscaPorID = controller_valido.RetornarProdutoPorID(new Guid("81dac77a-70af-4a62-803c-543365d56b1c"));

        //Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
        Assert.IsType<NotFoundObjectResult>(resultadoBuscaPorID);

        Assert.NotNull(resultadoBuscaPorID);

        NotFoundObjectResult objetoRetornadoBuscaPorID = resultadoBuscaPorID as NotFoundObjectResult;

        Assert.Equal("O produto não foi encontrado!", objetoRetornadoBuscaPorID.Value);

    }

    #endregion

    #region Testes Negativos

    //[Fact]
    //public void Nao_Deve_Criar_Produto_Quando_Nao_Receber_Nome()
    //{
    //    CriarProdutoViewModel criar_Produto_SemNome = criarProduto_Completo;
    //    criar_Produto_SemNome.nomeProduto = null;

    //    var x = controller_valido.TryValidateModel(criar_Produto_SemNome);
    //    Assert.NotNull(x);

    //    //controller_valido.ModelState.Clear();
    //    //controller_valido.ModelState.AddModelError("nomeProduto", "Required");

    //    //var modelState = controller_valido.ModelState;

    //    //var resultado = controller_valido.CriarProduto(criar_Produto_SemNome);

    //    //Assert.NotNull(resultado);

    //    ////Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
    //    //Assert.IsType<BadRequestObjectResult>(resultado);

    //    //BadRequestObjectResult objetoRetornado = resultado as BadRequestObjectResult;

    //    //Assert.NotNull(objetoRetornado);
    //    //Assert.Equal(400, objetoRetornado.StatusCode);
    //    //Assert.Equal("Required", objetoRetornado.Value);

    //    //Assert.True(modelState.Count == 1);
    //    //Assert.True(modelState == 1);
    //}



    #endregion
}