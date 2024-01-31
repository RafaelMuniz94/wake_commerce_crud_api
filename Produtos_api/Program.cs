using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produtos_api.Application.Mappers;
using Produtos_api.DataBase;
using Produtos_api.DataBase.Repository;
using Produtos_api.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddTransient<IProdutoRepository, ProdutoRepository>();
builder.Services.AddDbContext<ProdutoDbContext>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
}); // Essa opcao evita que o .net execute um envio de erro automatico em caso de modelstate invalida, devido a Json em formato errado.

builder.Services.AddSwaggerGen();

//builder.Services.AddSingleton<ProdutosContext>(); // Adicionado objeto de contexto de banco de dados para servir como banco em memoria para desenvolvimento inicial
builder.Services.AddAutoMapper(typeof(ProdutoMapper)); // Configura processo de mapeamento do DTO.
var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProdutoDbContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program { } // Essa linha deve ser inserida para que uma instancia seja criada pela classe de Teste de Integracao.