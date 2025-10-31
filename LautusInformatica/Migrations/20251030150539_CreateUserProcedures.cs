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
                                IN  p_Email VARCHAR(50),
                                IN  p_PasswordHash VARCHAR(255),
                                OUT p_IsValid BOOLEAN
                            )
                            BEGIN
                                DECLARE user_id INT;
                                DECLARE user_lockout BOOLEAN;
                                DECLARE failed_count INT;
                                DECLARE stored_hash VARCHAR(255);

                                SET p_IsValid = FALSE;

                                SELECT Id, Lockout, AccessFailedCount, PasswordHash
                                INTO user_id, user_lockout, failed_count, stored_hash
                                FROM Users
                                WHERE Email = p_Email AND IsDeleted = FALSE
                                LIMIT 1;

                                IF user_id IS NULL THEN
                                    SIGNAL SQLSTATE '45000'
                                    SET MESSAGE_TEXT = 'Usuário não encontrado';
                                END IF;

                                IF user_lockout = TRUE THEN
                                    SIGNAL SQLSTATE '45002'
                                    SET MESSAGE_TEXT = 'Usuário bloqueado por tentativas inválidas';
                                END IF;

                                IF stored_hash = p_PasswordHash THEN
                                    SET p_IsValid = TRUE;

                                    UPDATE Users
                                    SET AccessFailedCount = 0
                                    WHERE Id = user_id;

                                ELSE
                                    SET failed_count = failed_count + 1;

                                    IF failed_count >= 3 THEN
                                        UPDATE Users
                                        SET Lockout = TRUE,
                                            AccessFailedCount = failed_count
                                        WHERE Id = user_id;

                                        SIGNAL SQLSTATE '45002'
                                        SET MESSAGE_TEXT = 'Usuário bloqueado por tentativas inválidas';
                                    ELSE
                                        UPDATE Users
                                        SET AccessFailedCount = failed_count
                                        WHERE Id = user_id;
                                    END IF;

                                END IF;
                            END //
                            DELIMITER ;

            ");

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
                                        SET MESSAGE_TEXT = 'Usuário não encontrado';
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

            migrationBuilder.Sql(@"DELIMITER //
                        CREATE PROCEDURE sp_ExcluirUsuario(
                            IN p_UserId INT
                        )
                        BEGIN
                            UPDATE Users
                            SET IsDeleted = TRUE,
                                DeletedAt = NOW()
                            WHERE Id = p_UserId;

                            SELECT ROW_COUNT() AS RowsAffected;
                        END //
                        DELIMITER ;");

            migrationBuilder.Sql(@"DELIMITER //
                    CREATE PROCEDURE sp_CreateUser(
                        IN p_Username VARCHAR(100),
                        IN p_PasswordHash VARCHAR(255),
                        IN p_Phone VARCHAR(20),
                        IN p_Email VARCHAR(255),
                        IN p_Role INT,
                        IN p_Address VARCHAR(255)
                    )
                    BEGIN
                        IF EXISTS (SELECT 1 FROM Users WHERE Email = p_Email AND IsDeleted = FALSE) THEN
                            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Email already exists';
                        END IF;

                        INSERT INTO Users (
                            Username, PasswordHash, Phone, Email, Role, Address,
                            CreatedAt, IsDeleted, Lockout, AccessFailedCount
                        )
                        VALUES (
                            p_Username, p_PasswordHash, p_Phone, p_Email, p_Role, p_Address,
                            NOW(), FALSE, FALSE, 0
                        );

                        SELECT LAST_INSERT_ID() AS UserId;
                    END //
                    DELIMITER ;");

            migrationBuilder.Sql(@"DELIMITER //
                                    CREATE PROCEDURE sp_UpdateUser(
                                        IN p_Id INT,
                                        IN p_Username VARCHAR(100),
                                        IN p_Phone VARCHAR(20),
                                        IN p_Email VARCHAR(255),
                                        IN p_Role INT,
                                        IN p_Address VARCHAR(255)
                                    )
                                    BEGIN
                                        -- Usuário existe?
                                        IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = p_Id AND IsDeleted = FALSE) THEN
                                            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'User not found';
                                        END IF;

                                        -- Email já em uso por outro usuário?
                                        IF EXISTS (SELECT 1 FROM Users WHERE Email = p_Email AND Id <> p_Id AND IsDeleted = FALSE) THEN
                                            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Email already in use by another account';
                                        END IF;

                                        UPDATE Users
                                        SET Username = p_Username,
                                            Phone = p_Phone,
                                            Email = p_Email,
                                            Role = p_Role,
                                            Address = p_Address
                                        WHERE Id = p_Id;

                                        SELECT ROW_COUNT() AS RowsAffected;
                                    END //
                                    DELIMITER ;
                                    ");

        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DesbloquearUsuario;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_TrocarSenha;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ValidaLogin;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ExcluirUsuario;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CreateUser;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateUser;");
        }
    }
}
