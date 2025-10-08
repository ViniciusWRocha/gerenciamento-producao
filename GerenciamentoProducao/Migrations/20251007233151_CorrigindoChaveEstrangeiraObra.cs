using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciamentoProducaoo.Migrations
{
    /// <inheritdoc />
    public partial class CorrigindoChaveEstrangeiraObra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obra_Usuario_UsuarioIdUsuario",
                table: "Obra");

            migrationBuilder.DropIndex(
                name: "IX_Obra_UsuarioIdUsuario",
                table: "Obra");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "Obra");

            migrationBuilder.CreateIndex(
                name: "IX_Obra_IdUsuario",
                table: "Obra",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Obra_Usuario_IdUsuario",
                table: "Obra",
                column: "IdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obra_Usuario_IdUsuario",
                table: "Obra");

            migrationBuilder.DropIndex(
                name: "IX_Obra_IdUsuario",
                table: "Obra");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdUsuario",
                table: "Obra",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Obra_UsuarioIdUsuario",
                table: "Obra",
                column: "UsuarioIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Obra_Usuario_UsuarioIdUsuario",
                table: "Obra",
                column: "UsuarioIdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario");
        }
    }
}
