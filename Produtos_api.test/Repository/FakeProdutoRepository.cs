using System;
using System.ComponentModel;
using Produtos_api.DataBase;
using Produtos_api.Domain.Models;

namespace Produtos_api.test.Repository
{
    public class FakeProdutoRepository : IProdutoRepository
    {

        public List<Produto> Produtos { get; set; }

        public FakeProdutoRepository()
        {

            Produtos = new List<Produto>
        {
        new Produto() { ID = new Guid("8b182530-6ded-47e9-874e-ed451c842de3"), Estoque = 2, Nome = "Produto A", Valor = 25.2 },
        new Produto() { ID = new Guid("c0408ffa-ec92-420d-b6c5-6bd04a1c5058"), Estoque = 5, Nome = "Produto B", Valor = 30 },
        new Produto() { ID = new Guid("81dac77a-70af-4a62-803c-543365d56b1c"), Estoque = 4, Nome = "Produto C", Valor = 40.2 },
        new Produto() { ID = new Guid("0e315285-bcd2-48ae-b84f-eca618faca14"), Estoque = 1, Nome = "Produto D", Valor = 125.2 },
        new Produto() { ID = new Guid("2b2164a4-9ce1-4185-923a-f107143e045a"), Estoque = 0, Nome = "Produto E", Valor = 100 },
        };

        }

        public async Task<Produto> AdicionarProduto(Produto produto)
        {
            Produtos.Add(produto);
            return produto;
        }

        public async Task<Produto> AtualizarProduto(Guid id, string? nomeProduto, int? quantidadeEstoque, double? valorProduto)
        {
           Produto produto = Produtos.SingleOrDefault(x => x.ID == id);

            if (produto != null)
            {
                produto.Nome = nomeProduto ?? produto.Nome;
                produto.Valor = valorProduto ?? produto.Valor;
                produto.Estoque = quantidadeEstoque ?? produto.Estoque;
            }

            return produto;
        }

        public async Task<bool?> DeletarProduto(Guid id)
        {
            Produto produto = Produtos.Where(prod => prod.ID == id).SingleOrDefault();

            if (produto == null)
            {
                return null;
            }

            Produtos.Remove(produto);
            

            return true; 
        }

        public async Task<List<Produto>> RetornarListaProdutos(string? campo = null)
        {

            if (campo != null)
            {

                switch (campo)
                {
                    case "Nome":
                        return Produtos.OrderBy(prop => prop.Nome).ToList();
                    case "Estoque":
                        return Produtos.OrderBy(prop => prop.Estoque).ToList();
                    case "Valor":
                        return Produtos.OrderBy(prop => prop.Valor).ToList();
                }
            }
            else
            {
                return Produtos;
            }

            return Produtos;
        }

        public async Task<Produto> RetornarProdutoPorId(Guid id)
        {
            Produto produto = Produtos.SingleOrDefault(x => x.ID == id);
            return produto;
        }

        public async Task<Produto> RetornarProdutoPorNome(string nome)
        {
            Produto produto = Produtos.SingleOrDefault(x => x.Nome == nome);
            return produto;
        }
    }
}

