using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciamentoProducaoo.Migrations
{
    /// <inheritdoc />
    public partial class imgObras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagemObraPath",
                table: "Obra",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagemObraPath",
                table: "Obra");
        }
    }
}
