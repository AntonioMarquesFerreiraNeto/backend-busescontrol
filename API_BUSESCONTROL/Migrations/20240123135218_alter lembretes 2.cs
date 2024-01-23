using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_BUSESCONTROL.Migrations
{
    public partial class alterlembretes2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Lembrete_RemetenteId",
                table: "Lembrete",
                column: "RemetenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lembrete_Funcionario_RemetenteId",
                table: "Lembrete",
                column: "RemetenteId",
                principalTable: "Funcionario",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lembrete_Funcionario_RemetenteId",
                table: "Lembrete");

            migrationBuilder.DropIndex(
                name: "IX_Lembrete_RemetenteId",
                table: "Lembrete");
        }
    }
}
