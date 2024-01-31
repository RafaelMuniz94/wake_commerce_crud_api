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

namespace Produtos_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {

        private readonly IProdutoRepository produtoRepository;
        private readonly IMapper _mapper;

        private readonly string mensagemProdutoNaoEncontrado = "O produto não foi encontrado!";

        public ProdutosController(IProdutoRepository repository,IMapper mapperInjected)
        {
            this.produtoRepository = repository;
            this._mapper = mapperInjected;
        }
       
        [HttpGet]
        public async Task<IActionResult> RetornarProdutos([FromQuery] OrdernarBuscaViewModel ordernarBuscaViewModel)
        {

            string? filtro = null;

           if(ordernarBuscaViewModel.FiltroCampo != null && ordernarBuscaViewModel.FiltroCampo != OrderByEnum.None)
            {
                filtro = ordernarBuscaViewModel.FiltroCampo.ToString();
            }


            List<Produto> produtos = await produtoRepository.RetornarListaProdutos(filtro);
            List<ProdutoDTO> produtoDto = _mapper.Map<List<ProdutoDTO>>(produtos);
            return Ok(produtoDto);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> RetornarProdutoPorID(Guid id)
        {
           
            Produto produto = await produtoRepository.RetornarProdutoPorId(id);

            if(produto == null)
            {
                return NotFound(mensagemProdutoNaoEncontrado);
            }

            ProdutoDTO produtoDto = _mapper.Map<ProdutoDTO>(produto);
            return Ok(produtoDto);
        }

        [HttpGet("PorNome")]
        public async Task<IActionResult> RetornarProdutoPorNome([FromQuery] BuscarPorNomeViewModel buscarPorNomeViewModel)
        {

            if (!ModelState.IsValid)
            {
                return RetornarBadRequestParaErrosDeValidacao();
            }

            Produto produto = await produtoRepository.RetornarProdutoPorNome(buscarPorNomeViewModel.nomeProduto);

            if (produto == null)
            {
                return NotFound(mensagemProdutoNaoEncontrado);
            }

            ProdutoDTO produtoDto = _mapper.Map<ProdutoDTO>(produto);
            return Ok(produtoDto);
        }


        [HttpPost]
        public async Task<IActionResult> CriarProduto([FromBody] CriarProdutoViewModel produto)
        {
            if (!ModelState.IsValid)
            {
                return RetornarBadRequestParaErrosDeValidacao();
            }
            Produto produtoCadastro = new Produto(produto.nomeProduto, produto.quantidadeEstoque, produto.valorProduto);
            produtoCadastro = await produtoRepository.AdicionarProduto(produtoCadastro);

            ProdutoDTO produtoDTO = _mapper.Map<ProdutoDTO>(produtoCadastro);

            return Created($"api/Produtos/{produtoDTO.id}", produtoDTO);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarProduto(Guid id, [FromBody] AtualizarProdutoViewModel produto)
        {

            if (!ModelState.IsValid)
            {
                return RetornarBadRequestParaErrosDeValidacao();
            }

            Produto produtoAtualizado = await produtoRepository.AtualizarProduto(id,produto.nomeProduto, produto.quantidadeEstoque, produto.valorProduto );

            if(produtoAtualizado == null)
            {
                return NotFound(mensagemProdutoNaoEncontrado);
            }


           
            ProdutoDTO produtoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);


            return Accepted(produtoDto);
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarProduto(Guid id)
        {

            bool? deletou = await produtoRepository.DeletarProduto(id);

            if(deletou == null)
            {
                return NotFound(mensagemProdutoNaoEncontrado);
            }

            return NoContent();

        }

        private IActionResult RetornarBadRequestParaErrosDeValidacao()
        {
            
           string errorMessages = string.Join("; ", ModelState.Values.SelectMany(model => model.Errors).Select(error => error.ErrorMessage));
                return BadRequest(errorMessages);
            
        }
    }
}

