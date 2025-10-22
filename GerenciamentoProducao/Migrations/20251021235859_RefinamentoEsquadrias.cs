using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciamentoProducaoo.Migrations
{
    /// <inheritdoc />
    public partial class RefinamentoEsquadrias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bandeira",
                table: "Obra",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataConclusao",
                table: "Obra",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicio",
                table: "Obra",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataTermino",
                table: "Obra",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Obra",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PercentualConclusao",
                table: "Obra",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PesoFinal",
                table: "Obra",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PesoProduzido",
                table: "Obra",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Prioridade",
                table: "Obra",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatusObra",
                table: "Obra",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataLiberacao",
                table: "Caixilho",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Liberado",
                table: "Caixilho",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Caixilho",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusProducao",
                table: "Caixilho",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MetaMensal",
                columns: table => new
                {
                    IdMetaMensal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    MetaPeso = table.Column<float>(type: "real", nullable: false),
                    PesoProduzido = table.Column<float>(type: "real", nullable: false),
                    PercentualAtingido = table.Column<float>(type: "real", nullable: false),
                    MetaAtingida = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "RelatorioProducao",
                columns: table => new
                {
                    IdRelatorio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataRelatorio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    PesoTotalProduzido = table.Column<float>(type: "real", nullable: false),
                    TotalCaixilhosProduzidos = table.Column<int>(type: "int", nullable: false),
                    TotalFamiliasProduzidas = table.Column<int>(type: "int", nullable: false),
                    EficienciaProducao = table.Column<float>(type: "real", nullable: false),
                    TempoMedioProducao = table.Column<float>(type: "real", nullable: false),
                    StatusMeta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatorioProducao", x => x.IdRelatorio);
                    table.ForeignKey(
                        name: "FK_RelatorioProducao_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetaMensal_IdUsuario",
                table: "MetaMensal",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_RelatorioProducao_IdUsuario",
                table: "RelatorioProducao",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetaMensal");

            migrationBuilder.DropTable(
                name: "RelatorioProducao");

            migrationBuilder.DropColumn(
                name: "Bandeira",
                table: "Obra");

            migrationBuilder.DropColumn(
                name: "DataConclusao",
                table: "Obra");

            migrationBuilder.DropColumn(
                name: "DataInicio",
                table: "Obra");

            migrationBuilder.DropColumn(
                name: "DataTermino",
                table: "Obra");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Obra");

            migrationBuilder.DropColumn(
                name: "PercentualConclusao",
                table: "Obra");

            migrationBuilder.DropColumn(
                name: "PesoFinal",
                table: "Obra");

            migrationBuilder.DropColumn(
                name: "PesoProduzido",
                table: "Obra");

            migrationBuilder.DropColumn(
                name: "Prioridade",
                table: "Obra");

            migrationBuilder.DropColumn(
                name: "StatusObra",
                table: "Obra");

            migrationBuilder.DropColumn(
                name: "DataLiberacao",
                table: "Caixilho");

            migrationBuilder.DropColumn(
                name: "Liberado",
                table: "Caixilho");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Caixilho");

            migrationBuilder.DropColumn(
                name: "StatusProducao",
                table: "Caixilho");
        }
    }
}
