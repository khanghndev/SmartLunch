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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `banners` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `PageName` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Title` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Subtitle` longtext CHARACTER SET utf8mb4 NULL,
        `ImageUrl` longtext CHARACTER SET utf8mb4 NOT NULL,
        `CtaLink` longtext CHARACTER SET utf8mb4 NULL,
        `DisplayOrder` int NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        `CreatedBy` int NULL,
        `UpdatedBy` int NULL,
        CONSTRAINT `PK_banners` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `cooking_methods` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` varchar(20) CHARACTER SET utf8mb4 NULL,
        `MethodKey` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `SortOrder` int NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_cooking_methods` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `customer_types` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` varchar(20) CHARACTER SET utf8mb4 NULL,
        `ProfileKey` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_customer_types` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `dish_categories` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` varchar(20) CHARACTER SET utf8mb4 NULL,
        `SlotKey` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `SortOrder` int NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_dish_categories` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `dish_values` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `Amount` decimal(12,2) NOT NULL,
        `Label` varchar(100) CHARACTER SET utf8mb4 NULL,
        `SortOrder` int NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_dish_values` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `ingredient_categories` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `NameEnglish` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(255) CHARACTER SET utf8mb4 NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_ingredient_categories` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `organizations` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Address` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Phone` varchar(50) CHARACTER SET utf8mb4 NULL,
        `ContactPerson` varchar(255) CHARACTER SET utf8mb4 NULL,
        `ContactEmail` varchar(255) CHARACTER SET utf8mb4 NULL,
        `TaxCode` longtext CHARACTER SET utf8mb4 NULL,
        `LegalRepresentative` longtext CHARACTER SET utf8mb4 NULL,
        `LogoUrl` longtext CHARACTER SET utf8mb4 NULL,
        `Website` longtext CHARACTER SET utf8mb4 NULL,
        `EducationLevel` longtext CHARACTER SET utf8mb4 NULL,
        `Type` longtext CHARACTER SET utf8mb4 NOT NULL,
        `IsSubscriptionActive` tinyint(1) NOT NULL,
        `DefaultDailyMeals` int NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        `CreatedBy` int NULL,
        `UpdatedBy` int NULL,
        CONSTRAINT `PK_organizations` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `partners` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `LegalName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `BusinessRegistrationNumber` varchar(100) CHARACTER SET utf8mb4 NULL,
        `TaxId` varchar(50) CHARACTER SET utf8mb4 NULL,
        `LegalRepresentative` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Address` varchar(255) CHARACTER SET utf8mb4 NULL,
        `ContactPerson` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Phone` varchar(50) CHARACTER SET utf8mb4 NULL,
        `Email` varchar(255) CHARACTER SET utf8mb4 NULL,
        `LogoUrl` longtext CHARACTER SET utf8mb4 NULL,
        `Website` longtext CHARACTER SET utf8mb4 NULL,
        `PerformanceRating` decimal(3,2) NULL,
        `ComplianceInfo` varchar(2000) CHARACTER SET utf8mb4 NULL,
        `FinancialTerms` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_partners` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `permissions` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
        `Resource` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Action` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        `CreatedBy` int NULL,
        `UpdatedBy` int NULL,
        CONSTRAINT `PK_permissions` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `promotions` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` varchar(40) CHARACTER SET utf8mb4 NULL,
        `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
        `ScopeType` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
        `DiscountType` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `DiscountValue` decimal(12,2) NOT NULL,
        `Priority` int NOT NULL,
        `SelectionMode` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `Channel` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `MinOrderQuantity` int NULL,
        `MinOrderAmount` decimal(12,2) NULL,
        `ValidFrom` date NOT NULL,
        `ValidTo` date NOT NULL,
        `BookingTimeStart` time(6) NULL,
        `BookingTimeEnd` time(6) NULL,
        `MaxTotalUses` int NULL,
        `MaxUsesPerUser` int NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_promotions` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `recruitment` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `Title` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Position` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Location` longtext CHARACTER SET utf8mb4 NULL,
        `JobType` longtext CHARACTER SET utf8mb4 NOT NULL,
        `SalaryRange` longtext CHARACTER SET utf8mb4 NULL,
        `Description` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Requirements` longtext CHARACTER SET utf8mb4 NULL,
        `Benefits` longtext CHARACTER SET utf8mb4 NULL,
        `Deadline` datetime(6) NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        `CreatedBy` int NULL,
        `UpdatedBy` int NULL,
        CONSTRAINT `PK_recruitment` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `roles` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        `IsSystemRole` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        `CreatedBy` int NULL,
        `UpdatedBy` int NULL,
        CONSTRAINT `PK_roles` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `system_backup_schedule` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `IsEnabled` tinyint(1) NOT NULL,
        `ScheduleMode` varchar(16) CHARACTER SET utf8mb4 NOT NULL,
        `TimeOfDayMinutes` int NOT NULL,
        `DayOfWeek` int NULL,
        `OnceScheduledAt` datetime(6) NULL,
        `LastRunAt` datetime(6) NULL,
        `NextRunAt` datetime(6) NULL,
        `UpdatedAt` datetime(6) NOT NULL,
        `UpdatedByUserId` int NULL,
        CONSTRAINT `PK_system_backup_schedule` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `system_backups` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `FileName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `StorageBucket` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `StorageObjectName` varchar(1024) CHARACTER SET utf8mb4 NOT NULL,
        `SizeBytes` bigint NOT NULL,
        `BackupSource` varchar(16) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'Manual',
        `CreatedAtUtc` datetime(6) NOT NULL,
        `RestoredAtUtc` datetime(6) NULL,
        `DeletedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_system_backups` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `system_logs` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Timestamp` datetime(6) NULL,
        `Level` varchar(50) CHARACTER SET utf8mb4 NULL,
        `Template` text CHARACTER SET utf8mb4 NULL,
        `Message` text CHARACTER SET utf8mb4 NULL,
        `Exception` text CHARACTER SET utf8mb4 NULL,
        `Properties` text CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_system_logs` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `transactions` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `Date` datetime(6) NOT NULL,
        `Description` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Amount` decimal(12,2) NOT NULL,
        `Category` varchar(100) CHARACTER SET utf8mb4 NULL,
        `Method` varchar(50) CHARACTER SET utf8mb4 NULL,
        `ReferenceId` int NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_transactions` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `users` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `Username` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Email` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `PasswordHash` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
        `FirstName` varchar(100) CHARACTER SET utf8mb4 NULL,
        `LastName` varchar(100) CHARACTER SET utf8mb4 NULL,
        `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 NULL,
        `AvatarUrl` longtext CHARACTER SET utf8mb4 NULL,
        `AvatarMediaFileId` int NULL,
        `Gender` longtext CHARACTER SET utf8mb4 NULL,
        `BirthDate` datetime(6) NULL,
        `Address` longtext CHARACTER SET utf8mb4 NULL,
        `Provider` varchar(50) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'system',
        `IsActive` tinyint(1) NOT NULL,
        `IsEmailVerified` tinyint(1) NOT NULL,
        `EmailVerifiedAt` datetime(6) NULL,
        `LastLoginAt` datetime(6) NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        `CreatedBy` int NULL,
        `UpdatedBy` int NULL,
        CONSTRAINT `PK_users` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `dishes` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `NameEnglish` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Description` varchar(255) CHARACTER SET utf8mb4 NULL,
        `CookingMethodId` int NULL,
        `Price` decimal(10,2) NOT NULL,
        `DietaryLabel` varchar(50) CHARACTER SET utf8mb4 NULL,
        `ImageUrl` longtext CHARACTER SET utf8mb4 NULL,
        `Calories` decimal(65,30) NULL,
        `Protein` decimal(65,30) NULL,
        `Fat` decimal(65,30) NULL,
        `Carbs` decimal(65,30) NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_dishes` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_dishes_cooking_methods_CookingMethodId` FOREIGN KEY (`CookingMethodId`) REFERENCES `cooking_methods` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `contracts` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `PartnerId` int NOT NULL,
        `OrganizationId` int NULL,
        `SourceOrderId` int NULL,
        `ContractNumber` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ContractType` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `SupplySchedule` varchar(500) CHARACTER SET utf8mb4 NULL,
        `StartDate` datetime(6) NOT NULL,
        `EndDate` datetime(6) NULL,
        `TotalValue` decimal(12,2) NULL,
        `DishValueId` int NULL,
        `MealUnitPrice` decimal(12,2) NULL,
        `MealsPerDay` int NULL,
        `DepositAmount` decimal(12,2) NULL,
        `LastWeeklyReminderWeekStart` date NULL,
        `WeeklyAutoFillWeekStart` date NULL,
        `ContractFileUrl` longtext CHARACTER SET utf8mb4 NULL,
        `IsDigitallySigned` tinyint(1) NOT NULL,
        `DigitalSignature` longtext CHARACTER SET utf8mb4 NULL,
        `DigitallySignedAt` datetime(6) NULL,
        `SignatureImage` longtext CHARACTER SET utf8mb4 NULL,
        `Status` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_contracts` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_contracts_dish_values_DishValueId` FOREIGN KEY (`DishValueId`) REFERENCES `dish_values` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_contracts_organizations_OrganizationId` FOREIGN KEY (`OrganizationId`) REFERENCES `organizations` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_contracts_partners_PartnerId` FOREIGN KEY (`PartnerId`) REFERENCES `partners` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `ingredients` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `NameEnglish` longtext CHARACTER SET utf8mb4 NULL,
        `Unit` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(255) CHARACTER SET utf8mb4 NULL,
        `DefaultSupplierId` int NULL,
        `CostPerUnit` decimal(10,2) NULL,
        `CategoryId` int NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_ingredients` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ingredients_ingredient_categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `ingredient_categories` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_ingredients_partners_DefaultSupplierId` FOREIGN KEY (`DefaultSupplierId`) REFERENCES `partners` (`Id`) ON DELETE SET NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `promotion_targets` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `PromotionId` int NOT NULL,
        `TargetType` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
        `TargetId` int NULL,
        `TargetKey` varchar(50) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_promotion_targets` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_promotion_targets_promotions_PromotionId` FOREIGN KEY (`PromotionId`) REFERENCES `promotions` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `role_permissions` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `RoleId` int NOT NULL,
        `PermissionId` int NOT NULL,
        `AssignedAt` datetime(6) NOT NULL,
        `AssignedBy` int NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_role_permissions` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_role_permissions_permissions_PermissionId` FOREIGN KEY (`PermissionId`) REFERENCES `permissions` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_role_permissions_roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `chatbot_logs` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `UserId` int NULL,
        `Message` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Response` longtext CHARACTER SET utf8mb4 NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_chatbot_logs` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_chatbot_logs_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `contact_inquiries` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `UserId` int NULL,
        `FullName` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Phone` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
        `Email` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `InterestedService` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Message` longtext CHARACTER SET utf8mb4 NULL,
        `Status` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `ManagerReply` longtext CHARACTER SET utf8mb4 NULL,
        `RepliedAt` datetime(6) NULL,
        `RepliedByUserId` int NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_contact_inquiries` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_contact_inquiries_users_RepliedByUserId` FOREIGN KEY (`RepliedByUserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_contact_inquiries_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `ingredient_intake_proposals` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `ProposalCode` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
        `Status` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `HeaderNote` varchar(500) CHARACTER SET utf8mb4 NULL,
        `CreatedByUserId` int NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `ReviewedByUserId` int NULL,
        `ReviewedAt` datetime(6) NULL,
        `ReviewNote` varchar(500) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_ingredient_intake_proposals` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ingredient_intake_proposals_users_CreatedByUserId` FOREIGN KEY (`CreatedByUserId`) REFERENCES `users` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_ingredient_intake_proposals_users_ReviewedByUserId` FOREIGN KEY (`ReviewedByUserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `internal_stock_issues` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `IssueCode` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
        `IssuedAt` datetime(6) NOT NULL,
        `Reason` varchar(500) CHARACTER SET utf8mb4 NULL,
        `CreatedByUserId` int NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_internal_stock_issues` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_internal_stock_issues_users_CreatedByUserId` FOREIGN KEY (`CreatedByUserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `media_files` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `OwnerUserId` int NOT NULL,
        `Bucket` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ObjectName` varchar(1024) CHARACTER SET utf8mb4 NOT NULL,
        `OriginalFileName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `ContentType` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `SizeBytes` bigint NOT NULL,
        `Md5HashBase64` varchar(128) CHARACTER SET utf8mb4 NULL,
        `MediaType` varchar(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'unknown',
        `IsPublic` tinyint(1) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_media_files` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_media_files_users_OwnerUserId` FOREIGN KEY (`OwnerUserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `menu_suggestions` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `WeekStart` datetime(6) NOT NULL,
        `GeneratedAt` datetime(6) NOT NULL,
        `Version` int NOT NULL DEFAULT 1,
        `RulesKey` varchar(50) CHARACTER SET utf8mb4 NULL,
        `BudgetPerServing` decimal(65,30) NULL,
        `TopK` int NULL,
        `TimeLimitSeconds` decimal(65,30) NULL,
        `PlanCount` int NULL,
        `SuggestionText` longtext CHARACTER SET utf8mb4 NOT NULL,
        `AlgorithmVersion` varchar(50) CHARACTER SET utf8mb4 NULL,
        `CreatedBy` int NULL,
        CONSTRAINT `PK_menu_suggestions` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_menu_suggestions_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `news` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `Title` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Slug` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Summary` longtext CHARACTER SET utf8mb4 NULL,
        `Content` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ThumbnailUrl` longtext CHARACTER SET utf8mb4 NULL,
        `Category` longtext CHARACTER SET utf8mb4 NULL,
        `AuthorId` int NULL,
        `IsPublished` tinyint(1) NOT NULL,
        `PublishedAt` datetime(6) NULL,
        `ViewCount` int NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedBy` int NULL,
        `UpdatedBy` int NULL,
        CONSTRAINT `PK_news` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_news_users_AuthorId` FOREIGN KEY (`AuthorId`) REFERENCES `users` (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `notifications` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `UserId` int NOT NULL,
        `Title` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Message` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Type` longtext CHARACTER SET utf8mb4 NOT NULL,
        `IsRead` tinyint(1) NOT NULL,
        `Link` longtext CHARACTER SET utf8mb4 NULL,
        `SendAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_notifications` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_notifications_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `user_organizations` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `UserId` int NOT NULL,
        `OrganizationId` int NOT NULL,
        `JoinedAt` datetime(6) NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_user_organizations` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_user_organizations_organizations_OrganizationId` FOREIGN KEY (`OrganizationId`) REFERENCES `organizations` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_user_organizations_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `user_permissions` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `UserId` int NOT NULL,
        `PermissionId` int NOT NULL,
        `AssignedAt` datetime(6) NOT NULL,
        `AssignedBy` int NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_user_permissions` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_user_permissions_permissions_PermissionId` FOREIGN KEY (`PermissionId`) REFERENCES `permissions` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_user_permissions_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `user_roles` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `UserId` int NOT NULL,
        `RoleId` int NOT NULL,
        `AssignedAt` datetime(6) NOT NULL,
        `AssignedBy` int NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_user_roles` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_user_roles_roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_user_roles_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `user_tokens` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `UserId` int NOT NULL,
        `AccessToken` varchar(2000) CHARACTER SET utf8mb4 NOT NULL,
        `RefreshToken` varchar(2000) CHARACTER SET utf8mb4 NOT NULL,
        `IssuedAt` datetime(6) NOT NULL,
        `ExpiresAt` datetime(6) NOT NULL,
        `RevokedAt` datetime(6) NULL,
        `ReplacedByToken` varchar(2000) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        `Jti` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_user_tokens` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_user_tokens_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `weekly_menus` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `StartDate` datetime(6) NOT NULL,
        `EndDate` datetime(6) NOT NULL,
        `MenuType` longtext CHARACTER SET utf8mb4 NOT NULL,
        `CustomerTypeId` int NULL,
        `Description` varchar(255) CHARACTER SET utf8mb4 NULL,
        `CreatedBy` int NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_weekly_menus` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_weekly_menus_customer_types_CustomerTypeId` FOREIGN KEY (`CustomerTypeId`) REFERENCES `customer_types` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_weekly_menus_users_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `dish_dish_categories` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` varchar(20) CHARACTER SET utf8mb4 NULL,
        `DishId` int NOT NULL,
        `DishCategoryId` int NOT NULL,
        CONSTRAINT `PK_dish_dish_categories` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_dish_dish_categories_dish_categories_DishCategoryId` FOREIGN KEY (`DishCategoryId`) REFERENCES `dish_categories` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_dish_dish_categories_dishes_DishId` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `contract_daily_meal_portions` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `ServiceDate` date NOT NULL,
        `MealCount` int NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_contract_daily_meal_portions` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_contract_daily_meal_portions_contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `contracts` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `contract_excluded_dates` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `ExcludedDate` date NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_contract_excluded_dates` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_contract_excluded_dates_contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `contracts` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `orders` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `UserId` int NULL,
        `ContractId` int NULL,
        `OrderDate` datetime(6) NOT NULL,
        `ScheduledDate` datetime(6) NOT NULL,
        `Status` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `TotalAmount` decimal(12,2) NOT NULL,
        `SubtotalAmount` decimal(12,2) NULL,
        `DiscountAmount` decimal(12,2) NOT NULL,
        `PaymentStatus` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        `InvoiceCode` varchar(40) CHARACTER SET utf8mb4 NULL,
        `CreatedBySalesUserId` int NULL,
        `AnnexPdfUrl` varchar(2048) CHARACTER SET utf8mb4 NULL,
        `AnnexSignedAt` datetime(6) NULL,
        `RecipientName` varchar(120) CHARACTER SET utf8mb4 NULL,
        `RecipientPhone` varchar(20) CHARACTER SET utf8mb4 NULL,
        `RecipientEmail` varchar(254) CHARACTER SET utf8mb4 NULL,
        `DeliveryAddress` varchar(500) CHARACTER SET utf8mb4 NULL,
        `DeliveryWardDistrict` varchar(255) CHARACTER SET utf8mb4 NULL,
        `DeliveryNotes` varchar(500) CHARACTER SET utf8mb4 NULL,
        `PreferredDeliveryTime` varchar(32) CHARACTER SET utf8mb4 NULL,
        `OrderConfirmationEmailSentAt` datetime(6) NULL,
        `PaymentReminderSentAt` datetime(6) NULL,
        CONSTRAINT `PK_orders` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_orders_contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `contracts` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_orders_users_CreatedBySalesUserId` FOREIGN KEY (`CreatedBySalesUserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_orders_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `partner_payments` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `ContractId` int NOT NULL,
        `PartnerId` int NOT NULL,
        `PaymentDate` datetime(6) NOT NULL,
        `Amount` decimal(12,2) NOT NULL,
        `Method` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
        `Status` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_partner_payments` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_partner_payments_contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `contracts` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_partner_payments_partners_PartnerId` FOREIGN KEY (`PartnerId`) REFERENCES `partners` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `dish_ingredients` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `DishId` int NOT NULL,
        `IngredientId` int NOT NULL,
        `DishValueId` int NOT NULL,
        `Quantity` decimal(10,2) NOT NULL,
        `Unit` varchar(20) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_dish_ingredients` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_dish_ingredients_dish_values_DishValueId` FOREIGN KEY (`DishValueId`) REFERENCES `dish_values` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_dish_ingredients_dishes_DishId` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_dish_ingredients_ingredients_IngredientId` FOREIGN KEY (`IngredientId`) REFERENCES `ingredients` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `ingredient_sources` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `IngredientId` int NOT NULL,
        `PartnerId` int NULL,
        `BatchNumber` varchar(50) CHARACTER SET utf8mb4 NULL,
        `OriginDetails` varchar(255) CHARACTER SET utf8mb4 NULL,
        `ProductionDate` datetime(6) NULL,
        `ExpirationDate` datetime(6) NULL,
        `Certification` varchar(255) CHARACTER SET utf8mb4 NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_ingredient_sources` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ingredient_sources_ingredients_IngredientId` FOREIGN KEY (`IngredientId`) REFERENCES `ingredients` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ingredient_sources_partners_PartnerId` FOREIGN KEY (`PartnerId`) REFERENCES `partners` (`Id`) ON DELETE SET NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `inventory` (
        `IngredientId` int NOT NULL,
        `QuantityAvailable` decimal(12,2) NOT NULL,
        `ReorderLevel` decimal(12,2) NULL,
        `LastUpdated` datetime(6) NOT NULL,
        CONSTRAINT `PK_inventory` PRIMARY KEY (`IngredientId`),
        CONSTRAINT `FK_inventory_ingredients_IngredientId` FOREIGN KEY (`IngredientId`) REFERENCES `ingredients` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `ingredient_actual_intakes` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `ReceiptCode` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
        `ProposalId` int NOT NULL,
        `CreatedByUserId` int NOT NULL,
        `ReceivedAt` datetime(6) NOT NULL,
        `Note` varchar(500) CHARACTER SET utf8mb4 NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_ingredient_actual_intakes` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ingredient_actual_intakes_ingredient_intake_proposals_Propos~` FOREIGN KEY (`ProposalId`) REFERENCES `ingredient_intake_proposals` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_ingredient_actual_intakes_users_CreatedByUserId` FOREIGN KEY (`CreatedByUserId`) REFERENCES `users` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `ingredient_intake_proposal_lines` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `ProposalId` int NOT NULL,
        `IngredientId` int NOT NULL,
        `Quantity` decimal(12,2) NOT NULL,
        `LineNote` varchar(255) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_ingredient_intake_proposal_lines` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ingredient_intake_proposal_lines_ingredient_intake_proposals~` FOREIGN KEY (`ProposalId`) REFERENCES `ingredient_intake_proposals` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ingredient_intake_proposal_lines_ingredients_IngredientId` FOREIGN KEY (`IngredientId`) REFERENCES `ingredients` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `internal_stock_issue_lines` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `IssueId` int NOT NULL,
        `IngredientId` int NOT NULL,
        `Quantity` decimal(12,2) NOT NULL,
        CONSTRAINT `PK_internal_stock_issue_lines` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_internal_stock_issue_lines_ingredients_IngredientId` FOREIGN KEY (`IngredientId`) REFERENCES `ingredients` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_internal_stock_issue_lines_internal_stock_issues_IssueId` FOREIGN KEY (`IssueId`) REFERENCES `internal_stock_issues` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `company_public_documents` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Title` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `DocumentType` varchar(50) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'other',
        `Description` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `MediaFileId` int NOT NULL,
        `SortOrder` int NOT NULL,
        `IsPublished` tinyint(1) NOT NULL DEFAULT TRUE,
        `IssuedDate` datetime(6) NULL,
        `ExpiryDate` datetime(6) NULL,
        `CreatedByUserId` int NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_company_public_documents` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_company_public_documents_media_files_MediaFileId` FOREIGN KEY (`MediaFileId`) REFERENCES `media_files` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `dish_images` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` varchar(20) CHARACTER SET utf8mb4 NULL,
        `DishId` int NOT NULL,
        `MediaFileId` int NOT NULL,
        `Role` varchar(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'gallery',
        `SortOrder` int NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_dish_images` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_dish_images_dishes_DishId` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_dish_images_media_files_MediaFileId` FOREIGN KEY (`MediaFileId`) REFERENCES `media_files` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `organization_legal_documents` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `OrganizationId` int NOT NULL,
        `Title` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `DocumentType` varchar(50) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'other',
        `Description` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `MediaFileId` int NOT NULL,
        `IssuedDate` datetime(6) NULL,
        `ExpiryDate` datetime(6) NULL,
        `UploadedByUserId` int NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_organization_legal_documents` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_organization_legal_documents_media_files_MediaFileId` FOREIGN KEY (`MediaFileId`) REFERENCES `media_files` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_organization_legal_documents_organizations_OrganizationId` FOREIGN KEY (`OrganizationId`) REFERENCES `organizations` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `partner_documents` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` varchar(20) CHARACTER SET utf8mb4 NULL,
        `PartnerId` int NOT NULL,
        `MediaFileId` int NOT NULL,
        `DocumentType` varchar(50) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'other',
        `IsVerified` tinyint(1) NOT NULL DEFAULT FALSE,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_partner_documents` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_partner_documents_media_files_MediaFileId` FOREIGN KEY (`MediaFileId`) REFERENCES `media_files` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_partner_documents_partners_PartnerId` FOREIGN KEY (`PartnerId`) REFERENCES `partners` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `menu_suggestion_plans` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `MenuSuggestionId` int NOT NULL,
        `Rank` int NOT NULL,
        `PlanScore` decimal(6,3) NOT NULL,
        `ObjectiveValue` decimal(18,3) NOT NULL,
        CONSTRAINT `PK_menu_suggestion_plans` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_menu_suggestion_plans_menu_suggestions_MenuSuggestionId` FOREIGN KEY (`MenuSuggestionId`) REFERENCES `menu_suggestions` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `menu_schedule` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `MenuId` int NOT NULL,
        `Date` datetime(6) NOT NULL,
        `MealSlot` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `DishId` int NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_menu_schedule` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_menu_schedule_dishes_DishId` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_menu_schedule_weekly_menus_MenuId` FOREIGN KEY (`MenuId`) REFERENCES `weekly_menus` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `weekly_menu_images` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` varchar(20) CHARACTER SET utf8mb4 NULL,
        `WeeklyMenuId` int NOT NULL,
        `MediaFileId` int NOT NULL,
        `Role` varchar(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'gallery',
        `SortOrder` int NOT NULL DEFAULT 0,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_weekly_menu_images` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_weekly_menu_images_media_files_MediaFileId` FOREIGN KEY (`MediaFileId`) REFERENCES `media_files` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_weekly_menu_images_weekly_menus_WeeklyMenuId` FOREIGN KEY (`WeeklyMenuId`) REFERENCES `weekly_menus` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `contract_weekly_selections` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `WeekMonday` date NOT NULL,
        `Status` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `FulfillmentOrderId` int NULL,
        `SelectedAt` datetime(6) NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_contract_weekly_selections` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_contract_weekly_selections_contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `contracts` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_contract_weekly_selections_orders_FulfillmentOrderId` FOREIGN KEY (`FulfillmentOrderId`) REFERENCES `orders` (`Id`) ON DELETE SET NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `deliveries` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `OrderId` int NOT NULL,
        `AssignedStaffId` int NULL,
        `DeliveryAddress` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `DeliveryStatus` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `DeliveredAt` datetime(6) NULL,
        `ProofImageUrl` longtext CHARACTER SET utf8mb4 NULL,
        `ProofCapturedAt` datetime(6) NULL,
        `Notes` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RecipientConfirmedName` varchar(200) CHARACTER SET utf8mb4 NULL,
        `RecipientConfirmationCode` varchar(20) CHARACTER SET utf8mb4 NULL,
        `RecipientSignatureUrl` varchar(500) CHARACTER SET utf8mb4 NULL,
        `HandoverDocumentUrl` varchar(500) CHARACTER SET utf8mb4 NULL,
        `ShipperSignatureUrl` varchar(500) CHARACTER SET utf8mb4 NULL,
        `RecipientConfirmedAt` datetime(6) NULL,
        `DeliveryOtp` varchar(6) CHARACTER SET utf8mb4 NULL,
        `DeliveryOtpExpiresAt` datetime(6) NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_deliveries` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_deliveries_orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_deliveries_users_AssignedStaffId` FOREIGN KEY (`AssignedStaffId`) REFERENCES `users` (`Id`) ON DELETE SET NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `order_items` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `OrderId` int NOT NULL,
        `DishId` int NOT NULL,
        `Quantity` int NOT NULL,
        `ServiceDate` date NULL,
        `UnitPrice` decimal(10,2) NOT NULL,
        `TotalPrice` decimal(12,2) NOT NULL,
        CONSTRAINT `PK_order_items` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_order_items_dishes_DishId` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_order_items_orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `order_promotion_applications` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `OrderId` int NOT NULL,
        `PromotionId` int NOT NULL,
        `PromotionCode` varchar(40) CHARACTER SET utf8mb4 NULL,
        `PromotionName` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `ScopeType` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
        `DiscountType` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `DiscountValue` decimal(12,2) NOT NULL,
        `SubtotalBefore` decimal(12,2) NOT NULL,
        `DiscountAmount` decimal(12,2) NOT NULL,
        `TotalAfter` decimal(12,2) NOT NULL,
        `SnapshotJson` longtext CHARACTER SET utf8mb4 NULL,
        `AppliedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_order_promotion_applications` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_order_promotion_applications_orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_order_promotion_applications_promotions_PromotionId` FOREIGN KEY (`PromotionId`) REFERENCES `promotions` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `payments` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `OrderId` int NOT NULL,
        `PayerId` int NULL,
        `PaymentDate` datetime(6) NOT NULL,
        `Amount` decimal(12,2) NOT NULL,
        `Method` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
        `Status` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_payments` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_payments_orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_payments_users_PayerId` FOREIGN KEY (`PayerId`) REFERENCES `users` (`Id`) ON DELETE SET NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `reviews` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `UserId` int NOT NULL,
        `DishId` int NULL,
        `OrderId` int NULL,
        `Rating` int NOT NULL,
        `Comment` longtext CHARACTER SET utf8mb4 NULL,
        `ManagerReply` longtext CHARACTER SET utf8mb4 NULL,
        `RepliedAt` datetime(6) NULL,
        `RepliedByUserId` int NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_reviews` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_reviews_dishes_DishId` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_reviews_orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_reviews_users_RepliedByUserId` FOREIGN KEY (`RepliedByUserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_reviews_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `ingredient_actual_intake_lines` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `IntakeId` int NOT NULL,
        `IngredientId` int NOT NULL,
        `Quantity` decimal(12,2) NOT NULL,
        CONSTRAINT `PK_ingredient_actual_intake_lines` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ingredient_actual_intake_lines_ingredient_actual_intakes_Int~` FOREIGN KEY (`IntakeId`) REFERENCES `ingredient_actual_intakes` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ingredient_actual_intake_lines_ingredients_IngredientId` FOREIGN KEY (`IngredientId`) REFERENCES `ingredients` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `menu_suggestion_plan_days` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `MenuSuggestionPlanId` int NOT NULL,
        `DayIndex` tinyint unsigned NOT NULL,
        `DayName` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_menu_suggestion_plan_days` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_menu_suggestion_plan_days_menu_suggestion_plans_MenuSuggesti~` FOREIGN KEY (`MenuSuggestionPlanId`) REFERENCES `menu_suggestion_plans` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `contract_weekly_selection_items` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `WeeklySelectionId` int NOT NULL,
        `ServiceDate` date NOT NULL,
        `DishId` int NOT NULL,
        `Quantity` int NOT NULL,
        CONSTRAINT `PK_contract_weekly_selection_items` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_contract_weekly_selection_items_contract_weekly_selections_W~` FOREIGN KEY (`WeeklySelectionId`) REFERENCES `contract_weekly_selections` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_contract_weekly_selection_items_dishes_DishId` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `complaints` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `UserId` int NOT NULL,
        `OrderId` int NULL,
        `Title` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Description` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Reason` varchar(40) CHARACTER SET utf8mb4 NULL,
        `MissingPortionCount` int NULL,
        `Status` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
        `Resolution` varchar(20) CHARACTER SET utf8mb4 NULL,
        `ResolutionNote` longtext CHARACTER SET utf8mb4 NULL,
        `AssignedTo` int NULL,
        `ResolvedByUserId` int NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `SubmittedAt` datetime(6) NULL,
        `ComplaintDeadlineAt` datetime(6) NULL,
        `RefundPortionCount` int NULL,
        `SuggestedRefundAmount` decimal(18,2) NULL,
        `FinalRefundAmount` decimal(18,2) NULL,
        `RefundPaymentId` int NULL,
        `ResolvedAt` datetime(6) NULL,
        CONSTRAINT `PK_complaints` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_complaints_orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_complaints_payments_RefundPaymentId` FOREIGN KEY (`RefundPaymentId`) REFERENCES `payments` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_complaints_users_AssignedTo` FOREIGN KEY (`AssignedTo`) REFERENCES `users` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_complaints_users_ResolvedByUserId` FOREIGN KEY (`ResolvedByUserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_complaints_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `sentiments` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NULL,
        `ReviewId` int NOT NULL,
        `SentimentLabel` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `Confidence` decimal(4,2) NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_sentiments` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_sentiments_reviews_ReviewId` FOREIGN KEY (`ReviewId`) REFERENCES `reviews` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `menu_suggestion_plan_items` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `MenuSuggestionPlanDayId` int NOT NULL,
        `SlotCategory` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `DishId` int NULL,
        `DishName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `DishSourceCategory` varchar(20) CHARACTER SET utf8mb4 NULL,
        `Score` decimal(6,3) NOT NULL,
        `CostPerServing` decimal(10,2) NOT NULL,
        `ReasonsJson` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_menu_suggestion_plan_items` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_menu_suggestion_plan_items_menu_suggestion_plan_days_MenuSug~` FOREIGN KEY (`MenuSuggestionPlanDayId`) REFERENCES `menu_suggestion_plan_days` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE TABLE `complaint_evidence` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ComplaintId` int NOT NULL,
        `Kind` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
        `MediaType` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `StorageObjectName` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
        `ContentType` varchar(100) CHARACTER SET utf8mb4 NULL,
        `FileSizeBytes` bigint NULL,
        `SortOrder` int NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        CONSTRAINT `PK_complaint_evidence` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_complaint_evidence_complaints_ComplaintId` FOREIGN KEY (`ComplaintId`) REFERENCES `complaints` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_banners_CreatedBy` ON `banners` (`CreatedBy`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_banners_IsActive` ON `banners` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_chatbot_logs_CreatedAt` ON `chatbot_logs` (`CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_chatbot_logs_UserId` ON `chatbot_logs` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_company_public_documents_IsPublished_SortOrder` ON `company_public_documents` (`IsPublished`, `SortOrder`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_company_public_documents_MediaFileId` ON `company_public_documents` (`MediaFileId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_complaint_evidence_ComplaintId` ON `complaint_evidence` (`ComplaintId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_complaints_AssignedTo` ON `complaints` (`AssignedTo`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_complaints_OrderId` ON `complaints` (`OrderId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_complaints_RefundPaymentId` ON `complaints` (`RefundPaymentId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_complaints_ResolvedByUserId` ON `complaints` (`ResolvedByUserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_complaints_Status` ON `complaints` (`Status`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_complaints_UserId` ON `complaints` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_contact_inquiries_CreatedAt` ON `contact_inquiries` (`CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_contact_inquiries_RepliedByUserId` ON `contact_inquiries` (`RepliedByUserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_contact_inquiries_Status` ON `contact_inquiries` (`Status`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_contact_inquiries_UserId` ON `contact_inquiries` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_contract_daily_meal_portions_ContractId_ServiceDate` ON `contract_daily_meal_portions` (`ContractId`, `ServiceDate`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_contract_excluded_dates_ContractId_ExcludedDate` ON `contract_excluded_dates` (`ContractId`, `ExcludedDate`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_contract_weekly_selection_items_DishId` ON `contract_weekly_selection_items` (`DishId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_contract_weekly_selection_items_WeeklySelectionId_ServiceDat~` ON `contract_weekly_selection_items` (`WeeklySelectionId`, `ServiceDate`, `DishId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_contract_weekly_selections_ContractId_WeekMonday` ON `contract_weekly_selections` (`ContractId`, `WeekMonday`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_contract_weekly_selections_FulfillmentOrderId` ON `contract_weekly_selections` (`FulfillmentOrderId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_contracts_DishValueId` ON `contracts` (`DishValueId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_contracts_OrganizationId` ON `contracts` (`OrganizationId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_contracts_PartnerId` ON `contracts` (`PartnerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_contracts_StartDate_EndDate` ON `contracts` (`StartDate`, `EndDate`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_contracts_Status` ON `contracts` (`Status`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_cooking_methods_Code` ON `cooking_methods` (`Code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_cooking_methods_MethodKey` ON `cooking_methods` (`MethodKey`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_customer_types_Code` ON `customer_types` (`Code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_customer_types_ProfileKey` ON `customer_types` (`ProfileKey`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_deliveries_AssignedStaffId` ON `deliveries` (`AssignedStaffId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_deliveries_OrderId` ON `deliveries` (`OrderId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_dish_categories_Code` ON `dish_categories` (`Code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_dish_categories_SlotKey` ON `dish_categories` (`SlotKey`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_dish_dish_categories_Code` ON `dish_dish_categories` (`Code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_dish_dish_categories_DishCategoryId` ON `dish_dish_categories` (`DishCategoryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_dish_dish_categories_DishId_DishCategoryId` ON `dish_dish_categories` (`DishId`, `DishCategoryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_dish_images_Code` ON `dish_images` (`Code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_dish_images_DishId_SortOrder` ON `dish_images` (`DishId`, `SortOrder`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_dish_images_MediaFileId` ON `dish_images` (`MediaFileId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_dish_ingredients_DishId` ON `dish_ingredients` (`DishId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_dish_ingredients_DishId_IngredientId_DishValueId` ON `dish_ingredients` (`DishId`, `IngredientId`, `DishValueId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_dish_ingredients_DishValueId` ON `dish_ingredients` (`DishValueId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_dish_ingredients_IngredientId` ON `dish_ingredients` (`IngredientId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_dish_values_Amount` ON `dish_values` (`Amount`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_dishes_CookingMethodId` ON `dishes` (`CookingMethodId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_dishes_IsActive` ON `dishes` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_dishes_Name` ON `dishes` (`Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_actual_intake_lines_IngredientId` ON `ingredient_actual_intake_lines` (`IngredientId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_actual_intake_lines_IntakeId` ON `ingredient_actual_intake_lines` (`IntakeId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_actual_intakes_CreatedByUserId` ON `ingredient_actual_intakes` (`CreatedByUserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_ingredient_actual_intakes_ProposalId` ON `ingredient_actual_intakes` (`ProposalId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_ingredient_actual_intakes_ReceiptCode` ON `ingredient_actual_intakes` (`ReceiptCode`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_actual_intakes_ReceivedAt` ON `ingredient_actual_intakes` (`ReceivedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_ingredient_categories_Code` ON `ingredient_categories` (`Code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_ingredient_categories_NameEnglish` ON `ingredient_categories` (`NameEnglish`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_intake_proposal_lines_IngredientId` ON `ingredient_intake_proposal_lines` (`IngredientId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_intake_proposal_lines_ProposalId` ON `ingredient_intake_proposal_lines` (`ProposalId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_intake_proposals_CreatedAt` ON `ingredient_intake_proposals` (`CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_intake_proposals_CreatedByUserId` ON `ingredient_intake_proposals` (`CreatedByUserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_ingredient_intake_proposals_ProposalCode` ON `ingredient_intake_proposals` (`ProposalCode`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_intake_proposals_ReviewedByUserId` ON `ingredient_intake_proposals` (`ReviewedByUserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_intake_proposals_Status` ON `ingredient_intake_proposals` (`Status`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_sources_ExpirationDate` ON `ingredient_sources` (`ExpirationDate`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_sources_IngredientId` ON `ingredient_sources` (`IngredientId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredient_sources_PartnerId` ON `ingredient_sources` (`PartnerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredients_CategoryId` ON `ingredients` (`CategoryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredients_DefaultSupplierId` ON `ingredients` (`DefaultSupplierId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_ingredients_IsActive` ON `ingredients` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_ingredients_Name` ON `ingredients` (`Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_internal_stock_issue_lines_IngredientId` ON `internal_stock_issue_lines` (`IngredientId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_internal_stock_issue_lines_IssueId` ON `internal_stock_issue_lines` (`IssueId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_internal_stock_issues_CreatedByUserId` ON `internal_stock_issues` (`CreatedByUserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_internal_stock_issues_IssueCode` ON `internal_stock_issues` (`IssueCode`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_internal_stock_issues_IssuedAt` ON `internal_stock_issues` (`IssuedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_media_files_Bucket_ObjectName` ON `media_files` (`Bucket`, `ObjectName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_media_files_MediaType` ON `media_files` (`MediaType`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_media_files_OwnerUserId` ON `media_files` (`OwnerUserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_media_files_OwnerUserId_CreatedAt` ON `media_files` (`OwnerUserId`, `CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_menu_schedule_Date` ON `menu_schedule` (`Date`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_menu_schedule_DishId` ON `menu_schedule` (`DishId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_menu_schedule_MenuId_Date_MealSlot_DishId` ON `menu_schedule` (`MenuId`, `Date`, `MealSlot`, `DishId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_menu_suggestion_plan_days_MenuSuggestionPlanId` ON `menu_suggestion_plan_days` (`MenuSuggestionPlanId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_menu_suggestion_plan_items_MenuSuggestionPlanDayId` ON `menu_suggestion_plan_items` (`MenuSuggestionPlanDayId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_menu_suggestion_plans_MenuSuggestionId` ON `menu_suggestion_plans` (`MenuSuggestionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_menu_suggestions_CreatedBy` ON `menu_suggestions` (`CreatedBy`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_menu_suggestions_WeekStart` ON `menu_suggestions` (`WeekStart`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_news_AuthorId` ON `news` (`AuthorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_news_CreatedBy` ON `news` (`CreatedBy`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_news_IsActive` ON `news` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_notifications_IsRead` ON `notifications` (`IsRead`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_notifications_SendAt` ON `notifications` (`SendAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_notifications_UserId` ON `notifications` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_order_items_DishId` ON `order_items` (`DishId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_order_items_OrderId` ON `order_items` (`OrderId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_order_promotion_applications_OrderId` ON `order_promotion_applications` (`OrderId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_order_promotion_applications_PromotionId` ON `order_promotion_applications` (`PromotionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_orders_ContractId` ON `orders` (`ContractId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_orders_CreatedBySalesUserId` ON `orders` (`CreatedBySalesUserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_orders_InvoiceCode` ON `orders` (`InvoiceCode`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_orders_ScheduledDate_Status` ON `orders` (`ScheduledDate`, `Status`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_orders_UserId` ON `orders` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_organization_legal_documents_MediaFileId` ON `organization_legal_documents` (`MediaFileId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_organization_legal_documents_OrganizationId` ON `organization_legal_documents` (`OrganizationId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_organizations_IsActive_Name` ON `organizations` (`IsActive`, `Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_organizations_Name` ON `organizations` (`Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_partner_documents_Code` ON `partner_documents` (`Code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_partner_documents_MediaFileId` ON `partner_documents` (`MediaFileId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_partner_documents_PartnerId_CreatedAt` ON `partner_documents` (`PartnerId`, `CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_partner_payments_ContractId` ON `partner_payments` (`ContractId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_partner_payments_PartnerId` ON `partner_payments` (`PartnerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_partner_payments_PaymentDate` ON `partner_payments` (`PaymentDate`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_partners_IsActive_LegalName` ON `partners` (`IsActive`, `LegalName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_partners_TaxId` ON `partners` (`TaxId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_payments_OrderId` ON `payments` (`OrderId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_payments_PayerId` ON `payments` (`PayerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_payments_PaymentDate` ON `payments` (`PaymentDate`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_permissions_Action` ON `permissions` (`Action`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_permissions_IsActive` ON `permissions` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_permissions_Name` ON `permissions` (`Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_permissions_Resource` ON `permissions` (`Resource`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_promotion_targets_PromotionId` ON `promotion_targets` (`PromotionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_promotions_Code` ON `promotions` (`Code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_recruitment_CreatedBy` ON `recruitment` (`CreatedBy`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_recruitment_IsActive` ON `recruitment` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_reviews_CreatedAt` ON `reviews` (`CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_reviews_DishId` ON `reviews` (`DishId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_reviews_OrderId` ON `reviews` (`OrderId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_reviews_RepliedByUserId` ON `reviews` (`RepliedByUserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_reviews_UserId` ON `reviews` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_role_permissions_IsActive` ON `role_permissions` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_role_permissions_PermissionId` ON `role_permissions` (`PermissionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_role_permissions_RoleId` ON `role_permissions` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_role_permissions_RoleId_PermissionId` ON `role_permissions` (`RoleId`, `PermissionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_roles_IsActive` ON `roles` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_roles_Name` ON `roles` (`Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_sentiments_ReviewId` ON `sentiments` (`ReviewId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_system_backups_CreatedAtUtc` ON `system_backups` (`CreatedAtUtc`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_system_backups_IsDeleted` ON `system_backups` (`IsDeleted`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_transactions_Category` ON `transactions` (`Category`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_transactions_Date` ON `transactions` (`Date`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_user_organizations_OrganizationId` ON `user_organizations` (`OrganizationId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_user_organizations_UserId_OrganizationId` ON `user_organizations` (`UserId`, `OrganizationId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_user_permissions_IsActive` ON `user_permissions` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_user_permissions_PermissionId` ON `user_permissions` (`PermissionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_user_permissions_UserId` ON `user_permissions` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_user_permissions_UserId_PermissionId` ON `user_permissions` (`UserId`, `PermissionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_user_roles_IsActive` ON `user_roles` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_user_roles_RoleId` ON `user_roles` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_user_roles_UserId` ON `user_roles` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_user_roles_UserId_RoleId` ON `user_roles` (`UserId`, `RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_user_tokens_AccessToken` ON `user_tokens` (`AccessToken`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_user_tokens_ExpiresAt` ON `user_tokens` (`ExpiresAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_user_tokens_IsActive` ON `user_tokens` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_user_tokens_RefreshToken` ON `user_tokens` (`RefreshToken`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_user_tokens_UserId` ON `user_tokens` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_users_Email` ON `users` (`Email`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_users_IsActive` ON `users` (`IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_users_Username` ON `users` (`Username`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_weekly_menu_images_MediaFileId` ON `weekly_menu_images` (`MediaFileId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_weekly_menu_images_WeeklyMenuId_SortOrder` ON `weekly_menu_images` (`WeeklyMenuId`, `SortOrder`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_weekly_menus_CreatedBy` ON `weekly_menus` (`CreatedBy`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE INDEX `IX_weekly_menus_CustomerTypeId` ON `weekly_menus` (`CustomerTypeId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    CREATE UNIQUE INDEX `IX_weekly_menus_StartDate_EndDate` ON `weekly_menus` (`StartDate`, `EndDate`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260614160925_AddDishIdToMenuSuggestionPlanItem') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260614160925_AddDishIdToMenuSuggestionPlanItem', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

