using System;
using Produtos_api.Domain.Models;

namespace Produtos_api.test.Repository
{
    public class FakeProdutoRepositoryInvalido : IProdutoRepository
    {
		public FakeProdutoRepositoryInvalido() 
		{
		}

        Task<Produto> IProdutoRepository.AdicionarProduto(Produto produto)
        {
            throw new NotImplementedException();
        }

        Task<Produto> IProdutoRepository.AtualizarProduto(Guid id, string? nomeProduto, int? quantidadeEstoque, double? valorProduto)
        {
            throw new NotImplementedException();
        }

        Task<bool?> IProdutoRepository.DeletarProduto(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<List<Produto>> IProdutoRepository.RetornarListaProdutos(string? campo)
        {
            throw new NotImplementedException();
        }

        Task<Produto> IProdutoRepository.RetornarProdutoPorId(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<Produto> IProdutoRepository.RetornarProdutoPorNome(string nome)
        {
            throw new NotImplementedException();
        }
    }
}

