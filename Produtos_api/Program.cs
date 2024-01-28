using Produtos_api.Application.Mappers;
using Produtos_api.DBContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ProdutosContext>(); // Adicionado objeto de contexto de banco de dados para servir como banco em memoria para desenvolvimento inicial
builder.Services.AddAutoMapper(typeof(ProdutoMapper)); // Configura processo de mapeamento do DTO.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

