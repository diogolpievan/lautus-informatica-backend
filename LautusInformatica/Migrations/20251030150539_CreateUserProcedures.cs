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
                                IN  p_Password VARCHAR(255),
                                OUT p_Success BOOLEAN
                            )
                            BEGIN
                                DECLARE v_UserId INT;
                                DECLARE v_IsLocked BOOLEAN;
                                DECLARE v_FailedCount INT;
                                DECLARE v_StoredPass VARBINARY(255);
                                DECLARE v_DecryptedPass VARCHAR(255);

                                SET p_Success = FALSE;

                                SELECT Id, IsLocked, AccessFailedCount, Password
                                INTO v_UserId, v_IsLocked, v_FailedCount, v_StoredPass
                                FROM Users
                                WHERE Email = p_Email AND IsDeleted = FALSE
                                LIMIT 1;

                                IF v_UserId IS NULL THEN
                                    SIGNAL SQLSTATE '45000'
                                    SET MESSAGE_TEXT = 'Usuário não encontrado';
                                END IF;

                                IF v_IsLocked = TRUE THEN
                                    SIGNAL SQLSTATE '45002'
                                    SET MESSAGE_TEXT = 'Usuário bloqueado por tentativas inválidas';
                                END IF;

                                SET @key = 'G7v$9kLm#4rPz2Q!';

                                SET v_DecryptedPass = CAST(AES_DECRYPT(v_StoredPass, @key) AS CHAR);

                               
                                 IF v_DecryptPass = p_Password THEN
                                    SET p_Success = TRUE;

                                    UPDATE Users
                                    SET AccessFailedCount = 0
                                    WHERE Id = v_UserId;

                                ELSE
                                    SET v_FailedCount = v_FailedCount + 1;

                                    IF v_FailedCount >= 3 THEN
                                        UPDATE Users
                                        SET IsLocked = TRUE,
                                            AccessFailedCount = v_FailedCount
                                        WHERE Id = v_UserId;

                                        SIGNAL SQLSTATE '45002'
                                        SET MESSAGE_TEXT = 'Usuário bloqueado por tentativas inválidas';
                                    ELSE
                                        UPDATE Users
                                        SET AccessFailedCount = v_FailedCount
                                        WHERE Id = v_UserId;
                                    END IF;

                                END IF;
                            END");

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_DesbloquearUsuario(
                                    IN p_UserId INT,
                                    IN p_AuthId INT,
                                    OUT p_Success BOOLEAN       
                                )
                                BEGIN
                                    SET p_Success = FALSE; 
                                    IF EXISTS (SELECT 1 FROM Users WHERE Id = p_UserId and IsLocked = TRUE) THEN
                                        UPDATE Users
                                        SET IsLocked = FALSE,
                                            AccessFailedCount = 0
                                        WHERE Id = p_UserId;
                                        SET p_Success = TRUE;
                                    ELSE
                                        SIGNAL SQLSTATE '45000'
                                        SET MESSAGE_TEXT = 'Usuário não encontrado';
                                    END IF;

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'Users',
                                        1,
                                        CONCAT('Usuário desbloqueado - ID: ', p_UserId),
                                        v_LogId
                                    );
                                END");

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_TrocarSenha(
                                        IN p_UserId INT,
                                        IN p_AuthId INT,
                                        IN p_NewPassword VARCHAR(255),
                                        OUT p_Success BOOLEAN
                                    )
                                    BEGIN
                                        DECLARE v_UserId INT;
                                        DECLARE v_Locked BOOLEAN;

                                        SELECT Id
                                        INTO v_UserId
                                        FROM Users
                                        WHERE Id = p_UserId
                                          AND IsDeleted = FALSE;

                                        IF v_UserId IS NULL THEN
                                            SET p_Success = FALSE;
                                            SIGNAL SQLSTATE '45000'
                                                SET MESSAGE_TEXT = 'Usuário não encontrado';
                                        END IF;
                                        
                                        SET @key = 'G7v$9kLm#4rPz2Q!';

                                        UPDATE Users
                                        SET Password = AES_ENCRYPT(p_NewPassword, @key)        
                                        WHERE Id = p_UserId;

                                        CALL sp_CreateLog(
                                            p_AuthId,
                                            'Users',
                                            1,
                                            CONCAT('Senha alterada - User ID: ', p_UserId),
                                            v_LogId);

                                        CALL sp_DesbloquearUsuario(p_UserId, @success);

                                        SET p_Success = TRUE;
                                    END");

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_ExcluirUsuario(
                                        IN p_UserId INT,
                                        IN p_AuthId INT,
                                        OUT p_Success BOOLEAN
                                    )
                                    BEGIN
                                        DECLARE v_UserId INT;

                                        SELECT Id
                                        INTO v_UserId
                                        FROM Users
                                        WHERE Id = p_UserId
                                          AND IsDeleted = FALSE;

                                        IF v_UserId IS NULL THEN
                                            SET p_Success = FALSE;
                                            SIGNAL SQLSTATE '45000'
                                                SET MESSAGE_TEXT = 'Usuário não encontrado';
                                        END IF;

                                        UPDATE Users
                                        SET IsDeleted = TRUE,
                                            DeletedAt = NOW()
                                        WHERE Id = p_UserId;
                                        
                                        INSERT INTO logs (UserId, TableName, OperationType, Description, OperationDate) VALUES (
	                            	        p_AuthId,
	                            	        'Users',
	                            	        2,
	                            	        CONCAT('User: ', p_Username, ' deletado'),
	                            	        NOW()	                      
	                                    );


                                        SET p_Success = TRUE;
                                    END");

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_CreateUser(
                                        IN p_Username VARCHAR(100),
                                        IN p_Password VARCHAR(255),
                                        IN p_Phone VARCHAR(20),
                                        IN p_Email VARCHAR(255),
                                        IN p_Role INT,
                                        IN p_Address VARCHAR(255),
                                        IN p_AuthId INT,
                                        OUT p_UserId INT
                                    )
                                    BEGIN
                                        DECLARE v_Exists INT DEFAULT 0;

                                        SELECT COUNT(*) INTO v_Exists
                                        FROM Users
                                        WHERE Email = p_Email
                                          AND IsDeleted = FALSE;

                                        IF v_Exists > 0 THEN
                                            SIGNAL SQLSTATE '45001'
                                                SET MESSAGE_TEXT = 'Este email já está sendo utilizado';
                                        END IF;

                                        SET @key = 'G7v$9kLm#4rPz2Q!';

                                        INSERT INTO Users (
                                            Username, Password, Phone, Email, Role, Address,
                                            CreatedAt, IsDeleted, IsLocked, AccessFailedCount
                                        )
                                        VALUES (
                                            p_Username, AES_ENCRYPT(p_Password, @key), p_Phone, p_Email, p_Role, p_Address,
                                            NOW(), FALSE, FALSE, 0
                                        );

                                        CALL sp_CreateLog(
                                            p_AuthId,
                                            'Users',
                                            0,
                                            CONCAT('Usuário criado - ID: ', p_UserId, ' - Nome: ', p_Username),
                                            v_LogId
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

                                        SELECT IsLocked INTO v_IsLocked FROM Users WHERE Id = p_Id;
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

                                       CALL sp_CreateLog(
                                            p_AuthId,
                                            'Users',
                                            1,
                                            CONCAT('Usuário atualizado - ID: ', p_Id, ' - Nome: ', p_Username),
                                            v_LogId
                                        );
                                        SET p_Success = TRUE;
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
