using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_BUSESCONTROL.Migrations
{
    public partial class updatefornecedor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComplementoResidencial",
                table: "Fornecedor");

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Fornecedor",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "Fornecedor",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Fornecedor",
                keyColumn: "Cpf",
                keyValue: null,
                column: "Cpf",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Fornecedor",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Fornecedor",
                keyColumn: "Cnpj",
                keyValue: null,
                column: "Cnpj",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "Fornecedor",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ComplementoResidencial",
                table: "Fornecedor",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
