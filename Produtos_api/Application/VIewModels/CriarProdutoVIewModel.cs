using System;
using System.ComponentModel.DataAnnotations;

namespace Produtos_api.Application.VIewModels
{
	public class CriarProdutoViewModel
	{
        [Required(ErrorMessage ="O produto deve possuir um nome!")]

		public string nomeProduto { get; set; }


        public int quantidadeEstoque { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$",ErrorMessage ="O preço deve ser definido com até duas casas decimais!")]
        [Range(0, 999999999.99)]
        public double valorProduto { get; set; }
    }
}

