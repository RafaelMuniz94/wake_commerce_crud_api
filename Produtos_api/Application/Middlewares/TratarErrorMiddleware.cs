using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Produtos_api.Controllers;
using Serilog;
using Serilog.Context;

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
            using (LogContext.PushProperty("RequestId", Guid.NewGuid()))
            {
                try
                {

                    Log.Information($"Iniciando Processamento!");
                    await proximaEtapa(contextoAplicacao);
                    Log.Information($"Processamento finalizado com sucesso!");

                }
                catch (Exception e)
                {
                    Log.Error($"Processamento nao foi finalizado devido a Exception: {e.Message}");
                    var mensagemErro = JsonConvert.SerializeObject(new { error = "Nao foi possivel processar requisicao!" });
                    contextoAplicacao.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    contextoAplicacao.Response.ContentType = "application/json";


                    await contextoAplicacao.Response.WriteAsync(mensagemErro);
                }
            }
        }
    }
}

