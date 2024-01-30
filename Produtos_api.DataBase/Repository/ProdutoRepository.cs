using System;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Produtos_api.Domain.Models;

namespace Produtos_api.DataBase.Repository
{
	public class ProdutoRepository : IProdutoRepository
	{

        private readonly ProdutoDbContext produtoContext;
		public ProdutoRepository(ProdutoDbContext dbContext)
		{
            produtoContext = dbContext;
		}

        public async Task<Produto> AdicionarProduto(Produto produto)
        {
            try
            {
                await produtoContext.Produtos.AddAsync(produto);
                int resposta = await produtoContext.SaveChangesAsync();

                if (resposta > 0) // Se salvou mais de uma requisicao, sera possivel assumir que a operacao deu certo
                {

                    return produto;
                }
                else
                {
                    return null;
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Deu ruim aqui:{ex.Message}");
                Console.WriteLine(ex);
                throw ex;
            }

        }

        public async Task<Produto> AtualizarProduto(Guid id, string? nomeProduto, int? quantidadeEstoque, double? valorProduto)
        {
            Produto produto = await produtoContext.Produtos.SingleOrDefaultAsync(prod => prod.ID == id);

            if(produto != null)
            {
                produto.Nome = nomeProduto ?? produto.Nome;
                produto.Valor = valorProduto ?? produto.Valor;
                produto.Estoque = quantidadeEstoque ?? produto.Estoque;
                await produtoContext.SaveChangesAsync();
            }

            return produto;
        }

        public async Task<bool?> DeletarProduto(Guid id)
        {

            Produto produto = await produtoContext.Produtos.Where(prod => prod.ID == id).SingleOrDefaultAsync();

            if(produto == null)
            {
                return null;
            }

            produtoContext.Produtos.Remove(produto);
           int resposta =  await produtoContext.SaveChangesAsync();

            return resposta > 0; // Se salvou mais de uma requisicao, sera possivel assumir que a operacao deu certo
        }

        public async Task<List<Produto>> RetornarListaProdutos(string? campo=null)
        {
            List<Produto> listaProdutos = new List<Produto>();
            
            if(campo != null)
            {
                // Utilizando reflections para descobrir o campo que foi passado e utilizar para ordenacao
                // Em cenarios que a classe seja maior, utilizar essa tecnica pode reduzir a performance e nao ser tao interessante.

                PropertyDescriptor propriedadesProduto = TypeDescriptor.GetProperties(typeof(Produto)).Find(campo, true);
                listaProdutos = await produtoContext.Produtos.OrderBy(prop => propriedadesProduto.GetValue(prop)).ToListAsync();

                //switch (campo)
                //{
                //    case "Nome":
                //        listaProdutos = await produtoContext.Produtos.OrderBy(prop => prop.Nome).ToListAsync();
                //    break;
                //    case "Estoque":
                //        listaProdutos = await produtoContext.Produtos.OrderBy(prop => prop.Estoque).ToListAsync();
                //        break;
                //    case "Valor":
                //        listaProdutos = await produtoContext.Produtos.OrderBy(prop => prop.Valor).ToListAsync();
                //        break;
                //}
            }
            else
            {
                listaProdutos = await produtoContext.Produtos.ToListAsync();
            }


            return listaProdutos;
        }

        public async Task<Produto> RetornarProdutoPorNome(string nome)
        {
            Produto produto = await produtoContext.Produtos.SingleOrDefaultAsync(prod => prod.Nome== nome);

            return produto;
        }

        public async Task<Produto> RetornarProdutoPorId(Guid id)
        {
           Produto produto =  await produtoContext.Produtos.SingleOrDefaultAsync(prod => prod.ID == id);

            return produto;
        }
    }
}

