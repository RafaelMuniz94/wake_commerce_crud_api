using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Produtos_api.Application.Middlewares
{
	public class TratarErrorMiddleware
	{
		private readonly RequestDelegate proximaEtapa;

		public TratarErrorMiddleware(RequestDelegate next)
		{
			proximaEtapa = next;
		}

		public async Task Invoke(HttpContext contextoAplicacao)
		{
			try
			{
				await proximaEtapa(contextoAplicacao);
			}catch(Exception e)
			{
				
				var mensagemErro = JsonConvert.SerializeObject(new { error= "Nao foi possivel processar requisicao!" });
				contextoAplicacao.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                contextoAplicacao.Response.ContentType = "application/json";


                await contextoAplicacao.Response.WriteAsync(mensagemErro);
			}
		}
	}
}

