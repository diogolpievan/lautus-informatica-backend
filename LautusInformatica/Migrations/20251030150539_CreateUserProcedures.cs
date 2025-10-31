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
<<<<<<< HEAD
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
=======
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

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_TrocarSenha(
                                        IN pUserId INT,
                                        IN newPass VARCHAR(25),
                                        OUT ok INT
                                    )
                                    BEGIN
                                        DECLARE userExists INT DEFAULT 0;
                                        DECLARE isDeleted BOOL DEFAULT FALSE;
                                        DECLARE isLocked BOOL DEFAULT FALSE;

                                        SELECT COUNT(*) INTO userExists FROM users WHERE Id = pUserId;

                                        IF userExists > 0 THEN
                                            SELECT IsDeleted, Lockout INTO isDeleted, isLocked FROM users WHERE Id = pUserId;

                                            IF isDeleted = FALSE THEN
                                                IF isLocked = FALSE THEN
                                                    UPDATE users
                                                    SET PasswordHash = newPass
                                                    WHERE Id = pUserId;

                                                    SET ok = 1;
                                                    SELECT 'Senha alterada com sucesso!' AS mensagem;

                                                ELSE
                                                    SET ok = 0;
                                                    SELECT 'Usuário bloqueado, contate o administrador para desbloquear!' AS mensagem;
                                                END IF;
                                            ELSE
                                                SET ok = 0;
                                                SELECT 'Usuário foi deletado!' AS mensagem;
                                            END IF;
                                        ELSE
                                            SET ok = 0;
                                            SELECT 'Usuário não existe!' AS mensagem;
                                        END IF;
                                    END");
>>>>>>> 219ac3c344d331f9b81645bece1946d494a2f6ce
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DesbloquearUsuario;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_TrocarSenha;");
        }
    }
}
