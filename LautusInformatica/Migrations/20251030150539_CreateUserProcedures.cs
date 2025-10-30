using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LautusInformatica.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_DesbloquearUsuario(
                                    IN pUserId INT
                                )
                                BEGIN
                                    IF EXISTS (SELECT 1 FROM users WHERE Id = pUserId) THEN
                                        UPDATE users
                                        SET Lockout = 0,
                                            AccessFailedCount = 0
                                        WHERE Id = pUserId;
                                    ELSE
                                        SIGNAL SQLSTATE '45000'
                                            SET MESSAGE_TEXT = 'Not found';
                                    END IF;
                                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
