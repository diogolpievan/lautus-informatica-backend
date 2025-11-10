using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LautusInformatica.Migrations
{
    public partial class CreateUsedItemsProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_CreateUsedItem(
                                    IN p_ServiceOrderId INT,
                                    IN p_ItemId INT,
                                    IN p_Quantity INT,
                                    IN p_AuthId INT,
                                    OUT p_UsedItemId INT
                                )
                                BEGIN
                                    DECLARE v_ServiceOrderExists INT DEFAULT 0;
                                    DECLARE v_ItemExists INT DEFAULT 0;
                                    DECLARE v_AvailableStock INT DEFAULT NULL;
                                    DECLARE v_LogId INT;

                                    -- Verificar se a ordem de serviço existe
                                    SELECT COUNT(*) INTO v_ServiceOrderExists
                                    FROM ServiceOrders
                                    WHERE Id = p_ServiceOrderId AND IsDeleted = FALSE;

                                    IF v_ServiceOrderExists = 0 THEN
                                        SIGNAL SQLSTATE '45000'
                                            SET MESSAGE_TEXT = 'Ordem de serviço não encontrada';
                                    END IF;

                                    -- Verificar se o item existe e tem estoque suficiente
                                    SELECT Quantity INTO v_AvailableStock
                                    FROM Items
                                    WHERE Id = p_ItemId AND IsDeleted = FALSE;

                                    IF v_AvailableStock IS NULL THEN
                                        SIGNAL SQLSTATE '45001'
                                            SET MESSAGE_TEXT = 'Item não encontrado';
                                    END IF;

                                    IF p_Quantity <= 0 THEN
                                        SIGNAL SQLSTATE '45003'
                                            SET MESSAGE_TEXT = 'A quantidade deve ser maior que zero';
                                    END IF;

                                    IF v_AvailableStock < p_Quantity THEN
                                        SIGNAL SQLSTATE '45002'
                                            SET MESSAGE_TEXT = 'Estoque insuficiente para este item';
                                    END IF;

                                    -- Inserir o item utilizado
                                    INSERT INTO UsedItems (
                                        ServiceOrderId, ItemId, Quantity, IsDeleted, DeletedDate
                                    )
                                    VALUES (
                                        p_ServiceOrderId, p_ItemId, p_Quantity, FALSE, NULL
                                    );

                                    SET p_UsedItemId = LAST_INSERT_ID();

                                    -- Atualizar o estoque do item
                                    UPDATE Items 
                                    SET Quantity = Quantity - p_Quantity 
                                    WHERE Id = p_ItemId;

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'UsedItems',
                                        0,
                                        CONCAT('Item utilizado adicionado - ID: ', p_UsedItemId, 
                                               ' - Ordem: ', p_ServiceOrderId, 
                                               ' - Item: ', p_ItemId, 
                                               ' - Quantidade: ', p_Quantity),
                                        v_LogId
                                    );
                                END");

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_UpdateUsedItem(
                                    IN p_Id INT,
                                    IN p_Quantity INT,
                                    IN p_AuthId INT,
                                    OUT p_Success BOOLEAN
                                )
                                BEGIN
                                    DECLARE v_OldQuantity INT;
                                    DECLARE v_ItemId INT;
                                    DECLARE v_ServiceOrderId INT;
                                    DECLARE v_AvailableStock INT;
                                    DECLARE v_QuantityDifference INT;
                                    DECLARE v_LogId INT;

                                    SET p_Success = FALSE;

                                    -- Buscar dados atuais
                                    SELECT Quantity, ItemId, ServiceOrderId 
                                    INTO v_OldQuantity, v_ItemId, v_ServiceOrderId
                                    FROM UsedItems
                                    WHERE Id = p_Id AND IsDeleted = FALSE;

                                    IF v_OldQuantity IS NULL THEN
                                        SIGNAL SQLSTATE '45000'
                                            SET MESSAGE_TEXT = 'Item utilizado não encontrado';
                                    END IF;

                                    IF p_Quantity <= 0 THEN
                                        SIGNAL SQLSTATE '45003'
                                            SET MESSAGE_TEXT = 'A quantidade deve ser maior que zero';
                                    END IF;

                                    -- Calcular diferença e verificar estoque
                                    SET v_QuantityDifference = p_Quantity - v_OldQuantity;

                                    IF v_QuantityDifference > 0 THEN
                                        -- Aumentando a quantidade, verificar se tem estoque
                                        SELECT Quantity INTO v_AvailableStock
                                        FROM Items
                                        WHERE Id = v_ItemId AND IsDeleted = FALSE;

                                        IF v_AvailableStock < v_QuantityDifference THEN
                                            SIGNAL SQLSTATE '45002'
                                                SET MESSAGE_TEXT = 'Estoque insuficiente para aumentar a quantidade';
                                        END IF;
                                    END IF;

                                    -- Atualizar quantidade
                                    UPDATE UsedItems
                                    SET Quantity = p_Quantity
                                    WHERE Id = p_Id;

                                    -- Ajustar estoque
                                    UPDATE Items 
                                    SET Quantity = Quantity - v_QuantityDifference 
                                    WHERE Id = v_ItemId;

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'UsedItems',
                                        1,
                                        CONCAT('Item utilizado atualizado - ID: ', p_Id, 
                                               ' - Quantidade: ', v_OldQuantity, ' → ', p_Quantity,
                                               ' - Diferença: ', v_QuantityDifference),
                                        v_LogId
                                    );

                                    SET p_Success = TRUE;
                                END");

            migrationBuilder.Sql(@"CREATE PROCEDURE sp_DeleteUsedItem(
                                    IN p_Id INT,
                                    IN p_AuthId INT,
                                    OUT p_Success BOOLEAN
                                )
                                BEGIN
                                    DECLARE v_Quantity INT;
                                    DECLARE v_ItemId INT;
                                    DECLARE v_ServiceOrderId INT;
                                    DECLARE v_LogId INT;

                                    SET p_Success = FALSE;

                                    -- Buscar dados para restaurar estoque
                                    SELECT Quantity, ItemId, ServiceOrderId 
                                    INTO v_Quantity, v_ItemId, v_ServiceOrderId
                                    FROM UsedItems
                                    WHERE Id = p_Id AND IsDeleted = FALSE;

                                    IF v_Quantity IS NULL THEN
                                        SIGNAL SQLSTATE '45000'
                                            SET MESSAGE_TEXT = 'Item utilizado não encontrado';
                                    END IF;

                                    -- Soft delete
                                    UPDATE UsedItems
                                    SET IsDeleted = TRUE,
                                        DeletedDate = NOW()
                                    WHERE Id = p_Id;

                                    -- Restaurar estoque
                                    UPDATE Items 
                                    SET Quantity = Quantity + v_Quantity 
                                    WHERE Id = v_ItemId;

                                    CALL sp_CreateLog(
                                        p_AuthId,
                                        'UsedItems',
                                        2,
                                        CONCAT('Item utilizado removido - ID: ', p_Id, 
                                               ' - Quantidade restaurada: ', v_Quantity,
                                               ' - Item: ', v_ItemId,
                                               ' - Ordem: ', v_ServiceOrderId),
                                        v_LogId
                                    );

                                    SET p_Success = TRUE;
                                END");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CreateUsedItem;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateUsedItem;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DeleteUsedItem;");
        }
    }
}