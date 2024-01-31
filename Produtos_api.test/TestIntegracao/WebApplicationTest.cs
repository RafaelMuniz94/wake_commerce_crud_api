using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Produtos_api.DataBase;

namespace Produtos_api.test.TestIntegracao
{
	// Criando um Factory customizado, dessa forma, sera possivel trocar o tipo de banco que utilizo para testes
	// Evitando que o banco esteja sujo quando o teste de integracao executar.
    // Essa classe ira utilizar reflecions para obter as configuracoes feitas na startup da api (Produtos_api/Program.cs)
	public class WebApplicationTest<TProgram>: WebApplicationFactory<TProgram> where TProgram : class
	{
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            //builder.ConfigureAppConfiguration(config =>
            //{
            //    config.AddInMemoryCollection(new Dictionary<string,string?>()
            //    {
            //        ["ConnectionStrings:BancoProdutos"] = "mode=memory;cache=shared"
            //    });
            //});
            
            builder.ConfigureServices(services =>
            {
            
                // As linhas abaixo irao limpar o contexto da startup e a conexao caso ela exista
                var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ProdutoDbContext>));
                services.Remove(dbContext);

                var dbConnection = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

                services.Remove(dbConnection);

                // Criando a conexao que sera utilizada apenas durante os testes de integracao
                services.AddSingleton<DbConnection>(containerAplicacao =>
                {
                    // Criando conexao em memoria
                    var conexao = new SqliteConnection("mode=memory;cache=shared");

                    return conexao;
                });

                // Inserindo conexao criada anteriormente no contexto
                services.AddDbContext<ProdutoDbContext>((containerAplicacao, options) => {
                    var conexao = containerAplicacao.GetRequiredService<DbConnection>();
                    options.UseSqlite(conexao);
                });

            
            });
            builder.UseEnvironment("Development");
        }
    }
}

