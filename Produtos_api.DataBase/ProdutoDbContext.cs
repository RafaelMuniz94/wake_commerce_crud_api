using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Produtos_api.Domain.Models;

namespace Produtos_api.DataBase;

public class ProdutoDbContext : DbContext
{
    private IConfiguration configuracao;
    public DbSet<Produto> Produtos { get; set; }

    public ProdutoDbContext(IConfiguration config, DbContextOptions dbOptions) :base(dbOptions)
    {
        configuracao = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string tipoBase = configuracao["TipoBanco"];
        


        if (string.IsNullOrEmpty(tipoBase) || tipoBase.ToUpper() == "SQLITE")
        {
            optionsBuilder.UseSqlite(configuracao.GetConnectionString("BancoProdutos"));
            
        }
    }
}

