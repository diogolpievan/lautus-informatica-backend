using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LautusInformatica.Migrations
{
    public partial class CreateServiceOrderProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Procedure para criar ordem de serviço
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_CreateServiceOrder(
                                    IN p_Equipment VARCHAR(100),
                                    IN p_Problem TEXT,
                                    IN p_Description TEXT,
                                    IN p_EntryDate DATE,
                                    IN p_CompletionDate DATE,
                                    IN p_ServicePrice DECIMAL(10,2),
                                    IN p_UserId INT,
                                    IN p_AuthId INT,
                                    OUT p_ServiceOrderId INT
                                )
                                BEGIN
                                    DECLARE v_UserExists INT DEFAULT 0;
                                    DECLARE v_LogId INT;

                                    -- Verifica se a data de entrada é maior que a data atual
                                    IF p_EntryDate > CURDATE() THEN
                                        SIGNAL SQLSTATE '45005'
                                            SET MESSAGE_TEXT = 'A data de entrada não pode ser maior que a data atual';
                                    END IF;

                                    -- Verifica se data de conclusão é anterior à data de entrada
                                    IF p_CompletionDate IS NOT NULL AND p_CompletionDate < p_EntryDate THEN
                                        SIGNAL SQLSTATE '45004'
                                            SET MESSAGE_TEXT = 'A data de conclusão não pode ser anterior à data de entrada';
                                    END IF;

                                    SELECT COUNT(*) INTO v_UserExists
                                    FROM Users
                                    WHERE Id = p_UserId AND IsDeleted = FALSE;

                                    IF v_UserExists = 0 THEN
                                        SIGNAL SQLSTATE '45000'
                                            SET MESSAGE_TEXT = 'Usuário não encontrado';
                                    END IF;

                                    INSERT INTO ServiceOrders (
                                        Equipment, Problem, Description, EntryDate, CompletionDate, Status,
                                        UserId, IsDeleted, DeletedDate, ServicePrice
                                    )
                                    VALUES (
                                        p_Equipment, p_Problem, p_Description, p_EntryDate, p_CompletionDate, 1, 
                                        p_UserId, FALSE, NULL, p_ServicePrice
                                    );

                                    SET p_ServiceOrderId = LAST_INSERT_ID();

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'ServiceOrders',
                                        0,
                                        CONCAT('Ordem de serviço criada - ID: ', p_ServiceOrderId, ' - Equipamento: ', p_Equipment),
                                        v_LogId
                                    );
                                END");

            // Procedure para atualizar ordem de serviço
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_UpdateServiceOrder(
                                    IN p_Id INT,
                                    IN p_Equipment VARCHAR(100),
                                    IN p_Problem TEXT,
                                    IN p_Description TEXT,
                                    IN p_EntryDate DATE,
                                    IN p_CompletionDate DATE,
                                    IN p_ServicePrice DECIMAL(10,2),
                                    IN p_UserId INT,
                                    IN p_AuthId INT,
                                    OUT p_Success BOOLEAN
                                )
                                BEGIN
                                    DECLARE v_UserExists INT DEFAULT 0;
                                    DECLARE v_LogId INT;
                                    SET p_Success = FALSE;

                                    -- Verifica se a ordem existe
                                    IF NOT EXISTS (SELECT 1 FROM ServiceOrders WHERE Id = p_Id AND IsDeleted = FALSE) THEN
                                        SIGNAL SQLSTATE '45001' 
                                            SET MESSAGE_TEXT = 'Ordem de serviço não encontrada';
                                    END IF;

                                    -- Verifica se o usuário é válido
                                    SELECT COUNT(*) INTO v_UserExists
                                    FROM Users
                                    WHERE Id = p_UserId AND IsDeleted = FALSE;

                                    IF v_UserExists = 0 THEN
                                        SIGNAL SQLSTATE '45000'
                                            SET MESSAGE_TEXT = 'Usuário não encontrado';
                                    END IF;

                                    -- Verifica se a data de entrada é maior que a data atual
                                    IF p_EntryDate > CURDATE() THEN
                                        SIGNAL SQLSTATE '45005'
                                            SET MESSAGE_TEXT = 'A data de entrada não pode ser maior que a data atual';
                                    END IF;

                                    -- Verifica se data de conclusão é anterior à data de entrada
                                    IF p_CompletionDate IS NOT NULL AND p_CompletionDate < p_EntryDate THEN
                                        SIGNAL SQLSTATE '45004'
                                            SET MESSAGE_TEXT = 'A data de conclusão não pode ser anterior à data de entrada';
                                    END IF;

                                    UPDATE ServiceOrders
                                    SET Equipment = p_Equipment,
                                        Problem = p_Problem,
                                        Description = p_Description,
                                        EntryDate = p_EntryDate,
                                        CompletionDate = p_CompletionDate,
                                        ServicePrice = p_ServicePrice,
                                        UserId = p_UserId
                                    WHERE Id = p_Id;

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'ServiceOrders',
                                        1,
                                        CONCAT('Ordem de serviço atualizada - ID: ', p_Id),
                                        v_LogId
                                    );

                                    SET p_Success = TRUE;
                                END");

            // As demais procedures permanecem idênticas
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_DeleteServiceOrder(
                                    IN p_Id INT,
                                    IN p_AuthId INT,
                                    OUT p_Success BOOLEAN
                                )
                                BEGIN
                                    DECLARE v_HasUsedItems INT DEFAULT 0;
                                    DECLARE v_LogId INT;

                                    IF NOT EXISTS (SELECT 1 FROM ServiceOrders WHERE Id = p_Id AND IsDeleted = FALSE) THEN
                                        SET p_Success = FALSE;
                                        SIGNAL SQLSTATE '45001'
                                            SET MESSAGE_TEXT = 'Ordem de serviço não encontrada';
                                    END IF;

                                    SELECT COUNT(*) INTO v_HasUsedItems
                                    FROM UsedItems
                                    WHERE ServiceOrderId = p_Id;

                                    IF v_HasUsedItems > 0 THEN
                                        SIGNAL SQLSTATE '45003'
                                            SET MESSAGE_TEXT = 'Não é possível excluir ordem de serviço com itens utilizados vinculados';
                                    END IF;

                                    UPDATE ServiceOrders
                                    SET IsDeleted = TRUE,
                                        DeletedDate = NOW()
                                    WHERE Id = p_Id;

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'ServiceOrders',
                                        2,
                                        CONCAT('Ordem de serviço excluída - ID: ', p_Id),
                                        v_LogId
                                    );

                                    SET p_Success = TRUE;
                                END");

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_ChangeServiceOrderStatus(
                                    IN p_Id INT,
                                    IN p_Status INT,
                                    IN p_AuthId INT,
                                    OUT p_Success BOOLEAN
                                )
                                BEGIN
                                    DECLARE v_CurrentStatus INT;
                                    DECLARE v_LogId INT;

                                    SET p_Success = FALSE;

                                    SELECT Status INTO v_CurrentStatus
                                    FROM ServiceOrders
                                    WHERE Id = p_Id AND IsDeleted = FALSE;

                                    IF v_CurrentStatus IS NULL THEN
                                        SIGNAL SQLSTATE '45001'
                                            SET MESSAGE_TEXT = 'Ordem de serviço não encontrada';
                                    END IF;;

                                    IF p_Status = 4 THEN 
                                        UPDATE ServiceOrders
                                        SET Status = p_Status,
                                            CompletionDate = CURDATE()
                                        WHERE Id = p_Id;
                                    ELSE
                                        UPDATE ServiceOrders
                                        SET Status = p_Status,
                                            CompletionDate = NULL
                                        WHERE Id = p_Id;
                                    END IF;

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'ServiceOrders',
                                        1,
                                        CONCAT('Status da ordem alterado - ID: ', p_Id, 
                                               ' - De: ', v_CurrentStatus, ' Para: ', p_Status),
                                        v_LogId
                                    );

                                    SET p_Success = TRUE;
                                END");

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_CompleteServiceOrder(
                                    IN p_Id INT,
                                    IN p_CompletionDate DATE,
                                    IN p_AuthId INT,
                                    OUT p_Success BOOLEAN
                                )
                                BEGIN
                                    DECLARE v_CurrentStatus INT;
                                    DECLARE v_LogId INT;

                                    SET p_Success = FALSE;

                                    SELECT Status INTO v_CurrentStatus
                                    FROM ServiceOrders
                                    WHERE Id = p_Id AND IsDeleted = FALSE;

                                    IF v_CurrentStatus IS NULL THEN
                                        SIGNAL SQLSTATE '45001'
                                            SET MESSAGE_TEXT = 'Ordem de serviço não encontrada';
                                    END IF;

                                    UPDATE ServiceOrders
                                    SET Status = 4, 
                                        CompletionDate = p_CompletionDate
                                    WHERE Id = p_Id;

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'ServiceOrders',
                                        1,
                                        CONCAT('Ordem de serviço marcada como concluída - ID: ', p_Id, 
                                               ' - Data: ', p_CompletionDate),
                                        v_LogId
                                    );

                                    SET p_Success = TRUE;
                                END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CreateServiceOrder;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateServiceOrder;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DeleteServiceOrder;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ChangeServiceOrderStatus;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CompleteServiceOrder;");
        }
    }
}
