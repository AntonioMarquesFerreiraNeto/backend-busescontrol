using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_BUSESCONTROL.Migrations
{
    public partial class altercargostocargo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cargos",
                table: "Funcionario",
                newName: "Cargo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cargo",
                table: "Funcionario",
                newName: "Cargos");
        }
    }
}
