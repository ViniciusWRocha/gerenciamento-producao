using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciamentoProducaoo.Migrations
{
    /// <inheritdoc />
    public partial class CorrigindoChaveEstrangeiraProducao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producao_Usuario_UsuarioIdUsuario",
                table: "Producao");

            migrationBuilder.DropIndex(
                name: "IX_Producao_UsuarioIdUsuario",
                table: "Producao");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "Producao");

            migrationBuilder.CreateIndex(
                name: "IX_Producao_IdUsuario",
                table: "Producao",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Producao_Usuario_IdUsuario",
                table: "Producao",
                column: "IdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producao_Usuario_IdUsuario",
                table: "Producao");

            migrationBuilder.DropIndex(
                name: "IX_Producao_IdUsuario",
                table: "Producao");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdUsuario",
                table: "Producao",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Producao_UsuarioIdUsuario",
                table: "Producao",
                column: "UsuarioIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Producao_Usuario_UsuarioIdUsuario",
                table: "Producao",
                column: "UsuarioIdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario");
        }
    }
}
