using System;
using System.ComponentModel.DataAnnotations;


namespace Produtos_api.Application.VIewModels
{
	public class AtualizarProdutoViewModel
	{
		public string? nomeProduto { get; set; }
        public int? quantidadeEstoque { get; set; }

        
        [Range(0, 999999999.99, ErrorMessage = "O valor do produto deve ser positivo!")]
        public double? valorProduto { get; set; }
    }
}

