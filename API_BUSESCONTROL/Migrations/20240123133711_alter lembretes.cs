using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_BUSESCONTROL.Migrations
{
    public partial class alterlembretes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemetenteId",
                table: "Lembrete",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemetenteId",
                table: "Lembrete");
        }
    }
}
