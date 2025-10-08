using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciamentoProducaoo.Migrations
{
    /// <inheritdoc />
    public partial class CorrigindoChaveEstrangeiraCaixilho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Caixilho_FamiliaCaixilho_FamiliaCaixilhoIdFamiliaCaixilho",
                table: "Caixilho");

            migrationBuilder.DropForeignKey(
                name: "FK_Caixilho_TipoCaixilho_TipoCaixilhoIdTipoCaixilho",
                table: "Caixilho");

            migrationBuilder.DropIndex(
                name: "IX_Caixilho_FamiliaCaixilhoIdFamiliaCaixilho",
                table: "Caixilho");

            migrationBuilder.DropIndex(
                name: "IX_Caixilho_TipoCaixilhoIdTipoCaixilho",
                table: "Caixilho");

            migrationBuilder.DropColumn(
                name: "FamiliaCaixilhoIdFamiliaCaixilho",
                table: "Caixilho");

            migrationBuilder.DropColumn(
                name: "TipoCaixilhoIdTipoCaixilho",
                table: "Caixilho");

            migrationBuilder.CreateIndex(
                name: "IX_Caixilho_IdFamiliaCaixilho",
                table: "Caixilho",
                column: "IdFamiliaCaixilho");

            migrationBuilder.CreateIndex(
                name: "IX_Caixilho_IdTipoCaixilho",
                table: "Caixilho",
                column: "IdTipoCaixilho");

            migrationBuilder.AddForeignKey(
                name: "FK_Caixilho_FamiliaCaixilho_IdFamiliaCaixilho",
                table: "Caixilho",
                column: "IdFamiliaCaixilho",
                principalTable: "FamiliaCaixilho",
                principalColumn: "IdFamiliaCaixilho",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Caixilho_TipoCaixilho_IdTipoCaixilho",
                table: "Caixilho",
                column: "IdTipoCaixilho",
                principalTable: "TipoCaixilho",
                principalColumn: "IdTipoCaixilho",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Caixilho_FamiliaCaixilho_IdFamiliaCaixilho",
                table: "Caixilho");

            migrationBuilder.DropForeignKey(
                name: "FK_Caixilho_TipoCaixilho_IdTipoCaixilho",
                table: "Caixilho");

            migrationBuilder.DropIndex(
                name: "IX_Caixilho_IdFamiliaCaixilho",
                table: "Caixilho");

            migrationBuilder.DropIndex(
                name: "IX_Caixilho_IdTipoCaixilho",
                table: "Caixilho");

            migrationBuilder.AddColumn<int>(
                name: "FamiliaCaixilhoIdFamiliaCaixilho",
                table: "Caixilho",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoCaixilhoIdTipoCaixilho",
                table: "Caixilho",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Caixilho_FamiliaCaixilhoIdFamiliaCaixilho",
                table: "Caixilho",
                column: "FamiliaCaixilhoIdFamiliaCaixilho");

            migrationBuilder.CreateIndex(
                name: "IX_Caixilho_TipoCaixilhoIdTipoCaixilho",
                table: "Caixilho",
                column: "TipoCaixilhoIdTipoCaixilho");

            migrationBuilder.AddForeignKey(
                name: "FK_Caixilho_FamiliaCaixilho_FamiliaCaixilhoIdFamiliaCaixilho",
                table: "Caixilho",
                column: "FamiliaCaixilhoIdFamiliaCaixilho",
                principalTable: "FamiliaCaixilho",
                principalColumn: "IdFamiliaCaixilho");

            migrationBuilder.AddForeignKey(
                name: "FK_Caixilho_TipoCaixilho_TipoCaixilhoIdTipoCaixilho",
                table: "Caixilho",
                column: "TipoCaixilhoIdTipoCaixilho",
                principalTable: "TipoCaixilho",
                principalColumn: "IdTipoCaixilho");
        }
    }
}
