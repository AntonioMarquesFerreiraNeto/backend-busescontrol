using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_BUSESCONTROL.Migrations
{
    public partial class motoristassubstitutosedit2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubContratoMotoristas_Contrato_ContratoId",
                table: "SubContratoMotoristas");

            migrationBuilder.DropForeignKey(
                name: "FK_SubContratoMotoristas_Funcionario_FuncionarioId",
                table: "SubContratoMotoristas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubContratoMotoristas",
                table: "SubContratoMotoristas");

            migrationBuilder.RenameTable(
                name: "SubContratoMotoristas",
                newName: "SubContratoMotorista");

            migrationBuilder.RenameIndex(
                name: "IX_SubContratoMotoristas_FuncionarioId",
                table: "SubContratoMotorista",
                newName: "IX_SubContratoMotorista_FuncionarioId");

            migrationBuilder.RenameIndex(
                name: "IX_SubContratoMotoristas_ContratoId",
                table: "SubContratoMotorista",
                newName: "IX_SubContratoMotorista_ContratoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubContratoMotorista",
                table: "SubContratoMotorista",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubContratoMotorista_Contrato_ContratoId",
                table: "SubContratoMotorista",
                column: "ContratoId",
                principalTable: "Contrato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubContratoMotorista_Funcionario_FuncionarioId",
                table: "SubContratoMotorista",
                column: "FuncionarioId",
                principalTable: "Funcionario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubContratoMotorista_Contrato_ContratoId",
                table: "SubContratoMotorista");

            migrationBuilder.DropForeignKey(
                name: "FK_SubContratoMotorista_Funcionario_FuncionarioId",
                table: "SubContratoMotorista");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubContratoMotorista",
                table: "SubContratoMotorista");

            migrationBuilder.RenameTable(
                name: "SubContratoMotorista",
                newName: "SubContratoMotoristas");

            migrationBuilder.RenameIndex(
                name: "IX_SubContratoMotorista_FuncionarioId",
                table: "SubContratoMotoristas",
                newName: "IX_SubContratoMotoristas_FuncionarioId");

            migrationBuilder.RenameIndex(
                name: "IX_SubContratoMotorista_ContratoId",
                table: "SubContratoMotoristas",
                newName: "IX_SubContratoMotoristas_ContratoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubContratoMotoristas",
                table: "SubContratoMotoristas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubContratoMotoristas_Contrato_ContratoId",
                table: "SubContratoMotoristas",
                column: "ContratoId",
                principalTable: "Contrato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubContratoMotoristas_Funcionario_FuncionarioId",
                table: "SubContratoMotoristas",
                column: "FuncionarioId",
                principalTable: "Funcionario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
