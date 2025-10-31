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
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_ValidaLogin(
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
                            END");

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
                                        IN pNewPasswordHash VARCHAR(255),
                                        OUT p_Success BOOLEAN
                                    )
                                    BEGIN
                                        DECLARE vUserId INT;
                                        DECLARE vLocked BOOLEAN;

                                        SELECT Id, Lockout
                                        INTO vUserId, vLocked
                                        FROM Users
                                        WHERE Id = pUserId
                                          AND IsDeleted = FALSE;

                                        IF vUserId IS NULL THEN
                                            SET p_Success = FALSE;
                                            SIGNAL SQLSTATE '45000'
                                                SET MESSAGE_TEXT = 'Usuário não encontrado';
                                        END IF;

                                        IF vLocked = TRUE THEN
                                            SET p_Success = FALSE;
                                            SIGNAL SQLSTATE '45002'
                                                SET MESSAGE_TEXT = 'Usuário bloqueado, contate o administrador';
                                        END IF;

                                        UPDATE Users
                                        SET PasswordHash = pNewPasswordHash,
                                            AccessFailedCount = 0,      
                                            Lockout = FALSE           
                                        WHERE Id = pUserId;

                                        SET p_Success = TRUE;
                                    END");

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_ExcluirUsuario(
                                        IN p_UserId INT,
                                        OUT p_Success BOOLEAN
                                    )
                                    BEGIN
                                        DECLARE vUserId INT;

                                        SELECT Id
                                        INTO vUserId
                                        FROM Users
                                        WHERE Id = p_UserId
                                          AND IsDeleted = FALSE;

                                        IF vUserId IS NULL THEN
                                            SET p_Success = FALSE;
                                            SIGNAL SQLSTATE '45000'
                                                SET MESSAGE_TEXT = 'Usuário não encontrado';
                                        END IF;

                                        UPDATE Users
                                        SET IsDeleted = TRUE,
                                            DeletedAt = NOW()
                                        WHERE Id = p_UserId;

                                        SET p_Success = TRUE;
                                    END");

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_CreateUser(
                                        IN p_Username VARCHAR(100),
                                        IN p_PasswordHash VARCHAR(255),
                                        IN p_Phone VARCHAR(20),
                                        IN p_Email VARCHAR(255),
                                        IN p_Role INT,
                                        IN p_Address VARCHAR(255),
                                        OUT p_UserId INT
                                    )
                                    BEGIN
                                        DECLARE vExists INT DEFAULT 0;

                                        SELECT COUNT(*) INTO vExists
                                        FROM Users
                                        WHERE Email = p_Email
                                          AND IsDeleted = FALSE;

                                        IF vExists > 0 THEN
                                            SIGNAL SQLSTATE '45001'
                                                SET MESSAGE_TEXT = 'Este email já está sendo utilizado';
                                        END IF;

                                        INSERT INTO Users (
                                            Username, PasswordHash, Phone, Email, Role, Address,
                                            CreatedAt, IsDeleted, Lockout, AccessFailedCount
                                        )
                                        VALUES (
                                            p_Username, p_PasswordHash, p_Phone, p_Email, p_Role, p_Address,
                                            NOW(), FALSE, FALSE, 0
                                        );

                                        SET p_UserId = LAST_INSERT_ID();
                                    END");

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_UpdateUser(
                                        IN p_Id INT,
                                        IN p_Username VARCHAR(100),
                                        IN p_Phone VARCHAR(20),
                                        IN p_Email VARCHAR(255),
                                        IN p_Role INT,
                                        IN p_Address VARCHAR(255),
                                        OUT p_Success BOOLEAN     
                                    )
                                    BEGIN
                                        DECLARE v_IsLocked BOOLEAN DEFAULT FALSE;
                                        SET p_Success = FALSE;

                                        IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = p_Id AND IsDeleted = FALSE) THEN
                                            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Usuário não encontrado';
                                        END IF;

                                        SELECT Lockout INTO v_IsLocked FROM Users WHERE Id = p_Id;
                                        IF v_IsLocked = TRUE THEN
                                            SIGNAL SQLSTATE '45002' SET MESSAGE_TEXT = 'Usuário bloqueado, contate o administrador';
                                        END IF;

                                        IF EXISTS (SELECT 1 FROM Users WHERE Email = p_Email AND Id <> p_Id AND IsDeleted = FALSE) THEN
                                            SIGNAL SQLSTATE '45001' SET MESSAGE_TEXT = 'Este email já está sendo utilizado';
                                        END IF;

                                        UPDATE Users
                                        SET Username = p_Username,
                                            Phone = p_Phone,
                                            Email = p_Email,
                                            Role = p_Role,
                                            Address = p_Address
                                        WHERE Id = p_Id;

                                        SET p_Success = TRUE;
                                        SELECT p_Success AS Success;
                                    END");

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
