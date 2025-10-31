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
            migrationBuilder.Sql(@"DELIMITER //
            CREATE PROCEDURE sp_ValidaLogin(
                IN p_Email VARCHAR(50),
                IN p_PasswordHash VARCHAR(255)
            )
            BEGIN
                DECLARE user_id INT;
                DECLARE user_lockout BOOLEAN;
                DECLARE failed_count INT;
                DECLARE stored_hash VARCHAR(255);
                DECLARE is_valid BOOLEAN DEFAULT FALSE;

                SELECT Id, Lockout, AccessFailedCount, PasswordHash 
                INTO user_id, user_lockout, failed_count, stored_hash
                FROM Users 
                WHERE email = p_Email AND IsDeleted = FALSE;

                IF user_id IS NOT NULL AND user_lockout = FALSE THEN
                    IF stored_hash = p_PasswordHash THEN
                        SET is_valid = TRUE;
                        UPDATE Users 
                        SET AccessFailedCount = 0 
                        WHERE Id = user_id;
                    ELSE
                        SET failed_count = failed_count + 1;
            
                        IF failed_count >= 3 THEN
                            -- Bloquear usuário após 3 tentativas
                            UPDATE Users 
                            SET Lockout = TRUE, 
                                AccessFailedCount = failed_count
                            WHERE Id = user_id;
                        ELSE
                            -- Apenas incrementar contador
                            UPDATE Users 
                            SET AccessFailedCount = failed_count 
                            WHERE Id = user_id;
                        END IF;
                    END IF;
                END IF;

                SELECT 
                    is_valid AS LoginValid;
            END //
            DELIMITER ;
            ");

            migrationBuilder.Sql(@"")
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
