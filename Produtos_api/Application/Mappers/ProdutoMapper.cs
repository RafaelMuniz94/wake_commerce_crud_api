using System;
using AutoMapper;
using Produtos_api.Domain.Dtos;
using Produtos_api.Domain.Models;

namespace Produtos_api.Application.Mappers
{
	public class ProdutoMapper : Profile
	{
		public ProdutoMapper()
		{
			CreateMap<Produto, ProdutoDTO>()
				.ForMember(destino => destino.id, member => member.MapFrom(origem => origem.ID))
				.ForMember(destino => destino.valorProduto, member => member.MapFrom(origem => origem.Valor))
				.ForMember(destino => destino.nomeProduto, member=> member.MapFrom(origem => origem.Nome))
				.ForMember(destino => destino.quantidadeEstoque, member=> member.MapFrom(origem => origem.Estoque));
        }
	}
}

