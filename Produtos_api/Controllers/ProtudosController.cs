using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Produtos_api.DBContext;
using Produtos_api.Models;

namespace Produtos_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : Controller
    {

        private readonly ProdutosContext produtosDbContext;

        public ProdutosController(ProdutosContext dbContext)
        {
            this.produtosDbContext = dbContext;
        }
       
        [HttpGet]
        public IActionResult Get()
        {
        
            List<Produto> produtos = produtosDbContext.Produtos;
            return Ok(produtos);
        }

        
        [HttpGet("{id}")]
        public IActionResult GetByID(Guid id)
        {
           
            Produto produto = produtosDbContext.Produtos.SingleOrDefault(pro => pro.ID == id);

            if(produto == null)
            {
                return NotFound("Não Foi encontrado o produto!");
            }
            return Ok(produto);
        }

       
        [HttpPost]
        public IActionResult Post([FromBody]Produto produto)
        {
           
            Produto produtoCadastro = new Produto(produto.Nome, produto.Estoque, produto.Valor);
            produtosDbContext.Produtos.Add(produtoCadastro);

            return Created($"Produto ID:{produtoCadastro.ID} criado com sucesso!", produtoCadastro);
        }

        
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]Produto produto)
        {
            
            Produto produtoAtualizado = produtosDbContext.Produtos.SingleOrDefault(pro => pro.ID == id);

            if(produtoAtualizado == null)
            {
                return NotFound("O produto não foi encontrado!");
            }

            produtoAtualizado.Nome = produto.Nome;
            produtoAtualizado.Valor = produto.Valor;
            produtoAtualizado.Estoque = produto.Estoque;


            return Accepted();
        }

      
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
          
            Produto produtoDeleteado = produtosDbContext.Produtos.SingleOrDefault(pro => pro.ID == id);

            if (produtoDeleteado == null)
            {
                return NotFound("O produto não foi encontrado!");
            }

            produtosDbContext.Produtos.Remove(produtoDeleteado);

            return NoContent();

        }
    }
}

