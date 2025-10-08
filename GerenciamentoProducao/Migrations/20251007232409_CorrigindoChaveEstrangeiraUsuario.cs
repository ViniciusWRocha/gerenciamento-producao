using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciamentoProducaoo.Migrations
{
    /// <inheritdoc />
    public partial class CorrigindoChaveEstrangeiraUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_TipoUsuario_TipoUsuarioIdTipoUsuario",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_TipoUsuarioIdTipoUsuario",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TipoUsuarioIdTipoUsuario",
                table: "Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdTipoUsuario",
                table: "Usuario",
                column: "IdTipoUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_TipoUsuario_IdTipoUsuario",
                table: "Usuario",
                column: "IdTipoUsuario",
                principalTable: "TipoUsuario",
                principalColumn: "IdTipoUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_TipoUsuario_IdTipoUsuario",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_IdTipoUsuario",
                table: "Usuario");

            migrationBuilder.AddColumn<int>(
                name: "TipoUsuarioIdTipoUsuario",
                table: "Usuario",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_TipoUsuarioIdTipoUsuario",
                table: "Usuario",
                column: "TipoUsuarioIdTipoUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_TipoUsuario_TipoUsuarioIdTipoUsuario",
                table: "Usuario",
                column: "TipoUsuarioIdTipoUsuario",
                principalTable: "TipoUsuario",
                principalColumn: "IdTipoUsuario");
        }
    }
}
