using System;
using Produtos_api.Models;

namespace Produtos_api.DBContext
{
	public class ProdutosContext
	{
		public List<Produto> Produtos { get; set; }

		public ProdutosContext()
		{
            Produtos = new List<Produto>();
		}


	}
}

