using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LautusInformatica.Migrations
{
    /// <inheritdoc />
    public partial class CreateLogsTablesAndTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lockout",
                table: "Users",
                newName: "IsLocked");

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TableName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OperationType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OperationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                column: "UserId");

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_CreateLog(
                                    IN p_UserId INT,
                                    IN p_TableName VARCHAR(50),
                                    IN p_OperationType INT,
                                    IN p_Description TEXT,
                                    OUT p_LogId INT
                                )
                                BEGIN
                                    INSERT INTO Logs (
                                        UserId, 
                                        TableName, 
                                        OperationType, 
                                        Description, 
                                        OperationDate
                                    )
                                    VALUES (
                                        p_UserId,
                                        p_TableName,
                                        p_OperationType,
                                        p_Description,
                                        NOW()
                                    );

                                    SET p_LogId = LAST_INSERT_ID();
                                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.RenameColumn(
                name: "IsLocked",
                table: "Users",
                newName: "Lockout");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CreateLog;");
        }
    }
}
