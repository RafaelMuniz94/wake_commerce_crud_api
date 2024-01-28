using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Produtos_api.Application.VIewModels;
using Produtos_api.DBContext;
using Produtos_api.Domain.Dtos;
using Produtos_api.Domain.Models;

namespace Produtos_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {

        private readonly ProdutosContext produtosDbContext;
        private readonly IMapper _mapper;

        public ProdutosController(ProdutosContext dbContext,IMapper mapperInjected)
        {
            this.produtosDbContext = dbContext;
            this._mapper = mapperInjected;
        }
       
        [HttpGet]
        public IActionResult RetornarProdutos()
        {
        
            List<Produto> produtos = produtosDbContext.Produtos;
            List<ProdutoDTO> produtoDto = _mapper.Map<List<ProdutoDTO>>(produtos);
            return Ok(produtoDto);
        }

        
        [HttpGet("{id}")]
        public IActionResult RetornarProdutoPorID(Guid id)
        {
           
            Produto produto = produtosDbContext.Produtos.SingleOrDefault(pro => pro.ID == id);

            if(produto == null)
            {
                return NotFound("Não Foi encontrado o produto!");
            }

            ProdutoDTO produtoDto = _mapper.Map<ProdutoDTO>(produto);
            return Ok(produtoDto);
        }

       
        [HttpPost]
        public IActionResult CriarProduto([FromBody] CriarProdutoViewModel produto)
        {
            if (!ModelState.IsValid)
            {
                ValidarEntrada();
            }
            Produto produtoCadastro = new Produto(produto.nomeProduto, produto.quantidadeEstoque, produto.valorProduto);
            produtosDbContext.Produtos.Add(produtoCadastro);

            ProdutoDTO produtoDTO = _mapper.Map<ProdutoDTO>(produtoCadastro);

            return Created($"api/Produtos/{produtoDTO.id}", produtoDTO);
        }

        
        [HttpPut("{id}")]
        public IActionResult AtualizarProduto(Guid id, [FromBody] AtualizarProdutoViewModel produto)
        {

            if (!ModelState.IsValid)
            {
                ValidarEntrada();
            }

            Produto produtoAtualizado = produtosDbContext.Produtos.SingleOrDefault(pro => pro.ID == id);

            if(produtoAtualizado == null)
            {
                return NotFound("O produto não foi encontrado!");
            }

            produtoAtualizado.Nome = produto.nomeProduto ?? produtoAtualizado.Nome;
            produtoAtualizado.Valor = produto.valorProduto ?? produtoAtualizado.Valor;
            produtoAtualizado.Estoque = produto.quantidadeEstoque ?? produtoAtualizado.Estoque;


            return Accepted();
        }

      
        [HttpDelete("{id}")]
        public IActionResult DeletarProduto(Guid id)
        {
          
            Produto produtoDeleteado = produtosDbContext.Produtos.SingleOrDefault(pro => pro.ID == id);

            if (produtoDeleteado == null)
            {
                return NotFound("O produto não foi encontrado!");
            }

            produtosDbContext.Produtos.Remove(produtoDeleteado);

            return NoContent();

        }

        private IActionResult ValidarEntrada()
        {
            
           string errorMessages = string.Join("; ", ModelState.Values.SelectMany(model => model.Errors).Select(error => error.ErrorMessage));
                return BadRequest(errorMessages);
            
        }
    }
}

