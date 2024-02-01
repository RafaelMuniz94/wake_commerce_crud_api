using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Produtos_api.Application.VIewModels;
using Produtos_api.DataBase;
using Produtos_api.Domain.Dtos;
using Produtos_api.Domain.Models;
using Serilog;

namespace Produtos_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {

        private readonly IProdutoRepository produtoRepository;
        private readonly IMapper _mapper;

        private readonly string mensagemProdutonaoEncontrado = "O produto não foi encontrado!";

        public ProdutosController(IProdutoRepository repository,IMapper mapperInjected)
        {
            this.produtoRepository = repository;
            this._mapper = mapperInjected;
        }
       
        [HttpGet]
        public async Task<IActionResult> RetornarProdutos([FromQuery] OrdernarBuscaViewModel ordernarBuscaViewModel)
        {
            Log.Information("Iniciando Retorno de produtos");
            string? filtro = null;

           if(ordernarBuscaViewModel.FiltroCampo != null && ordernarBuscaViewModel.FiltroCampo != OrderByEnum.None)
            {
                filtro = ordernarBuscaViewModel.FiltroCampo.ToString();
                Log.Information($"Possui ordenacao, do tipo: {filtro}");
            }


            Log.Information($"Iniciando chamada ao banco de dados");
            List<Produto> produtos = await produtoRepository.RetornarListaProdutos(filtro);
            Log.Information($"Finalizada chamada ao banco de dados");
            Log.Information($"Iniciando mapeamento de DTO");
            List<ProdutoDTO> produtoDto = _mapper.Map<List<ProdutoDTO>>(produtos);
            Log.Information($"Finalizado mapeamento de DTO");
            return Ok(produtoDto);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> RetornarProdutoPorID(Guid id)
        {
            Log.Information("Iniciando Retorno de produto por ID");
            Log.Information($"Iniciando chamada ao banco de dados");
            Produto produto = await produtoRepository.RetornarProdutoPorId(id);
            Log.Information($"Finalizada chamada ao banco de dados");

            if (produto == null)
            {
                Log.Error($"Produto ID:{id}, nao encontrado!");
                return NotFound(mensagemProdutonaoEncontrado);
            }
            Log.Information($"Iniciando mapeamento de DTO");
            ProdutoDTO produtoDto = _mapper.Map<ProdutoDTO>(produto);
            Log.Information($"Finalizado mapeamento de DTO");
            return Ok(produtoDto);
        }

        [HttpGet("PorNome")]
        public async Task<IActionResult> RetornarProdutoPorNome([FromQuery] BuscarPorNomeViewModel buscarPorNomeViewModel)
        {
            Log.Information("Iniciando Retorno de produto por ID");
            
            if (!ModelState.IsValid)
            {
               
                return RetornarBadRequestParaErrosDeValidacao();
            }

            Log.Information($"Iniciando chamada ao banco de dados");
            Produto produto = await produtoRepository.RetornarProdutoPorNome(buscarPorNomeViewModel.nomeProduto);
            Log.Information($"Finalizada chamada ao banco de dados");

            if (produto == null)
            {
                Log.Error($"Produto Nome:{buscarPorNomeViewModel.nomeProduto}, nao encontrado!");
                return NotFound(mensagemProdutonaoEncontrado);
            }
            Log.Information($"Iniciando mapeamento de DTO");
            ProdutoDTO produtoDto = _mapper.Map<ProdutoDTO>(produto);
            Log.Information($"Finalizado mapeamento de DTO");
            return Ok(produtoDto);
        }


        [HttpPost]
        public async Task<IActionResult> CriarProduto([FromBody] CriarProdutoViewModel produto)
        {
            Log.Information("Iniciando Inclusao de produto");
            if (!ModelState.IsValid)
            {
                return RetornarBadRequestParaErrosDeValidacao();
            }
            Produto produtoCadastro = new Produto(produto.nomeProduto, produto.quantidadeEstoque, produto.valorProduto);
            Log.Information($"Iniciando chamada ao banco de dados");
            produtoCadastro = await produtoRepository.AdicionarProduto(produtoCadastro);
            Log.Information($"Finalizada chamada ao banco de dados");

            Log.Information($"Iniciando mapeamento de DTO");
            ProdutoDTO produtoDto = _mapper.Map<ProdutoDTO>(produtoCadastro);
            Log.Information($"Finalizado mapeamento de DTO");

            Log.Information($"Produto ID: {produtoDto.id} criado com sucesso!");

            return Created($"api/Produtos/{produtoDto.id}", produtoDto);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarProduto(Guid id, [FromBody] AtualizarProdutoViewModel produto)
        {
            Log.Information("Iniciando Atualizacao de produto");

            if (!ModelState.IsValid)
            {
                return RetornarBadRequestParaErrosDeValidacao();
            }

            Log.Information($"Iniciando chamada ao banco de dados");
            Produto produtoAtualizado = await produtoRepository.AtualizarProduto(id,produto.nomeProduto, produto.quantidadeEstoque, produto.valorProduto );
            Log.Information($"Finalizada chamada ao banco de dados");

            if (produtoAtualizado == null)
            {
                Log.Error($"Produto ID:{id}, nao encontrado!");
                return NotFound(mensagemProdutonaoEncontrado);
            }


            Log.Information($"Iniciando mapeamento de DTO");
            ProdutoDTO produtoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);
            Log.Information($"Finalizado mapeamento de DTO");


            Log.Information($"Produto ID: {produtoDto.id} atualizado com sucesso!");

            return Accepted(produtoDto);
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarProduto(Guid id)
        {
            Log.Information("Iniciando Remocao de produto");

            Log.Information($"Iniciando chamada ao banco de dados");
            bool? deletou = await produtoRepository.DeletarProduto(id);
            Log.Information($"Finalizada chamada ao banco de dados");

            if (deletou == null)
            {
                Log.Error($"Produto ID:{id}, nao encontrado!");
                return NotFound(mensagemProdutonaoEncontrado);
            }

            Log.Information($"Produto ID: {id} deletado com sucesso!");
            return NoContent();

        }

        private IActionResult RetornarBadRequestParaErrosDeValidacao()
        {
            
           string errorMessages = string.Join("; ", ModelState.Values.SelectMany(model => model.Errors).Select(error => error.ErrorMessage));
            Log.Error($"Erro ao validar modelState: {errorMessages}");
                return BadRequest(errorMessages);
            
        }
    }
}

