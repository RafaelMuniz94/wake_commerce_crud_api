﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Produtos_api.Domain.Models
{
    [Table("produtos")]
	public class Produto
	{
		public Produto()
		{

		}

        public Produto(string nome, int estoque, double valor)
        {
            ID = Guid.NewGuid();
            Nome = nome;
            Estoque = estoque;
            Valor = valor;
        }

        public Produto(string nome, int? estoque, double valor)
        {
            ID = Guid.NewGuid();
            Nome = nome;
            Estoque = estoque ?? 0;
            Valor = valor;
        }

        [Key]
        public Guid ID { get; set; }
        public string Nome{ get; set; }
        public int Estoque { get; set; }
        public double Valor { get; set; }
 
    }
}

