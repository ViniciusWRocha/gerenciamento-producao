using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciamentoProducaoo.Migrations
{
    /// <inheritdoc />
    public partial class usuarioImg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetaMensal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetaMensal",
                columns: table => new
                {
                    IdMetaMensal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    MetaAtingida = table.Column<bool>(type: "bit", nullable: false),
                    MetaPesoKg = table.Column<float>(type: "real", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PercentualAtingido = table.Column<float>(type: "real", nullable: false),
                    PesoProduzido = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaMensal", x => x.IdMetaMensal);
                    table.ForeignKey(
                        name: "FK_MetaMensal_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetaMensal_IdUsuario",
                table: "MetaMensal",
                column: "IdUsuario");
        }
    }
}
