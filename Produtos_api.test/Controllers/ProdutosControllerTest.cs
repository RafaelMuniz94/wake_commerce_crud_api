using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Reflection.Metadata;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Produtos_api.Application.Mappers;
using Produtos_api.Application.VIewModels;
using Produtos_api.Controllers;
using Produtos_api.DataBase;
using Produtos_api.DataBase.Repository;
using Produtos_api.Domain.Dtos;
using Produtos_api.Domain.Models;
using Produtos_api.test.Repository;

namespace Produtos_api.test;

public class ProdutosControllerTest
{
    private readonly IProdutoRepository produtoRepository;
    private CriarProdutoViewModel criarProduto_Completo;
    private AtualizarProdutoViewModel atualizarProduto_completo;
    private OrdernarBuscaViewModel OrdernarBuscaViewModel_vazio;
    private ProdutosController controller_valido;
    private IMapper _mapper;

    public ProdutosControllerTest()
    {

        produtoRepository = new FakeProdutoRepository();


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

        OrdernarBuscaViewModel_vazio = new OrdernarBuscaViewModel
        {
            FiltroCampo = null
        };

        controller_valido = new ProdutosController(produtoRepository, _mapper);
    }

    #region Testes Positivos
    [Fact]
    public async void Deve_Retornar_Lista_De_Produtos_Sem_Ordenar()
    {

        var resultado = await controller_valido.RetornarProdutos(OrdernarBuscaViewModel_vazio);

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
    public async void Deve_Retornar_Produto_Especifico_Quando_Receber_ID()
    {
        var resultado = await controller_valido.RetornarProdutoPorID(new Guid("81dac77a-70af-4a62-803c-543365d56b1c"));

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
    public async void Deve_Criar_Produto_Quando_Receber_Todos_Campos()
    {
        var resultado = await controller_valido.CriarProduto(criarProduto_Completo);

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
    public async void Deve_Criar_Produto_Quando_Nao_Receber_Campo_Quantidade_Estoque()
    {
        CriarProdutoViewModel criar_Produto_SemQuantidadeEstoque = criarProduto_Completo;

        criar_Produto_SemQuantidadeEstoque.quantidadeEstoque = null;

        var resultado = await controller_valido.CriarProduto(criar_Produto_SemQuantidadeEstoque);

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
    public async void Deve_Atualizar_Produto_Quando_Receber_Todos_Campos()
    {
        Guid guidProdutoAlterado = new Guid("2b2164a4-9ce1-4185-923a-f107143e045a");
        var resultado = await controller_valido.AtualizarProduto(guidProdutoAlterado, atualizarProduto_completo);

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
    public async void Deve_Atualizar_Produto_Quando_Nao_Receber_Campo_Nome()
    {
        Guid guidProdutoAlterado = new Guid("8b182530-6ded-47e9-874e-ed451c842de3");

        //Criando instancia para garantir que a original nao seja alterada
        AtualizarProdutoViewModel atualizarProduto_SemNome = atualizarProduto_completo;
        //Removendo campo nome para que o teste ocorra corretamente
        atualizarProduto_SemNome.nomeProduto = null;


        var resultado = await controller_valido.AtualizarProduto(guidProdutoAlterado, atualizarProduto_SemNome);

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
    public async void Deve_Atualizar_Produto_Quando_Nao_Receber_Campo_Quantidade_Estoque()
    {
        Guid guidProdutoAlterado = new Guid("c0408ffa-ec92-420d-b6c5-6bd04a1c5058");

        //Criando instancia para garantir que a original nao seja alterada
        AtualizarProdutoViewModel atualizarProduto_SemQuantidadeEstoque = atualizarProduto_completo;
        //Removendo campo nome para que o teste ocorra corretamente
        atualizarProduto_SemQuantidadeEstoque.quantidadeEstoque = null;


        var resultado = await controller_valido.AtualizarProduto(guidProdutoAlterado, atualizarProduto_SemQuantidadeEstoque);

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
    public async void Deve_Atualizar_Produto_Quando_Nao_Receber_Campo_Valor()
    {
        Guid guidProdutoAlterado = new Guid("81dac77a-70af-4a62-803c-543365d56b1c");

        //Criando instancia para garantir que a original nao seja alterada
        AtualizarProdutoViewModel atualizarProduto_SemValor = atualizarProduto_completo;
        //Removendo campo nome para que o teste ocorra corretamente
        atualizarProduto_SemValor.valorProduto = null;


        var resultado = await controller_valido.AtualizarProduto(guidProdutoAlterado, atualizarProduto_SemValor);

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
    public async void Deve_Deletar_Quando_Receber_ID()
    {
        Guid guidProdutoDeletado = new Guid("81dac77a-70af-4a62-803c-543365d56b1c");

        var resultadoBuscaListaInicial = await controller_valido.RetornarProdutos(OrdernarBuscaViewModel_vazio) as OkObjectResult;

        var listaInicial = resultadoBuscaListaInicial.Value as List<ProdutoDTO>;

        int quantidadeInicial = listaInicial.Count();

        var resultado = await controller_valido.DeletarProduto(guidProdutoDeletado);

        Assert.NotNull(resultado);

        //Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
        Assert.IsType<NoContentResult>(resultado);

        NoContentResult objetoRetornado = resultado as NoContentResult;

        Assert.NotNull(objetoRetornado);

        var resultadoBuscaListaFinal = await controller_valido.RetornarProdutos(OrdernarBuscaViewModel_vazio) as OkObjectResult;

        var listaFinal = resultadoBuscaListaFinal.Value as List<ProdutoDTO>;

        int quantidadeFinal = listaFinal.Count();

        Assert.NotEqual(quantidadeInicial, quantidadeFinal);
        Assert.Equal(quantidadeInicial - 1, quantidadeFinal);

        // Validando se o produto correto foi removido

        var resultadoBuscaPorID = await controller_valido.RetornarProdutoPorID(new Guid("81dac77a-70af-4a62-803c-543365d56b1c"));

        //Validando se o objeto recebido é do tipo correto, garantindo que o status code definido foi respeitado
        Assert.IsType<NotFoundObjectResult>(resultadoBuscaPorID);

        Assert.NotNull(resultadoBuscaPorID);

        NotFoundObjectResult objetoRetornadoBuscaPorID = resultadoBuscaPorID as NotFoundObjectResult;

        Assert.Equal("O produto não foi encontrado!", objetoRetornadoBuscaPorID.Value);

    }

    [Fact]
    public async void Deve_Retornar_Produto_Por_Nome_Quando_Valido()
    {
        // Arrange
        var buscarPorNomeViewModel = new BuscarPorNomeViewModel
        {
            nomeProduto = "Produto C"
        };

        // Act
        var resultado = await controller_valido.RetornarProdutoPorNome(buscarPorNomeViewModel);

        // Assert
        Assert.IsType<OkObjectResult>(resultado);

        OkObjectResult objetoRetornado = resultado as OkObjectResult;

        Assert.NotNull(objetoRetornado);

        // Validar se o objeto apresenta o DTO correto
        Assert.IsType<ProdutoDTO>(objetoRetornado.Value);
        var produto = objetoRetornado.Value as ProdutoDTO;

        Assert.NotNull(produto);

        // Validar se os valores estão corretos
        Assert.Equal(new Guid("81dac77a-70af-4a62-803c-543365d56b1c"), produto.id);
        Assert.Equal(4, produto.quantidadeEstoque);
        Assert.Equal("Produto C", produto.nomeProduto);
        Assert.Equal(40.2, produto.valorProduto);
    }

    [Fact]
    public async void Deve_Retornar_Lista_De_Produtos_Ordenada_Por_Nome()
    {
        var ordernarBuscaViewModel = new OrdernarBuscaViewModel
        {
            FiltroCampo = OrderByEnum.Nome
        };

        var resultado = await controller_valido.RetornarProdutos(ordernarBuscaViewModel);

        Assert.IsType<OkObjectResult>(resultado);

        OkObjectResult objetoRetornado = resultado as OkObjectResult;

        Assert.NotNull(objetoRetornado);

        Assert.IsType<List<ProdutoDTO>>(objetoRetornado.Value);
        var listaOrdenada = (objetoRetornado.Value as List<ProdutoDTO>).ToList();

        Assert.Equal("Produto A", listaOrdenada[0].nomeProduto);
        Assert.Equal("Produto B", listaOrdenada[1].nomeProduto);
        Assert.Equal("Produto C", listaOrdenada[2].nomeProduto);
        Assert.Equal("Produto D", listaOrdenada[3].nomeProduto);
        Assert.Equal("Produto E", listaOrdenada[4].nomeProduto);
    }

    [Fact]
    public async void Deve_Retornar_Lista_De_Produtos_Ordenada_Por_Valor()
    {
        var ordernarBuscaViewModel = new OrdernarBuscaViewModel
        {
            FiltroCampo = OrderByEnum.Valor
        };

        var resultado = await controller_valido.RetornarProdutos(ordernarBuscaViewModel);

        Assert.IsType<OkObjectResult>(resultado);

        OkObjectResult objetoRetornado = resultado as OkObjectResult;

        Assert.NotNull(objetoRetornado);

        Assert.IsType<List<ProdutoDTO>>(objetoRetornado.Value);
        var listaOrdenada = (objetoRetornado.Value as List<ProdutoDTO>).ToList();

        Assert.Equal(25.2, listaOrdenada[0].valorProduto);
        Assert.Equal(30.0, listaOrdenada[1].valorProduto);
        Assert.Equal(40.2, listaOrdenada[2].valorProduto);
        Assert.Equal(100, listaOrdenada[3].valorProduto);
        Assert.Equal(125.2, listaOrdenada[4].valorProduto);
    }

    [Fact]
    public async void Deve_Retornar_Lista_De_Produtos_Ordenada_Por_Estoque()
    {
        var ordernarBuscaViewModel = new OrdernarBuscaViewModel
        {
            FiltroCampo = OrderByEnum.Estoque
        };

        var resultado = await controller_valido.RetornarProdutos(ordernarBuscaViewModel);

        Assert.IsType<OkObjectResult>(resultado);

        OkObjectResult objetoRetornado = resultado as OkObjectResult;

        Assert.NotNull(objetoRetornado);

        Assert.IsType<List<ProdutoDTO>>(objetoRetornado.Value);
        var listaOrdenada = (objetoRetornado.Value as List<ProdutoDTO>).ToList();

        Assert.Equal(0, listaOrdenada[0].quantidadeEstoque);
        Assert.Equal(1, listaOrdenada[1].quantidadeEstoque);
        Assert.Equal(2, listaOrdenada[2].quantidadeEstoque);
        Assert.Equal(4, listaOrdenada[3].quantidadeEstoque);
        Assert.Equal(5, listaOrdenada[4].quantidadeEstoque);
    }

    #endregion

    #region Testes Negativos

    //[Fact]
    //public async void Deve_Retornar_BadRequest_Busca_Por_Nome_Invalido()
    //{
    //    // Arrange
    //    var buscarPorNomeViewModelNegativo = new BuscarPorNomeViewModel
    //    {
    //        nomeProduto = null
    //    };

    //    // Act
    //    var resultado = await controller_valido.RetornarProdutoPorNome(buscarPorNomeViewModelNegativo);

    //    // Assert
    //    Assert.IsType<BadRequestObjectResult>(resultado);

    //    BadRequestObjectResult objetoRetornado = resultado as BadRequestObjectResult;

    //    Assert.NotNull(objetoRetornado);

    //    // Validar se o objeto apresenta os erros de validação corretos
    //    Assert.IsType<SerializableError>(objetoRetornado.Value);
    //    var erros = objetoRetornado.Value as SerializableError;

    //    Assert.NotNull(erros);
    //    Assert.True(erros.ContainsKey("nomeProduto"));
    //    Assert.Equal(new[] { "O nome do produto é obrigatório." }, erros["nomeProduto"] as string[]);
    //}


    #endregion
}