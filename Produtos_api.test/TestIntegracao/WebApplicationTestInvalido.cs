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
using Produtos_api.DataBase.Repository;
using Produtos_api.Domain.Models;
using Produtos_api.test.Repository;

namespace Produtos_api.test.TestIntegracao
{

    public class WebApplicationTestInvalido<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {


            builder.ConfigureServices(services =>
            {

                // As linhas abaixo irao limpar o contexto da startup e a conexao caso ela exista
                var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ProdutoDbContext>));
                services.Remove(dbContext);

                var dbConnection = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

                services.Remove(dbConnection);

                var repositoryContext = services.SingleOrDefault(d => d.ServiceType ==
                typeof(IProdutoRepository));

                services.Remove(repositoryContext);

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

                services.AddTransient<IProdutoRepository, FakeProdutoRepositoryInvalido>();

            });
            builder.UseEnvironment("Development");
        }
    }
}

