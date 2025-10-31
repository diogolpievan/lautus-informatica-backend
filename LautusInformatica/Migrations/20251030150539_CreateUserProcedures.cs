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
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DesbloquearUsuario;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_TrocarSenha;");
        }
    }
}
