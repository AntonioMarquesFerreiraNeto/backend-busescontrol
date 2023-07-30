using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_BUSESCONTROL.Migrations
{
    public partial class financeiro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Financeiro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContratoId = table.Column<int>(type: "int", nullable: true),
                    PessoaJuridicaId = table.Column<int>(type: "int", nullable: true),
                    PessoaFisicaId = table.Column<int>(type: "int", nullable: true),
                    FornecedorFisicoId = table.Column<int>(type: "int", nullable: true),
                    FornecedorJuridicoId = table.Column<int>(type: "int", nullable: true),
                    FornecedorId = table.Column<int>(type: "int", nullable: true),
                    DataVencimento = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ValorParcelaDR = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    ValorTotDR = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    ValorTotalPagoCliente = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    ValorTotTaxaJurosPaga = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    DataEmissao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    QtParcelas = table.Column<int>(type: "int", nullable: true),
                    TypeEfetuacao = table.Column<int>(type: "int", nullable: false),
                    DespesaReceita = table.Column<int>(type: "int", nullable: false),
                    Pagament = table.Column<int>(type: "int", nullable: false),
                    FinanceiroStatus = table.Column<int>(type: "int", nullable: false),
                    Detalhamento = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Financeiro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Financeiro_Cliente_PessoaFisicaId",
                        column: x => x.PessoaFisicaId,
                        principalTable: "Cliente",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Financeiro_Cliente_PessoaJuridicaId",
                        column: x => x.PessoaJuridicaId,
                        principalTable: "Cliente",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Financeiro_Contrato_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contrato",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Financeiro_Fornecedor_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedor",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Parcela",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FinanceiroId = table.Column<int>(type: "int", nullable: true),
                    NomeParcela = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValorJuros = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    DataVencimentoParcela = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DataEfetuacao = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    StatusPagamento = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcela", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parcela_Financeiro_FinanceiroId",
                        column: x => x.FinanceiroId,
                        principalTable: "Financeiro",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Financeiro_ContratoId",
                table: "Financeiro",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Financeiro_FornecedorId",
                table: "Financeiro",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Financeiro_PessoaFisicaId",
                table: "Financeiro",
                column: "PessoaFisicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Financeiro_PessoaJuridicaId",
                table: "Financeiro",
                column: "PessoaJuridicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcela_FinanceiroId",
                table: "Parcela",
                column: "FinanceiroId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parcela");

            migrationBuilder.DropTable(
                name: "Financeiro");
        }
    }
}
