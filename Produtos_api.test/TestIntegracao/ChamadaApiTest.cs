using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Produtos_api.Application.VIewModels;
using Produtos_api.DataBase;
using Produtos_api.Domain.Dtos;
using Produtos_api.test.TestIntegracao;

namespace Produtos_api.test.IntegrationTest
{
    public class ChamadaApiTest : IClassFixture<WebApplicationTest<Program>>
    {

        private WebApplicationTest<Program> programFactory;
        private HttpClient client;
        private CriarProdutoViewModel criarProduto_completo;
        private AtualizarProdutoViewModel atualizarProduto_completo;
        public ChamadaApiTest(WebApplicationTest<Program> factory)
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

        [Fact]
        public async Task Deve_Retornar_Lista_Produtos()
        {
            CriarProdutoViewModel produto = criarProduto_completo;

            StringContent contentCriar = new StringContent(JsonConvert.SerializeObject(produto));
            contentCriar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            await client.PostAsync("/api/Produtos", contentCriar);

            var responseBusca = await client.GetAsync("/api/Produtos/");
            

            Assert.Equal(HttpStatusCode.OK,responseBusca.StatusCode);

            string resposta = await responseBusca.Content.ReadAsStringAsync();

            List<ProdutoDTO> listaRetornarda = JsonConvert.DeserializeObject<List<ProdutoDTO>>(resposta);

            Assert.Equal(7, listaRetornarda.Count());

            Assert.NotEqual(listaRetornarda.FirstOrDefault().id, listaRetornarda.LastOrDefault().id);
        }

        [Fact]
        public async Task Deve_Retornar_Apenas_Um_Produto_Quando_Receber_ID()
        {
            var responseBusca = await client.GetAsync("/api/Produtos/");

            Assert.Equal(HttpStatusCode.OK, responseBusca.StatusCode);

            string resposta = await responseBusca.Content.ReadAsStringAsync();

            List<ProdutoDTO> listaRetornarda = JsonConvert.DeserializeObject<List<ProdutoDTO>>(resposta);

            ProdutoDTO primeiroProdutoBuscaGeral = listaRetornarda.FirstOrDefault();

            var responseBuscaByID = await client.GetAsync($"/api/Produtos/{primeiroProdutoBuscaGeral.id}");

            string respostaBuscaByID = await responseBuscaByID.Content.ReadAsStringAsync();

            ProdutoDTO produtoEspecifico = JsonConvert.DeserializeObject<ProdutoDTO>(respostaBuscaByID);

            Assert.NotNull(produtoEspecifico);


            Assert.NotEqual(produtoEspecifico.id,listaRetornarda.LastOrDefault().id);
            Assert.Equal(produtoEspecifico.id, primeiroProdutoBuscaGeral.id);
            Assert.Equal(produtoEspecifico.nomeProduto, primeiroProdutoBuscaGeral.nomeProduto);
            Assert.Equal(produtoEspecifico.valorProduto, primeiroProdutoBuscaGeral.valorProduto);
            Assert.Equal(produtoEspecifico.quantidadeEstoque, primeiroProdutoBuscaGeral.quantidadeEstoque);

        }

        #endregion

        #region Testes Negativos

        [Fact]
        public async Task Nao_Deve_Criar_Produto_Caso_Valor_Seja_Negativo()
        {
            CriarProdutoViewModel criarProduto_ValorNegativo = criarProduto_completo;
            criarProduto_ValorNegativo.valorProduto = -1;


            StringContent content = new StringContent(JsonConvert.SerializeObject(criarProduto_ValorNegativo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync("/api/Produtos", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string resposta = await response.Content.ReadAsStringAsync();
                        
            Assert.Equal("O valor do produto deve ser positivo!", resposta);

        }

        [Fact]
        public async Task Nao_Deve_Atualizar_Produto_Caso_Valor_Seja_Negativo()
        {

            StringContent contentCriar = new StringContent(JsonConvert.SerializeObject(criarProduto_completo));
            contentCriar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseCriar = await client.PostAsync("/api/Produtos", contentCriar);

            string respostaCriar = await responseCriar.Content.ReadAsStringAsync();

            ProdutoDTO produtoCriadoDTO = JsonConvert.DeserializeObject<ProdutoDTO>(respostaCriar);
            Guid guidCriado = produtoCriadoDTO.id;


            AtualizarProdutoViewModel atualizarProduto_ValorNegativo = atualizarProduto_completo;
            atualizarProduto_ValorNegativo.valorProduto = -1;


            StringContent contentAtualizar = new StringContent(JsonConvert.SerializeObject(atualizarProduto_ValorNegativo));
            contentAtualizar.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseAtualizar = await client.PutAsync($"/api/Produtos/{guidCriado}", contentAtualizar);

            Assert.Equal(HttpStatusCode.BadRequest, responseAtualizar.StatusCode);

            string resposta = await responseAtualizar.Content.ReadAsStringAsync();

            Assert.Equal("O valor do produto deve ser positivo!", resposta);

        }



        #endregion

    }
}

