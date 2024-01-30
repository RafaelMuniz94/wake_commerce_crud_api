using System;
namespace Produtos_api.Domain.Dtos
{
	public class ProdutoDTO
	{
		public ProdutoDTO()
		{
		}

        public Guid id { get; set; }
        public string nomeProduto { get; set; }
        public int quantidadeEstoque { get; set; }
        public double valorProduto { get; set; }
    }
}

