using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using Produtos_api.Application.VIewModels;
using Produtos_api.Domain.Dtos;

namespace Produtos_api.test.IntegrationTest
{
    public class ChamadaApiTest : IClassFixture<WebApplicationFactory<Program>>
    {

        private WebApplicationFactory<Program> programFactory;
        private HttpClient client;
        private CriarProdutoViewModel criarProduto_completo;
        private AtualizarProdutoViewModel atualizarProduto_completo;
        public ChamadaApiTest(WebApplicationFactory<Program> factory)
        {
            programFactory = factory;
            client = programFactory.CreateClient();

            criarProduto_completo = new CriarProdutoViewModel()
            {
                nomeProduto = "Produto A",
                quantidadeEstoque = 15,
                valorProduto = 150.30
            };

            atualizarProduto_completo = new AtualizarProdutoViewModel()
            {
                nomeProduto = "Produto G",
                quantidadeEstoque = 14,
                valorProduto = 200
            };
        }

        #region Testes Positivos

        [Fact]
        public async Task Deve_Criar_Produto()
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(criarProduto_completo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync("/api/Produtos", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            string resposta = await response.Content.ReadAsStringAsync();

            ProdutoDTO produtoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(resposta);
            Assert.Equal(150.3, produtoDTO.valorProduto);
            Assert.Equal("Produto A", produtoDTO.nomeProduto);
            Assert.Equal(15, produtoDTO.quantidadeEstoque);
        }

        [Fact]
        public async Task Deve_Criar_Produto_Sem_Estoque()
        {
            CriarProdutoViewModel criarProduto_SemEstoque = criarProduto_completo;
            criarProduto_SemEstoque.quantidadeEstoque = null;
            StringContent content = new StringContent(JsonConvert.SerializeObject(criarProduto_SemEstoque));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync("/api/Produtos", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            string resposta = await response.Content.ReadAsStringAsync();

            ProdutoDTO produtoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(resposta);
            Assert.Equal(150.3, produtoDTO.valorProduto);
            Assert.Equal("Produto A", produtoDTO.nomeProduto);
            Assert.Equal(0, produtoDTO.quantidadeEstoque);
        }

        [Fact]
        public async Task Deve_Alterar_Produto_Completo()
        {

            StringContent contentCriar = new StringContent(JsonConvert.SerializeObject(criarProduto_completo));
            contentCriar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseCriar = await client.PostAsync("/api/Produtos", contentCriar);

            string respostaCriar = await responseCriar.Content.ReadAsStringAsync();

            ProdutoDTO produtoCriadoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(respostaCriar);
            Guid guidCriado = produtoCriadoDTO.id;

            StringContent contentAtualizar = new StringContent(JsonConvert.SerializeObject(atualizarProduto_completo));
            contentAtualizar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseAtualizar = await client.PutAsync($"/api/Produtos/{guidCriado}", contentAtualizar);

            string respostaAtualizar = await responseAtualizar.Content.ReadAsStringAsync();

            ProdutoDTO produtoAtualizadoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(respostaAtualizar);

            Assert.Equal(guidCriado, produtoAtualizadoDTO.id);
            Assert.Equal(200, produtoAtualizadoDTO.valorProduto);
            Assert.Equal("Produto G", produtoAtualizadoDTO.nomeProduto);
            Assert.Equal(14, produtoAtualizadoDTO.quantidadeEstoque);
        }

        [Fact]
        public async Task Deve_Alterar_Produto_Sem_Atualizar_Nome()
        {
            AtualizarProdutoViewModel atualizarProduto_SemNome = atualizarProduto_completo;
            atualizarProduto_SemNome.nomeProduto = null;


            StringContent contentCriar = new StringContent(JsonConvert.SerializeObject(criarProduto_completo));
            contentCriar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseCriar = await client.PostAsync("/api/Produtos", contentCriar);

            string respostaCriar = await responseCriar.Content.ReadAsStringAsync();

            ProdutoDTO produtoCriadoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(respostaCriar);
            Guid guidCriado = produtoCriadoDTO.id;

            StringContent contentAtualizar = new StringContent(JsonConvert.SerializeObject(atualizarProduto_SemNome));
            contentAtualizar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseAtualizar = await client.PutAsync($"/api/Produtos/{guidCriado}", contentAtualizar);

            string respostaAtualizar = await responseAtualizar.Content.ReadAsStringAsync();

            ProdutoDTO produtoAtualizadoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(respostaAtualizar);

            Assert.Equal(guidCriado, produtoAtualizadoDTO.id);
            Assert.Equal(200, produtoAtualizadoDTO.valorProduto);
            Assert.Equal("Produto A", produtoAtualizadoDTO.nomeProduto);
            Assert.Equal(14, produtoAtualizadoDTO.quantidadeEstoque);
        }

        [Fact]
        public async Task Deve_Alterar_Produto_Sem_Atualizar_Quantidade_Estoque()
        {
            AtualizarProdutoViewModel atualizarProduto_SemQuantidadeEstoque = atualizarProduto_completo;
            atualizarProduto_SemQuantidadeEstoque.quantidadeEstoque = null;


            StringContent contentCriar = new StringContent(JsonConvert.SerializeObject(criarProduto_completo));
            contentCriar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseCriar = await client.PostAsync("/api/Produtos", contentCriar);

            string respostaCriar = await responseCriar.Content.ReadAsStringAsync();

            ProdutoDTO produtoCriadoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(respostaCriar);
            Guid guidCriado = produtoCriadoDTO.id;

            StringContent contentAtualizar = new StringContent(JsonConvert.SerializeObject(atualizarProduto_SemQuantidadeEstoque));
            contentAtualizar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseAtualizar = await client.PutAsync($"/api/Produtos/{guidCriado}", contentAtualizar);

            string respostaAtualizar = await responseAtualizar.Content.ReadAsStringAsync();

            ProdutoDTO produtoAtualizadoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(respostaAtualizar);

            Assert.Equal(guidCriado, produtoAtualizadoDTO.id);
            Assert.Equal(200, produtoAtualizadoDTO.valorProduto);
            Assert.Equal("Produto G", produtoAtualizadoDTO.nomeProduto);
            Assert.Equal(15, produtoAtualizadoDTO.quantidadeEstoque);
        }


        [Fact]
        public async Task Deve_Alterar_Produto_Sem_Atualizar_Valor()
        {
            AtualizarProdutoViewModel atualizarProduto_SemValor = atualizarProduto_completo;
            atualizarProduto_SemValor.valorProduto = null;


            StringContent contentCriar = new StringContent(JsonConvert.SerializeObject(criarProduto_completo));
            contentCriar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseCriar = await client.PostAsync("/api/Produtos", contentCriar);

            string respostaCriar = await responseCriar.Content.ReadAsStringAsync();

            ProdutoDTO produtoCriadoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(respostaCriar);
            Guid guidCriado = produtoCriadoDTO.id;

            StringContent contentAtualizar = new StringContent(JsonConvert.SerializeObject(atualizarProduto_SemValor));
            contentAtualizar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseAtualizar = await client.PutAsync($"/api/Produtos/{guidCriado}", contentAtualizar);

            string respostaAtualizar = await responseAtualizar.Content.ReadAsStringAsync();

            ProdutoDTO produtoAtualizadoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(respostaAtualizar);

            Assert.Equal(guidCriado, produtoAtualizadoDTO.id);
            Assert.Equal(150.30, produtoAtualizadoDTO.valorProduto);
            Assert.Equal("Produto G", produtoAtualizadoDTO.nomeProduto);
            Assert.Equal(14, produtoAtualizadoDTO.quantidadeEstoque);
        }

        [Fact]
        public async Task Deve_Deletar_Produto()
        {
          
            StringContent contentCriar = new StringContent(JsonConvert.SerializeObject(criarProduto_completo));
            contentCriar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseCriar = await client.PostAsync("/api/Produtos", contentCriar);

            string respostaCriar = await responseCriar.Content.ReadAsStringAsync();

            ProdutoDTO produtoCriadoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(respostaCriar);
            Guid guidCriado = produtoCriadoDTO.id;

            var responseDeletar = await client.DeleteAsync($"/api/Produtos/{guidCriado}");

            Assert.Equal(HttpStatusCode.NoContent, responseDeletar.StatusCode);
        }

        [Fact]
        public async Task Deve_Deletar_Produto_Correto()
        {
            CriarProdutoViewModel produto = criarProduto_completo;

            StringContent contentCriar = new StringContent(JsonConvert.SerializeObject(produto));
            contentCriar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            await client.PostAsync("/api/Produtos", contentCriar);

            produto.nomeProduto = "Produto B";

            contentCriar = new StringContent(JsonConvert.SerializeObject(produto));
            contentCriar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseCriar = await client.PostAsync("/api/Produtos", contentCriar);

            string respostaCriar = await responseCriar.Content.ReadAsStringAsync();

            ProdutoDTO produtoCriadoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(respostaCriar);
            Guid guidASerDeletado = produtoCriadoDTO.id;

            produto.nomeProduto = "Produto C";

            contentCriar = new StringContent(JsonConvert.SerializeObject(produto));
            contentCriar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            await client.PostAsync("/api/Produtos", contentCriar);

            var responseDeletar = await client.DeleteAsync($"/api/Produtos/{guidASerDeletado}");

            Assert.Equal(HttpStatusCode.NoContent, responseDeletar.StatusCode);

            var responseBuscaByID = await client.GetAsync($"/api/Produtos/{guidASerDeletado}");
            string resposta = await responseBuscaByID.Content.ReadAsStringAsync();


            Assert.Equal(HttpStatusCode.NotFound, responseBuscaByID.StatusCode);
            Assert.Equal("O produto não foi encontrado!", resposta);
        }

        #endregion
    }
}

