using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_BUSESCONTROL.Migrations
{
    public partial class finishtabledb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rescisao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Multa = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    ValorPagoContrato = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    ContratoId = table.Column<int>(type: "int", nullable: true),
                    PessoaFisicaId = table.Column<int>(type: "int", nullable: true),
                    PessoaJuridicaId = table.Column<int>(type: "int", nullable: true),
                    DataRescisao = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rescisao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rescisao_Cliente_PessoaFisicaId",
                        column: x => x.PessoaFisicaId,
                        principalTable: "Cliente",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rescisao_Cliente_PessoaJuridicaId",
                        column: x => x.PessoaJuridicaId,
                        principalTable: "Cliente",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rescisao_Contrato_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contrato",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Rescisao_ContratoId",
                table: "Rescisao",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Rescisao_PessoaFisicaId",
                table: "Rescisao",
                column: "PessoaFisicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rescisao_PessoaJuridicaId",
                table: "Rescisao",
                column: "PessoaJuridicaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rescisao");
        }
    }
}
