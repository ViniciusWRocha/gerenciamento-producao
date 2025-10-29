using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciamentoProducaoo.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleCalendarEventId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleCalendarEventId",
                table: "Obra",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleCalendarEventId",
                table: "Obra");
        }
    }
}
