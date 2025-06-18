using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Producto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEsActivoProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsActivo",
                table: "Productos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsActivo",
                table: "Productos");
        }
    }
}
