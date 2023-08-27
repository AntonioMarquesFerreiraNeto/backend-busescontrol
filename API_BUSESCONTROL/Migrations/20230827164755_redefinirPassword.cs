using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_BUSESCONTROL.Migrations
{
    public partial class redefinirPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChaveRedefinition",
                table: "Funcionario",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChaveRedefinition",
                table: "Funcionario");
        }
    }
}
