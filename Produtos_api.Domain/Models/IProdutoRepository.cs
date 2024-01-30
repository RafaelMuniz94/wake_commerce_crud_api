using System;
namespace Produtos_api.Domain.Models
{
	public interface IProdutoRepository
	{
		Task<Produto> AdicionarProduto(Produto produto);
		Task<List<Produto>> RetornarListaProdutos(string? campo=null);
        Task<Produto> RetornarProdutoPorId(Guid id);
        Task<Produto> RetornarProdutoPorNome(string nome);
        Task<bool?> DeletarProduto(Guid id);
        Task<Produto> AtualizarProduto(Guid id, string? nomeProduto, int? quantidadeEstoque, double? valorProduto);


	}
}

