DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS `POMELO_AFTER_ADD_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` LIKE '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    CREATE TABLE `Cliente` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Email` longtext CHARACTER SET utf8mb4 NULL,
        `Telefone` varchar(9) CHARACTER SET utf8mb4 NOT NULL,
        `Cep` varchar(8) CHARACTER SET utf8mb4 NOT NULL,
        `NumeroResidencial` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Logradouro` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ComplementoResidencial` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Bairro` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Cidade` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Estado` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Ddd` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Adimplente` int NOT NULL,
        `Discriminator` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Name` longtext CHARACTER SET utf8mb4 NULL,
        `Cpf` longtext CHARACTER SET utf8mb4 NULL,
        `DataNascimento` datetime(6) NULL,
        `Rg` longtext CHARACTER SET utf8mb4 NULL,
        `NameMae` longtext CHARACTER SET utf8mb4 NULL,
        `IdVinculacaoContratual` int NULL,
        `Status` int NULL,
        `NomeFantasia` longtext CHARACTER SET utf8mb4 NULL,
        `Cnpj` longtext CHARACTER SET utf8mb4 NULL,
        `RazaoSocial` longtext CHARACTER SET utf8mb4 NULL,
        `InscricaoEstadual` longtext CHARACTER SET utf8mb4 NULL,
        `InscricaoMunicipal` longtext CHARACTER SET utf8mb4 NULL,
        `PessoaJuridica_Status` int NULL,
        CONSTRAINT `PK_Cliente` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    CREATE TABLE `Funcionario` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Cpf` longtext CHARACTER SET utf8mb4 NOT NULL,
        `DataNascimento` datetime(6) NOT NULL,
        `Email` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Telefone` varchar(9) CHARACTER SET utf8mb4 NOT NULL,
        `Cep` varchar(8) CHARACTER SET utf8mb4 NOT NULL,
        `NumeroResidencial` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Logradouro` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ComplementoResidencial` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Bairro` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Cidade` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Estado` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Ddd` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Apelido` longtext CHARACTER SET utf8mb4 NULL,
        `Senha` longtext CHARACTER SET utf8mb4 NULL,
        `Status` int NOT NULL,
        `Cargo` int NOT NULL,
        `StatusUsuario` int NOT NULL,
        CONSTRAINT `PK_Funcionario` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    CREATE TABLE `Onibus` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Marca` longtext CHARACTER SET utf8mb4 NOT NULL,
        `NameBus` longtext CHARACTER SET utf8mb4 NOT NULL,
        `DataFabricacao` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Renavam` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Placa` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Chassi` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Assentos` longtext CHARACTER SET utf8mb4 NOT NULL,
        `CorBus` longtext CHARACTER SET utf8mb4 NOT NULL,
        `StatusOnibus` int NOT NULL,
        CONSTRAINT `PK_Onibus` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    CREATE TABLE `PaletaCores` (
        `id` int NOT NULL AUTO_INCREMENT,
        `Cor` longtext CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_PaletaCores` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    CREATE TABLE `Contrato` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `MotoristaId` int NOT NULL,
        `OnibusId` int NOT NULL,
        `ValorMonetario` decimal(65,30) NOT NULL,
        `ValorParcelaContrato` decimal(65,30) NULL,
        `ValorTotalPagoContrato` decimal(65,30) NULL,
        `ValorParcelaContratoPorCliente` decimal(65,30) NULL,
        `DataEmissao` datetime(6) NOT NULL,
        `DataVencimento` datetime(6) NOT NULL,
        `Detalhamento` longtext CHARACTER SET utf8mb4 NOT NULL,
        `QtParcelas` int NULL,
        `Pagament` int NOT NULL,
        `StatusContrato` int NOT NULL,
        `Aprovacao` int NOT NULL,
        `Andamento` int NOT NULL,
        CONSTRAINT `PK_Contrato` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Contrato_Funcionario_MotoristaId` FOREIGN KEY (`MotoristaId`) REFERENCES `Funcionario` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_Contrato_Onibus_OnibusId` FOREIGN KEY (`OnibusId`) REFERENCES `Onibus` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    CREATE TABLE `ClientesContrato` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContratoId` int NULL,
        `PessoaJuridicaId` int NULL,
        `PessoaFisicaId` int NULL,
        `DataEmissaoPdfRescisao` datetime(6) NULL,
        `ProcessRescisao` int NOT NULL,
        CONSTRAINT `PK_ClientesContrato` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ClientesContrato_Cliente_PessoaFisicaId` FOREIGN KEY (`PessoaFisicaId`) REFERENCES `Cliente` (`Id`),
        CONSTRAINT `FK_ClientesContrato_Cliente_PessoaJuridicaId` FOREIGN KEY (`PessoaJuridicaId`) REFERENCES `Cliente` (`Id`),
        CONSTRAINT `FK_ClientesContrato_Contrato_ContratoId` FOREIGN KEY (`ContratoId`) REFERENCES `Contrato` (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    CREATE INDEX `IX_ClientesContrato_ContratoId` ON `ClientesContrato` (`ContratoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    CREATE INDEX `IX_ClientesContrato_PessoaFisicaId` ON `ClientesContrato` (`PessoaFisicaId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    CREATE INDEX `IX_ClientesContrato_PessoaJuridicaId` ON `ClientesContrato` (`PessoaJuridicaId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    CREATE INDEX `IX_Contrato_MotoristaId` ON `Contrato` (`MotoristaId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    CREATE INDEX `IX_Contrato_OnibusId` ON `Contrato` (`OnibusId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622002627_create database') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230622002627_create database', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622004211_corrigindo mapeamento') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230622004211_corrigindo mapeamento', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622004842_corrigindo mapeamento  2') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230622004842_corrigindo mapeamento  2', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622165422_REPARACAO') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230622165422_REPARACAO', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230622165700_REPARACAO 2') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230622165700_REPARACAO 2', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230707175535_fornecedor') THEN

    CREATE TABLE `Fornecedor` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Email` longtext CHARACTER SET utf8mb4 NULL,
        `Telefone` varchar(9) CHARACTER SET utf8mb4 NOT NULL,
        `NameOrRazaoSocial` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Cpf` longtext CHARACTER SET utf8mb4 NOT NULL,
        `DataFornecedor` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Cnpj` longtext CHARACTER SET utf8mb4 NOT NULL,
        `TypePessoa` int NOT NULL,
        `Status` int NOT NULL,
        `Cep` varchar(8) CHARACTER SET utf8mb4 NOT NULL,
        `NumeroResidencial` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Logradouro` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ComplementoResidencial` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Bairro` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Cidade` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Estado` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Ddd` longtext CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Fornecedor` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230707175535_fornecedor') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230707175535_fornecedor', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230707181210_correção de fornecedor') THEN

    ALTER TABLE `Fornecedor` MODIFY COLUMN `DataFornecedor` datetime(6) NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230707181210_correção de fornecedor') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230707181210_correção de fornecedor', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230708180014_update fornecedor') THEN

    ALTER TABLE `Fornecedor` DROP COLUMN `ComplementoResidencial`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230708180014_update fornecedor') THEN

    ALTER TABLE `Fornecedor` MODIFY COLUMN `Cpf` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230708180014_update fornecedor') THEN

    ALTER TABLE `Fornecedor` MODIFY COLUMN `Cnpj` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230708180014_update fornecedor') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230708180014_update fornecedor', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230719145037_financeiro') THEN

    CREATE TABLE `Financeiro` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContratoId` int NULL,
        `PessoaJuridicaId` int NULL,
        `PessoaFisicaId` int NULL,
        `FornecedorFisicoId` int NULL,
        `FornecedorJuridicoId` int NULL,
        `FornecedorId` int NULL,
        `DataVencimento` datetime(6) NOT NULL,
        `ValorParcelaDR` decimal(65,30) NULL,
        `ValorTotDR` decimal(65,30) NOT NULL,
        `ValorTotalPagoCliente` decimal(65,30) NULL,
        `ValorTotTaxaJurosPaga` decimal(65,30) NULL,
        `DataEmissao` datetime(6) NOT NULL,
        `QtParcelas` int NULL,
        `TypeEfetuacao` int NOT NULL,
        `DespesaReceita` int NOT NULL,
        `Pagament` int NOT NULL,
        `FinanceiroStatus` int NOT NULL,
        `Detalhamento` longtext CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Financeiro` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Financeiro_Cliente_PessoaFisicaId` FOREIGN KEY (`PessoaFisicaId`) REFERENCES `Cliente` (`Id`),
        CONSTRAINT `FK_Financeiro_Cliente_PessoaJuridicaId` FOREIGN KEY (`PessoaJuridicaId`) REFERENCES `Cliente` (`Id`),
        CONSTRAINT `FK_Financeiro_Contrato_ContratoId` FOREIGN KEY (`ContratoId`) REFERENCES `Contrato` (`Id`),
        CONSTRAINT `FK_Financeiro_Fornecedor_FornecedorId` FOREIGN KEY (`FornecedorId`) REFERENCES `Fornecedor` (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230719145037_financeiro') THEN

    CREATE TABLE `Parcela` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `FinanceiroId` int NULL,
        `NomeParcela` longtext CHARACTER SET utf8mb4 NULL,
        `ValorJuros` decimal(65,30) NULL,
        `DataVencimentoParcela` datetime(6) NULL,
        `DataEfetuacao` datetime(6) NULL,
        `StatusPagamento` int NULL,
        CONSTRAINT `PK_Parcela` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Parcela_Financeiro_FinanceiroId` FOREIGN KEY (`FinanceiroId`) REFERENCES `Financeiro` (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230719145037_financeiro') THEN

    CREATE INDEX `IX_Financeiro_ContratoId` ON `Financeiro` (`ContratoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230719145037_financeiro') THEN

    CREATE INDEX `IX_Financeiro_FornecedorId` ON `Financeiro` (`FornecedorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230719145037_financeiro') THEN

    CREATE INDEX `IX_Financeiro_PessoaFisicaId` ON `Financeiro` (`PessoaFisicaId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230719145037_financeiro') THEN

    CREATE INDEX `IX_Financeiro_PessoaJuridicaId` ON `Financeiro` (`PessoaJuridicaId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230719145037_financeiro') THEN

    CREATE INDEX `IX_Parcela_FinanceiroId` ON `Parcela` (`FinanceiroId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230719145037_financeiro') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230719145037_financeiro', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230719213651_edit financeiro') THEN

    ALTER TABLE `Financeiro` DROP COLUMN `FornecedorFisicoId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230719213651_edit financeiro') THEN

    ALTER TABLE `Financeiro` DROP COLUMN `FornecedorJuridicoId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230719213651_edit financeiro') THEN

    ALTER TABLE `Financeiro` RENAME COLUMN `ValorTotalPagoCliente` TO `ValorTotalPago`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230719213651_edit financeiro') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230719213651_edit financeiro', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230729202458_finish table db') THEN

    CREATE TABLE `Rescisao` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Multa` decimal(65,30) NULL,
        `ValorPagoContrato` decimal(65,30) NULL,
        `ContratoId` int NULL,
        `PessoaFisicaId` int NULL,
        `PessoaJuridicaId` int NULL,
        `DataRescisao` datetime(6) NULL,
        CONSTRAINT `PK_Rescisao` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Rescisao_Cliente_PessoaFisicaId` FOREIGN KEY (`PessoaFisicaId`) REFERENCES `Cliente` (`Id`),
        CONSTRAINT `FK_Rescisao_Cliente_PessoaJuridicaId` FOREIGN KEY (`PessoaJuridicaId`) REFERENCES `Cliente` (`Id`),
        CONSTRAINT `FK_Rescisao_Contrato_ContratoId` FOREIGN KEY (`ContratoId`) REFERENCES `Contrato` (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230729202458_finish table db') THEN

    CREATE INDEX `IX_Rescisao_ContratoId` ON `Rescisao` (`ContratoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230729202458_finish table db') THEN

    CREATE INDEX `IX_Rescisao_PessoaFisicaId` ON `Rescisao` (`PessoaFisicaId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230729202458_finish table db') THEN

    CREATE INDEX `IX_Rescisao_PessoaJuridicaId` ON `Rescisao` (`PessoaJuridicaId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230729202458_finish table db') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230729202458_finish table db', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230731002352_get parcelas') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230731002352_get parcelas', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230827164755_redefinirPassword') THEN

    ALTER TABLE `Funcionario` ADD `ChaveRedefinition` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230827164755_redefinirPassword') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230827164755_redefinirPassword', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924011553_motoristas substitutos') THEN

    CREATE TABLE `SubContratoMotorista` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContratoId` int NOT NULL,
        `FuncionarioId` int NOT NULL,
        `DataInicio` datetime(6) NOT NULL,
        `DataFinal` datetime(6) NOT NULL,
        CONSTRAINT `PK_SubContratoMotorista` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_SubContratoMotorista_Contrato_ContratoId` FOREIGN KEY (`ContratoId`) REFERENCES `Contrato` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_SubContratoMotorista_Funcionario_FuncionarioId` FOREIGN KEY (`FuncionarioId`) REFERENCES `Funcionario` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924011553_motoristas substitutos') THEN

    CREATE INDEX `IX_SubContratoMotorista_ContratoId` ON `SubContratoMotorista` (`ContratoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924011553_motoristas substitutos') THEN

    CREATE INDEX `IX_SubContratoMotorista_FuncionarioId` ON `SubContratoMotorista` (`FuncionarioId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924011553_motoristas substitutos') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230924011553_motoristas substitutos', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924015308_motoristas substitutos edit') THEN

    ALTER TABLE `SubContratoMotorista` DROP FOREIGN KEY `FK_SubContratoMotorista_Contrato_ContratoId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924015308_motoristas substitutos edit') THEN

    ALTER TABLE `SubContratoMotorista` DROP FOREIGN KEY `FK_SubContratoMotorista_Funcionario_FuncionarioId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924015308_motoristas substitutos edit') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'SubContratoMotorista');
    ALTER TABLE `SubContratoMotorista` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924015308_motoristas substitutos edit') THEN

    ALTER TABLE `SubContratoMotorista` RENAME `SubContratoMotoristas`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924015308_motoristas substitutos edit') THEN

    ALTER TABLE `SubContratoMotoristas` RENAME COLUMN `DataInicio` TO `DataInicial`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924015308_motoristas substitutos edit') THEN

    ALTER TABLE `SubContratoMotoristas` RENAME INDEX `IX_SubContratoMotorista_FuncionarioId` TO `IX_SubContratoMotoristas_FuncionarioId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924015308_motoristas substitutos edit') THEN

    ALTER TABLE `SubContratoMotoristas` RENAME INDEX `IX_SubContratoMotorista_ContratoId` TO `IX_SubContratoMotoristas_ContratoId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924015308_motoristas substitutos edit') THEN

    ALTER TABLE `SubContratoMotoristas` ADD CONSTRAINT `PK_SubContratoMotoristas` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'SubContratoMotoristas', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924015308_motoristas substitutos edit') THEN

    ALTER TABLE `SubContratoMotoristas` ADD CONSTRAINT `FK_SubContratoMotoristas_Contrato_ContratoId` FOREIGN KEY (`ContratoId`) REFERENCES `Contrato` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924015308_motoristas substitutos edit') THEN

    ALTER TABLE `SubContratoMotoristas` ADD CONSTRAINT `FK_SubContratoMotoristas_Funcionario_FuncionarioId` FOREIGN KEY (`FuncionarioId`) REFERENCES `Funcionario` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924015308_motoristas substitutos edit') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230924015308_motoristas substitutos edit', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924020332_motoristas substitutos edit2') THEN

    ALTER TABLE `SubContratoMotoristas` DROP FOREIGN KEY `FK_SubContratoMotoristas_Contrato_ContratoId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924020332_motoristas substitutos edit2') THEN

    ALTER TABLE `SubContratoMotoristas` DROP FOREIGN KEY `FK_SubContratoMotoristas_Funcionario_FuncionarioId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924020332_motoristas substitutos edit2') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'SubContratoMotoristas');
    ALTER TABLE `SubContratoMotoristas` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924020332_motoristas substitutos edit2') THEN

    ALTER TABLE `SubContratoMotoristas` RENAME `SubContratoMotorista`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924020332_motoristas substitutos edit2') THEN

    ALTER TABLE `SubContratoMotorista` RENAME INDEX `IX_SubContratoMotoristas_FuncionarioId` TO `IX_SubContratoMotorista_FuncionarioId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924020332_motoristas substitutos edit2') THEN

    ALTER TABLE `SubContratoMotorista` RENAME INDEX `IX_SubContratoMotoristas_ContratoId` TO `IX_SubContratoMotorista_ContratoId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924020332_motoristas substitutos edit2') THEN

    ALTER TABLE `SubContratoMotorista` ADD CONSTRAINT `PK_SubContratoMotorista` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'SubContratoMotorista', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924020332_motoristas substitutos edit2') THEN

    ALTER TABLE `SubContratoMotorista` ADD CONSTRAINT `FK_SubContratoMotorista_Contrato_ContratoId` FOREIGN KEY (`ContratoId`) REFERENCES `Contrato` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924020332_motoristas substitutos edit2') THEN

    ALTER TABLE `SubContratoMotorista` ADD CONSTRAINT `FK_SubContratoMotorista_Funcionario_FuncionarioId` FOREIGN KEY (`FuncionarioId`) REFERENCES `Funcionario` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230924020332_motoristas substitutos edit2') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230924020332_motoristas substitutos edit2', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231016190256_New table intermediária') THEN

    CREATE TABLE `SubContratoOnibus` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContratoId` int NOT NULL,
        `OnibusId` int NOT NULL,
        `DataInicial` datetime(6) NOT NULL,
        `DataFinal` datetime(6) NOT NULL,
        CONSTRAINT `PK_SubContratoOnibus` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_SubContratoOnibus_Contrato_ContratoId` FOREIGN KEY (`ContratoId`) REFERENCES `Contrato` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_SubContratoOnibus_Onibus_OnibusId` FOREIGN KEY (`OnibusId`) REFERENCES `Onibus` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231016190256_New table intermediária') THEN

    CREATE INDEX `IX_SubContratoOnibus_ContratoId` ON `SubContratoOnibus` (`ContratoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231016190256_New table intermediária') THEN

    CREATE INDEX `IX_SubContratoOnibus_OnibusId` ON `SubContratoOnibus` (`OnibusId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231016190256_New table intermediária') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20231016190256_New table intermediária', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231101200324_inclusão da flag de disponibilidade') THEN

    ALTER TABLE `Onibus` ADD `Disponibilidade` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231101200324_inclusão da flag de disponibilidade') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20231101200324_inclusão da flag de disponibilidade', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;

DROP PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`;

