using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Produtos_api.Application.VIewModels
{
    public enum OrderByEnum
    {
        [Display(Name = "Nome")]
        Nome,
        [Display(Name = "Valor")]
        Valor,
        [Display(Name = "Estoque")]
        Estoque,
        [Display(Name = "Nenhum")]
        None
    }

    public class OrdernarBuscaViewModel
	{
        [EnumDataType(typeof(OrderByEnum))]
        public OrderByEnum? FiltroCampo { get; set; } = OrderByEnum.None;
    }
}

