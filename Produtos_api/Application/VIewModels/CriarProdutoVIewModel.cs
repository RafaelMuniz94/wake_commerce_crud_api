using System;
using System.ComponentModel.DataAnnotations;


namespace Produtos_api.Application.VIewModels
{
	public class CriarProdutoViewModel
	{
        [Required(ErrorMessage ="O produto deve possuir um nome!")]
		public string nomeProduto { get; set; }


        public int? quantidadeEstoque { get; set; }

        [Required(ErrorMessage = "O produto deve possuir um valor!")]
        [Range(0, 999999999.99,ErrorMessage = "O valor do produto deve ser positivo!")]
        public double valorProduto { get; set; }
    }
}

