using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LautusInformatica.Migrations
{
    /// <inheritdoc />
    public partial class CreateItemProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_CreateItem(
                                    IN p_Name VARCHAR(100),
                                    IN p_Description TEXT,
                                    IN p_Quantity INT,
                                    IN p_UnitPrice DECIMAL(10,2),
                                    IN p_Category INT,
                                    IN p_AuthId INT,
                                    OUT p_ItemId INT
                                )
                                BEGIN
                                    DECLARE v_Exists INT DEFAULT 0;
                                    DECLARE v_LogId INT;

                                    SELECT COUNT(*) INTO v_Exists
                                    FROM Items
                                    WHERE Name = p_Name
                                      AND IsDeleted = FALSE;

                                    IF v_Exists > 0 THEN
                                        SIGNAL SQLSTATE '45001'
                                            SET MESSAGE_TEXT = 'Já existe um item com este nome';
                                    END IF;

                                    INSERT INTO Items (
                                        Name, Description, Quantity, UnitPrice, Category,
                                        IsDeleted, DeletedDate
                                    )
                                    VALUES (
                                        p_Name, p_Description, p_Quantity, p_UnitPrice, p_Category,
                                        FALSE, NULL
                                    );

                                    SET p_ItemId = LAST_INSERT_ID();

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'Items',
                                        0,
                                        CONCAT('Item: ', p_Name, ' criado (ID: ', p_ItemId, ')'),
                                        v_LogId
                                    );
                                END");

            // Procedure para atualizar item
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_UpdateItem(
                                    IN p_Id INT,
                                    IN p_Name VARCHAR(100),
                                    IN p_Description TEXT,
                                    IN p_Quantity INT,
                                    IN p_UnitPrice DECIMAL(10,2),
                                    IN p_Category INT,
                                    IN p_AuthId INT,
                                    OUT p_Success BOOLEAN
                                )
                                BEGIN
                                    DECLARE v_OldName VARCHAR(100);
                                    DECLARE v_LogId INT;
                                    SET p_Success = FALSE;

                                    IF NOT EXISTS (SELECT 1 FROM Items WHERE Id = p_Id AND IsDeleted = FALSE) THEN
                                        SIGNAL SQLSTATE '45000' 
                                            SET MESSAGE_TEXT = 'Item não encontrado';
                                    END IF;

                                    IF EXISTS (SELECT 1 FROM Items WHERE Name = p_Name AND Id <> p_Id AND IsDeleted = FALSE) THEN
                                        SIGNAL SQLSTATE '45001' 
                                            SET MESSAGE_TEXT = 'Já existe um item com este nome';
                                    END IF;

                                    SELECT Name INTO v_OldName FROM Items WHERE Id = p_Id;

                                    UPDATE Items
                                    SET Name = p_Name,
                                        Description = p_Description,
                                        Quantity = p_Quantity,
                                        UnitPrice = p_UnitPrice,
                                        Category = p_Category
                                    WHERE Id = p_Id;

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'Items',
                                        1,
                                        CONCAT('Item atualizado - ID: ', p_Id, ' - De: ', v_OldName, ' Para: ', p_Name),
                                        v_LogId
                                    );

                                    SET p_Success = TRUE;
                                END");

            // Procedure para excluir item (soft delete)
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_DeleteItem(
                                    IN p_Id INT,
                                    IN p_AuthId INT,
                                    OUT p_Success BOOLEAN
                                )
                                BEGIN
                                    DECLARE v_ItemName VARCHAR(100);
                                    DECLARE v_HasUsedItems INT DEFAULT 0;
                                    DECLARE v_LogId INT;

                                    SELECT Name INTO v_ItemName
                                    FROM Items
                                    WHERE Id = p_Id
                                      AND IsDeleted = FALSE;

                                    IF v_ItemName IS NULL THEN
                                        SET p_Success = FALSE;
                                        SIGNAL SQLSTATE '45000'
                                            SET MESSAGE_TEXT = 'Item não encontrado';
                                    END IF;

                                    SELECT COUNT(*) INTO v_HasUsedItems
                                    FROM UsedItems
                                    WHERE ItemId = p_Id;

                                    IF v_HasUsedItems > 0 THEN
                                        SIGNAL SQLSTATE '45003'
                                            SET MESSAGE_TEXT = 'Não é possível excluir item pois está vinculado a registros de uso';
                                    END IF;

                                    UPDATE Items
                                    SET IsDeleted = TRUE,
                                        DeletedDate = NOW()
                                    WHERE Id = p_Id;

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'Items',
                                        2,
                                        CONCAT('Item excluído - ID: ', p_Id, ' - Nome: ', v_ItemName),
                                        v_LogId
                                    );

                                    SET p_Success = TRUE;
                                END");

            // Procedure para restaurar item
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_RestoreItem(
                                    IN p_Id INT,
                                    IN p_AuthId INT,
                                    OUT p_Success BOOLEAN
                                )
                                BEGIN
                                    DECLARE v_ItemName VARCHAR(100);
                                    DECLARE v_LogId INT;

                                    SELECT Name INTO v_ItemName
                                    FROM Items
                                    WHERE Id = p_Id
                                      AND IsDeleted = TRUE;

                                    IF v_ItemName IS NULL THEN
                                        SET p_Success = FALSE;
                                        SIGNAL SQLSTATE '45000'
                                            SET MESSAGE_TEXT = 'Item não encontrado ou já ativo';
                                    END IF;

                                    UPDATE Items
                                    SET IsDeleted = FALSE,
                                        DeletedDate = NULL
                                    WHERE Id = p_Id;

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'Items',
                                        1,
                                        CONCAT('Item restaurado - ID: ', p_Id, ' - Nome: ', v_ItemName),
                                        v_LogId
                                    );

                                    SET p_Success = TRUE;
                                END");

            // Procedure para ajustar estoque
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_AdjustStock(
                                    IN p_Id INT,
                                    IN p_Quantity INT,
                                    IN p_AuthId INT,
                                    IN p_Reason VARCHAR(255),
                                    OUT p_Success BOOLEAN
                                )
                                BEGIN
                                    DECLARE v_CurrentQuantity INT;
                                    DECLARE v_ItemName VARCHAR(100);
                                    DECLARE v_NewQuantity INT;
                                    DECLARE v_LogId INT;

                                    SELECT Quantity, Name INTO v_CurrentQuantity, v_ItemName
                                    FROM Items
                                    WHERE Id = p_Id
                                      AND IsDeleted = FALSE;

                                    IF v_ItemName IS NULL THEN
                                        SET p_Success = FALSE;
                                        SIGNAL SQLSTATE '45000'
                                            SET MESSAGE_TEXT = 'Item não encontrado';
                                    END IF;

                                    IF p_Quantity < 0 AND v_CurrentQuantity < ABS(p_Quantity) THEN
                                        SIGNAL SQLSTATE '45004'
                                            SET MESSAGE_TEXT = 'Estoque insuficiente para esta operação';
                                    END IF;

                                    SET v_NewQuantity = v_CurrentQuantity + p_Quantity;

                                    UPDATE Items
                                    SET Quantity = v_NewQuantity
                                    WHERE Id = p_Id;

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'Items',
                                        1,
                                        CONCAT('Ajuste de estoque - Item: ', v_ItemName, 
                                               ' (ID: ', p_Id, ') - Quantidade: ', p_Quantity,
                                               ' - Estoque anterior: ', v_CurrentQuantity,
                                               ' - Novo estoque: ', v_NewQuantity,
                                               ' - Motivo: ', p_Reason),
                                        v_LogId
                                    );

                                    SET p_Success = TRUE;
                                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CreateItem;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateItem;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DeleteItem;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_RestoreItem;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_AdjustStock;");
        }
    }
}
