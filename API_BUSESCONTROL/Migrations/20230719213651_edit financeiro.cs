using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_BUSESCONTROL.Migrations
{
    public partial class editfinanceiro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FornecedorFisicoId",
                table: "Financeiro");

            migrationBuilder.DropColumn(
                name: "FornecedorJuridicoId",
                table: "Financeiro");

            migrationBuilder.RenameColumn(
                name: "ValorTotalPagoCliente",
                table: "Financeiro",
                newName: "ValorTotalPago");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValorTotalPago",
                table: "Financeiro",
                newName: "ValorTotalPagoCliente");

            migrationBuilder.AddColumn<int>(
                name: "FornecedorFisicoId",
                table: "Financeiro",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FornecedorJuridicoId",
                table: "Financeiro",
                type: "int",
                nullable: true);
        }
    }
}
