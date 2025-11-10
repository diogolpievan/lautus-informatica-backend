using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LautusInformatica.Migrations
{
    /// <inheritdoc />
    public partial class AlterServiceOrderTableProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ServiceOrders",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "ServicePrice",
                table: "ServiceOrders",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "ServicePrice",
                table: "ServiceOrders");
        }
    }
}
