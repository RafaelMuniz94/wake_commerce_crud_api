using System;
using System.ComponentModel.DataAnnotations;

namespace Produtos_api.Application.VIewModels
{
	public class BuscarPorNomeViewModel
	{
        [Required(ErrorMessage = "Forneça um nome de produto válido!")]
        public string nomeProduto { get; set; }
    }
}

