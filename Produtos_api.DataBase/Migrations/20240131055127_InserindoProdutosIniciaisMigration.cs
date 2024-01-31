using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Produtos_api.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class InserindoProdutosIniciaisMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(table: "produtos",
            columns: new[] { "ID", "Nome", "Estoque", "Valor" },
            values: new object[,]
            {

                    {new Guid("e8265f6b-df44-44f6-a06b-c9274d8b73a1"),"Produto 1",1, 19.99 },
                    {new Guid("67e0c51a-142e-4ff0-8e37-fd9c5e7320c7"),"Produto 2",23, 189.00 },
                    {new Guid("9b4180d4-2040-4e6b-a9f4-2a95d8a09f4d"),"Produto 3",44, 329.99 },
                    {new Guid("1d5f1832-968e-42a0-aa7c-7ccfe09c21c6"),"Produto 4",120, 4.99 },
                    {new Guid("25ec5a8f-e55b-4a44-bbe7-42cf7a498990"),"Produto 5",113, 55.50 },

            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "produtos",
                keyColumn: "ID",
                keyValues: null
                );
        }
    }
}
