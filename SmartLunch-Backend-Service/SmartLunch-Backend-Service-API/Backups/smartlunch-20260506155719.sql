-- MySQL dump 10.13  Distrib 8.0.44, for Linux (x86_64)
--
-- Host: localhost    Database: SmartLunch
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `SmartLunch`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `SmartLunch` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `SmartLunch`;

--
-- Table structure for table `banners`
--

DROP TABLE IF EXISTS `banners`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `banners` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `PageName` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Trang hiển thị (Home, Factory, etc.)',
  `Title` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Tiêu đề banner',
  `Subtitle` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Phụ đề banner',
  `ImageUrl` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Đường dẫn ảnh banner',
  `CtaLink` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Đường dẫn khi click',
  `DisplayOrder` int NOT NULL DEFAULT '0',
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_banners_page_active` (`PageName`,`IsActive`,`DisplayOrder`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Banner quảng cáo';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `banners`
--

LOCK TABLES `banners` WRITE;
/*!40000 ALTER TABLE `banners` DISABLE KEYS */;
/*!40000 ALTER TABLE `banners` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_banners_code` BEFORE INSERT ON `banners` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('banners'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `chatbot_logs`
--

DROP TABLE IF EXISTS `chatbot_logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chatbot_logs` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'M? t? sinh (trigger)',
  `UserId` int DEFAULT NULL,
  `Message` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `Response` text COLLATE utf8mb4_unicode_ci,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_chatbot_logs_code` (`Code`),
  KEY `IX_chatbot_logs_user` (`UserId`),
  KEY `IX_chatbot_logs_created` (`CreatedAt` DESC),
  CONSTRAINT `FK_chatbot_logs_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Lịch sử chatbot';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chatbot_logs`
--

LOCK TABLES `chatbot_logs` WRITE;
/*!40000 ALTER TABLE `chatbot_logs` DISABLE KEYS */;
INSERT INTO `chatbot_logs` VALUES (1,'CBL00000001',10,'Hôm nay có món gì ăn trưa?','Hôm nay thực đơn trưa gồm: Cơm sườn nướng, Rau muống xào tỏi, Canh chua tôm.','2026-05-06 15:53:21'),(2,'CBL00000002',11,'Tôi muốn xem đánh giá món cơm gà','Cơm gà chiên mắm có đánh giá trung bình 4/5 sao.','2026-05-06 15:53:21'),(3,'CBL00000003',NULL,'Giờ đặt cơm trưa là mấy giờ?','Bạn có thể đặt cơm trưa trước 9:00 sáng hàng ngày.','2026-05-06 15:53:21');
/*!40000 ALTER TABLE `chatbot_logs` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_chatbot_logs_code` BEFORE INSERT ON `chatbot_logs` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('chatbot_logs'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `code_prefixes`
--

DROP TABLE IF EXISTS `code_prefixes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `code_prefixes` (
  `TableName` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Prefix` varchar(5) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LastSequence` int NOT NULL DEFAULT '0' COMMENT 'Bộ đếm sequence hiện tại',
  PRIMARY KEY (`TableName`),
  UNIQUE KEY `UK_code_prefixes_prefix` (`Prefix`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Cấu hình prefix + sequence cho cột Code';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `code_prefixes`
--

LOCK TABLES `code_prefixes` WRITE;
/*!40000 ALTER TABLE `code_prefixes` DISABLE KEYS */;
INSERT INTO `code_prefixes` VALUES ('banners','BNR','Banner',0),('chatbot_logs','CBL','Chatbot',3),('complaints','CPL','Khiếu nại',2),('contracts','CTR','Hợp đồng',5),('cooking_methods','CKM','Phương pháp chế biến',7),('deliveries','DLV','Giao hàng',3),('dish_categories','DCT','Danh mục slot món',6),('dish_dish_categories','DDC','Món — slot',56),('dish_ingredients','DIG','Nguyên liệu món',112),('dishes','DSH','Món ăn',50),('ingredient_actual_intake_lines','IAL','Chi tiết nhập',2),('ingredient_actual_intakes','IAI','Phiếu nhập kho',1),('ingredient_categories','ICAT','Danh mục nguyên liệu',7),('ingredient_intake_proposal_lines','IPL','Chi tiết đề xuất',6),('ingredient_intake_proposals','IIP','Đề xuất nhập',3),('ingredient_sources','IGS','Nguồn gốc lô hàng',7),('ingredients','IGR','Nguyên liệu',27),('internal_stock_issue_lines','ISL','Chi tiết xuất',5),('internal_stock_issues','ISI','Phiếu xuất kho',2),('media_files','MDF','Tệp media',2),('menu_schedule','MSC','Lịch thực đơn',16),('menu_suggestions','MSG','Gợi ý AI',2),('news','NWS','Tin tức',0),('notifications','NTC','Thông báo',0),('order_items','OIT','Chi tiết đơn',10),('orders','ORD','Đơn hàng',4),('organizations','ORG','Đơn vị',4),('partner_payments','PPM','Thanh toán NCC',4),('partners','PTN','Nhà cung cấp',3),('payments','PMT','Thanh toán',2),('permissions','PER','Quyền hạn',190),('recruitment','RCM','Tuyển dụng',0),('reviews','RVW','Đánh giá',3),('role_permissions','RPE','Gán quyền vai trò',579),('roles','ROL','Vai trò',8),('sentiments','STM','Cảm xúc',3),('transactions','TXN','Thu chi',5),('user_organizations','UOR','Thành viên đơn vị',0),('user_permissions','UPE','Gán quyền trực tiếp',361),('user_roles','URO','Gán vai trò',12),('user_tokens','UTK','Token xác thực',1),('users','USR','Người dùng',12),('weekly_menus','WMN','Thực đơn tuần',2);
/*!40000 ALTER TABLE `code_prefixes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `complaints`
--

DROP TABLE IF EXISTS `complaints`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `complaints` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `UserId` int NOT NULL,
  `OrderId` int DEFAULT NULL,
  `Title` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `Status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'new',
  `AssignedTo` int DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ResolvedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_complaints_code` (`Code`),
  KEY `IX_complaints_user` (`UserId`),
  KEY `IX_complaints_status` (`Status`),
  KEY `FK_complaints_order` (`OrderId`),
  KEY `FK_complaints_assignee` (`AssignedTo`),
  CONSTRAINT `FK_complaints_assignee` FOREIGN KEY (`AssignedTo`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_complaints_order` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_complaints_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Khiếu nại khách hàng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `complaints`
--

LOCK TABLES `complaints` WRITE;
/*!40000 ALTER TABLE `complaints` DISABLE KEYS */;
INSERT INTO `complaints` VALUES (1,'CPL00000001',7,2,'Giao hàng trễ','Đơn hàng cho trường XYZ bị giao trễ 30 phút.','in_progress',1,'2026-05-06 15:53:21',NULL),(2,'CPL00000002',10,3,'Thiếu món','Đặt 2 món nhưng chỉ nhận 1. Thiếu canh chua tôm.','new',NULL,'2026-05-06 15:53:21',NULL);
/*!40000 ALTER TABLE `complaints` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_complaints_code` BEFORE INSERT ON `complaints` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('complaints'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `contracts`
--

DROP TABLE IF EXISTS `contracts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `contracts` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `PartnerId` int DEFAULT NULL COMMENT 'Nhà cung cấp (nếu là hợp đồng mua)',
  `OrganizationId` int DEFAULT NULL COMMENT 'Đơn vị khách hàng (nếu là hợp đồng bán)',
  `ContractNumber` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ContractType` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Framework' COMMENT 'Framework | Order-Based',
  `Description` varchar(1000) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mô tả hợp đồng',
  `SupplySchedule` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Lịch giao hàng',
  `StartDate` date NOT NULL,
  `EndDate` date DEFAULT NULL,
  `TotalValue` decimal(12,2) DEFAULT NULL,
  `DepositAmount` decimal(12,2) DEFAULT NULL,
  `ContractFileUrl` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Đường dẫn file hợp đồng',
  `IsDigitallySigned` tinyint(1) NOT NULL DEFAULT '0',
  `DigitalSignature` longtext COLLATE utf8mb4_unicode_ci COMMENT 'Dữ liệu chữ ký số',
  `DigitallySignedAt` datetime DEFAULT NULL,
  `SignatureImage` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Ảnh chữ ký (nếu có)',
  `Status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'active' COMMENT 'active|expired|cancelled|pending_signature',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_contracts_code` (`Code`),
  KEY `IX_contracts_partner` (`PartnerId`),
  KEY `IX_contracts_org` (`OrganizationId`),
  KEY `IX_contracts_status` (`Status`),
  CONSTRAINT `FK_contracts_org` FOREIGN KEY (`OrganizationId`) REFERENCES `organizations` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_contracts_partner` FOREIGN KEY (`PartnerId`) REFERENCES `partners` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Hợp đồng và Chữ ký số';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contracts`
--

LOCK TABLES `contracts` WRITE;
/*!40000 ALTER TABLE `contracts` DISABLE KEYS */;
INSERT INTO `contracts` VALUES (1,'CTR00000001',1,NULL,'HD-2025-001','Framework','Cung cấp thịt heo, gà và hải sản tươi sống','T2, T4, T6 lúc 5h sáng','2025-01-01','2025-12-31',500000000.00,50000000.00,NULL,0,NULL,NULL,NULL,'active','2026-05-06 15:53:20',NULL),(2,'CTR00000002',2,NULL,'HD-2025-002','Framework','Cung cấp rau củ quả organic hàng ngày','Mỗi ngày lúc 4h30 sáng','2025-01-01','2025-06-30',200000000.00,20000000.00,NULL,0,NULL,NULL,NULL,'active','2026-05-06 15:53:20',NULL),(3,'CTR00000003',3,NULL,'HD-2025-003','Framework','Cung cấp gia vị và nước chấm hàng tháng','Ngày 1 và 15 hàng tháng','2025-03-01','2026-02-28',80000000.00,8000000.00,NULL,0,NULL,NULL,NULL,'active','2026-05-06 15:53:20',NULL),(4,'CTR00000004',1,1,'HD-B2B-ABC-001','Framework','Hợp đồng cung cấp suất ăn văn phòng cho ABC Tech','Mỗi ngày lúc 11h sáng','2025-01-01','2026-12-31',1000000000.00,100000000.00,NULL,0,NULL,NULL,NULL,'active','2026-05-06 15:53:20',NULL),(5,'CTR00000005',1,2,'HD-B2B-XYZ-002','Framework','Hợp đồng cung cấp suất ăn trường học cho XYZ','Mỗi ngày lúc 10h sáng','2025-01-01','2026-12-31',800000000.00,80000000.00,NULL,0,NULL,NULL,NULL,'active','2026-05-06 15:53:20',NULL);
/*!40000 ALTER TABLE `contracts` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_contracts_code` BEFORE INSERT ON `contracts` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('contracts'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `cooking_methods`
--

DROP TABLE IF EXISTS `cooking_methods`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cooking_methods` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `MethodKey` varchar(32) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'fried|stewed|boiled|stir_fried|grilled|steamed|raw',
  `Name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Tên hiển thị (VI)',
  `SortOrder` int NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_cooking_methods_method_key` (`MethodKey`),
  UNIQUE KEY `UK_cooking_methods_code` (`Code`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Phương pháp chế biến (AI)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cooking_methods`
--

LOCK TABLES `cooking_methods` WRITE;
/*!40000 ALTER TABLE `cooking_methods` DISABLE KEYS */;
INSERT INTO `cooking_methods` VALUES (1,'CKM00000001','fried','Chiên / rán',10,'2026-05-06 15:53:20'),(2,'CKM00000002','stewed','Kho / hầm',20,'2026-05-06 15:53:20'),(3,'CKM00000003','boiled','Luộc / nấu',30,'2026-05-06 15:53:20'),(4,'CKM00000004','stir_fried','Xào',40,'2026-05-06 15:53:20'),(5,'CKM00000005','grilled','Nướng',50,'2026-05-06 15:53:20'),(6,'CKM00000006','steamed','Hấp',60,'2026-05-06 15:53:20'),(7,'CKM00000007','raw','Sống / trộng',70,'2026-05-06 15:53:20');
/*!40000 ALTER TABLE `cooking_methods` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_cooking_methods_code` BEFORE INSERT ON `cooking_methods` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('cooking_methods'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `deliveries`
--

DROP TABLE IF EXISTS `deliveries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `deliveries` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `OrderId` int NOT NULL,
  `AssignedStaffId` int DEFAULT NULL,
  `DeliveryAddress` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `DeliveryStatus` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `DeliveredAt` datetime DEFAULT NULL,
  `Notes` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_deliveries_code` (`Code`),
  KEY `IX_deliveries_order` (`OrderId`),
  KEY `IX_deliveries_staff` (`AssignedStaffId`),
  CONSTRAINT `FK_deliveries_order` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_deliveries_staff` FOREIGN KEY (`AssignedStaffId`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Giao hàng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `deliveries`
--

LOCK TABLES `deliveries` WRITE;
/*!40000 ALTER TABLE `deliveries` DISABLE KEYS */;
INSERT INTO `deliveries` VALUES (1,'DLV00000001',1,8,'123 Nguyễn Huệ, Q.1, TP.HCM','completed','2026-05-04 15:53:21','Giao thành công','2026-05-06 15:53:21'),(2,'DLV00000002',2,8,'456 Lê Lợi, Q.3, TP.HCM','pending',NULL,'Dự kiến giao 11:00','2026-05-06 15:53:21'),(3,'DLV00000003',4,8,'456 Lê Lợi, Q.3, TP.HCM','completed','2026-05-05 15:53:21','Đã giao','2026-05-06 15:53:21');
/*!40000 ALTER TABLE `deliveries` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_deliveries_code` BEFORE INSERT ON `deliveries` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('deliveries'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `dish_categories`
--

DROP TABLE IF EXISTS `dish_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dish_categories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `SlotKey` varchar(32) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'main|side|soup|vegetable|noodle_soup|dessert — khớp AI',
  `Name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Tên hiển thị (VI)',
  `SortOrder` int NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_dish_categories_slot_key` (`SlotKey`),
  UNIQUE KEY `UK_dish_categories_code` (`Code`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Danh mục slot món (AI meal slot)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dish_categories`
--

LOCK TABLES `dish_categories` WRITE;
/*!40000 ALTER TABLE `dish_categories` DISABLE KEYS */;
INSERT INTO `dish_categories` VALUES (1,'DCT00000001','main','Món chính',10,'2026-05-06 15:53:21'),(2,'DCT00000002','side','Món phụ',20,'2026-05-06 15:53:21'),(3,'DCT00000003','soup','Canh / súp',30,'2026-05-06 15:53:21'),(4,'DCT00000004','vegetable','Rau / món xanh',40,'2026-05-06 15:53:21'),(5,'DCT00000005','noodle_soup','Món nước / phở / bún',50,'2026-05-06 15:53:21'),(6,'DCT00000006','dessert','Tráng miệng',60,'2026-05-06 15:53:21');
/*!40000 ALTER TABLE `dish_categories` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_dish_categories_code` BEFORE INSERT ON `dish_categories` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('dish_categories'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `dish_dish_categories`
--

DROP TABLE IF EXISTS `dish_dish_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dish_dish_categories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `DishId` int NOT NULL,
  `DishCategoryId` int NOT NULL COMMENT 'FK dish_categories.Id',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_dish_dish_categories_dish_cat` (`DishId`,`DishCategoryId`),
  UNIQUE KEY `UK_dish_dish_categories_code` (`Code`),
  KEY `IX_dish_dish_categories_category` (`DishCategoryId`),
  CONSTRAINT `FK_ddc_category` FOREIGN KEY (`DishCategoryId`) REFERENCES `dish_categories` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_ddc_dish` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=88 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Món — nhiều slot (AI covers)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dish_dish_categories`
--

LOCK TABLES `dish_dish_categories` WRITE;
/*!40000 ALTER TABLE `dish_dish_categories` DISABLE KEYS */;
INSERT INTO `dish_dish_categories` VALUES (1,'DDC00000001',13,1),(2,'DDC00000002',12,1),(3,'DDC00000003',6,1),(4,'DDC00000004',8,1),(5,'DDC00000005',3,1),(6,'DDC00000006',2,1),(7,'DDC00000007',14,1),(8,'DDC00000008',1,1),(9,'DDC00000009',16,1),(10,'DDC00000010',5,1),(11,'DDC00000011',10,1),(12,'DDC00000012',7,1),(13,'DDC00000013',4,1),(14,'DDC00000014',9,1),(15,'DDC00000015',15,1),(16,'DDC00000016',11,1),(32,'DDC00000017',18,3),(33,'DDC00000018',23,3),(34,'DDC00000019',26,3),(35,'DDC00000020',19,3),(36,'DDC00000021',17,3),(37,'DDC00000022',24,3),(38,'DDC00000023',27,3),(39,'DDC00000024',20,3),(40,'DDC00000025',25,3),(41,'DDC00000026',21,3),(42,'DDC00000027',22,3),(47,'DDC00000028',35,4),(48,'DDC00000029',32,4),(49,'DDC00000030',37,4),(50,'DDC00000031',33,4),(51,'DDC00000032',36,4),(52,'DDC00000033',30,4),(53,'DDC00000034',31,4),(54,'DDC00000035',29,4),(55,'DDC00000036',34,4),(56,'DDC00000037',28,4),(62,'DDC00000038',46,2),(63,'DDC00000039',42,2),(64,'DDC00000040',40,2),(65,'DDC00000041',39,2),(66,'DDC00000042',45,2),(67,'DDC00000043',43,2),(68,'DDC00000044',44,2),(69,'DDC00000045',38,2),(70,'DDC00000046',41,2),(77,'DDC00000047',47,6),(78,'DDC00000048',48,6),(80,'DDC00000049',49,1),(81,'DDC00000050',49,3),(82,'DDC00000051',49,4),(83,'DDC00000052',49,5),(84,'DDC00000053',50,1),(85,'DDC00000054',50,3),(86,'DDC00000055',50,4),(87,'DDC00000056',50,5);
/*!40000 ALTER TABLE `dish_dish_categories` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_dish_dish_categories_code` BEFORE INSERT ON `dish_dish_categories` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('dish_dish_categories'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `dish_images`
--

DROP TABLE IF EXISTS `dish_images`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dish_images` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `DishId` int NOT NULL,
  `MediaFileId` int NOT NULL,
  `Role` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'gallery' COMMENT 'cover | gallery',
  `SortOrder` int NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_dish_images_code` (`Code`),
  KEY `IX_dish_images_dish` (`DishId`,`SortOrder`),
  KEY `IX_dish_images_media` (`MediaFileId`),
  CONSTRAINT `FK_dish_images_dish` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_dish_images_media` FOREIGN KEY (`MediaFileId`) REFERENCES `media_files` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Ảnh món ăn';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dish_images`
--

LOCK TABLES `dish_images` WRITE;
/*!40000 ALTER TABLE `dish_images` DISABLE KEYS */;
/*!40000 ALTER TABLE `dish_images` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dish_ingredients`
--

DROP TABLE IF EXISTS `dish_ingredients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dish_ingredients` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `DishId` int NOT NULL,
  `IngredientId` int NOT NULL,
  `Quantity` decimal(10,2) NOT NULL,
  `Unit` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_dish_ingredients_dish_ing` (`DishId`,`IngredientId`),
  UNIQUE KEY `UK_dish_ingredients_code` (`Code`),
  KEY `IX_dish_ingredients_ingredient` (`IngredientId`),
  CONSTRAINT `FK_dish_ingredients_dish` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_dish_ingredients_ingredient` FOREIGN KEY (`IngredientId`) REFERENCES `ingredients` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=113 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Nguyên liệu của món ăn';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dish_ingredients`
--

LOCK TABLES `dish_ingredients` WRITE;
/*!40000 ALTER TABLE `dish_ingredients` DISABLE KEYS */;
INSERT INTO `dish_ingredients` VALUES (1,'DIG00000001',1,1,0.20,'kg'),(2,'DIG00000002',1,23,0.15,'kg'),(3,'DIG00000003',2,2,0.25,'kg'),(4,'DIG00000004',2,23,0.15,'kg'),(5,'DIG00000005',2,24,0.03,'lít'),(6,'DIG00000006',3,23,0.20,'kg'),(7,'DIG00000007',3,3,0.05,'kg'),(8,'DIG00000008',3,7,1.00,'quả'),(9,'DIG00000009',3,13,0.05,'kg'),(10,'DIG00000010',4,1,0.18,'kg'),(11,'DIG00000011',4,7,1.00,'quả'),(12,'DIG00000012',4,27,0.05,'lít'),(13,'DIG00000013',5,2,0.22,'kg'),(14,'DIG00000014',5,22,0.02,'kg'),(15,'DIG00000015',6,4,0.20,'kg'),(16,'DIG00000016',7,6,0.14,'kg'),(17,'DIG00000017',7,21,0.08,'kg'),(18,'DIG00000018',8,5,0.18,'kg'),(19,'DIG00000019',8,23,0.15,'kg'),(20,'DIG00000020',8,24,0.02,'lít'),(21,'DIG00000021',9,1,0.16,'kg'),(22,'DIG00000022',9,10,0.12,'kg'),(23,'DIG00000023',10,2,0.22,'kg'),(24,'DIG00000024',10,12,0.02,'kg'),(25,'DIG00000025',10,22,0.01,'kg'),(26,'DIG00000026',11,3,0.12,'kg'),(27,'DIG00000027',11,25,0.03,'kg'),(28,'DIG00000028',12,5,0.20,'kg'),(29,'DIG00000029',12,26,0.02,'lít'),(30,'DIG00000030',13,6,0.13,'kg'),(31,'DIG00000031',13,10,0.10,'kg'),(32,'DIG00000032',14,23,0.18,'kg'),(33,'DIG00000033',14,2,0.12,'kg'),(34,'DIG00000034',14,7,1.00,'quả'),(35,'DIG00000035',15,1,0.17,'kg'),(36,'DIG00000036',15,12,0.02,'kg'),(37,'DIG00000037',16,2,0.22,'kg'),(38,'DIG00000038',16,22,0.02,'kg'),(39,'DIG00000039',17,3,0.15,'kg'),(40,'DIG00000040',17,10,0.10,'kg'),(41,'DIG00000041',18,15,0.10,'kg'),(42,'DIG00000042',18,1,0.05,'kg'),(43,'DIG00000043',19,17,0.15,'kg'),(44,'DIG00000044',19,1,0.05,'kg'),(45,'DIG00000045',20,16,0.15,'kg'),(46,'DIG00000046',20,1,0.08,'kg'),(47,'DIG00000047',21,19,0.15,'kg'),(48,'DIG00000048',21,3,0.06,'kg'),(49,'DIG00000049',22,13,0.08,'kg'),(50,'DIG00000050',22,14,0.10,'kg'),(51,'DIG00000051',22,1,0.05,'kg'),(52,'DIG00000052',23,10,0.12,'kg'),(53,'DIG00000053',23,7,1.00,'quả'),(54,'DIG00000054',24,8,2.00,'miếng'),(55,'DIG00000055',24,10,0.10,'kg'),(56,'DIG00000056',25,14,0.18,'kg'),(57,'DIG00000057',25,1,0.05,'kg'),(58,'DIG00000058',26,17,0.18,'kg'),(59,'DIG00000059',26,10,0.08,'kg'),(60,'DIG00000060',27,16,0.12,'kg'),(61,'DIG00000061',27,10,0.08,'kg'),(62,'DIG00000062',28,9,0.30,'kg'),(63,'DIG00000063',28,12,0.02,'kg'),(64,'DIG00000064',29,8,2.00,'miếng'),(65,'DIG00000065',29,10,0.08,'kg'),(66,'DIG00000066',30,17,0.25,'kg'),(67,'DIG00000067',30,12,0.02,'kg'),(68,'DIG00000068',31,18,0.20,'kg'),(69,'DIG00000069',31,1,0.05,'kg'),(70,'DIG00000070',32,15,0.15,'kg'),(71,'DIG00000071',32,3,0.05,'kg'),(72,'DIG00000072',33,13,0.22,'kg'),(73,'DIG00000073',33,7,2.00,'quả'),(74,'DIG00000074',34,14,0.20,'kg'),(75,'DIG00000075',34,1,0.06,'kg'),(76,'DIG00000076',35,15,0.18,'kg'),(77,'DIG00000077',35,12,0.02,'kg'),(78,'DIG00000078',36,17,0.28,'kg'),(79,'DIG00000079',37,10,0.15,'kg'),(80,'DIG00000080',37,7,2.00,'quả'),(81,'DIG00000081',38,7,2.00,'quả'),(82,'DIG00000082',39,14,0.20,'kg'),(83,'DIG00000083',40,8,2.00,'miếng'),(84,'DIG00000084',41,7,1.00,'quả'),(85,'DIG00000085',42,20,0.15,'kg'),(86,'DIG00000086',42,12,0.01,'kg'),(87,'DIG00000087',43,20,0.12,'kg'),(88,'DIG00000088',43,13,0.10,'kg'),(89,'DIG00000089',44,10,0.15,'kg'),(90,'DIG00000090',44,12,0.02,'kg'),(91,'DIG00000091',45,14,0.22,'kg'),(92,'DIG00000092',46,13,0.18,'kg'),(93,'DIG00000093',46,25,0.02,'kg'),(94,'DIG00000094',47,15,0.20,'kg'),(95,'DIG00000095',47,27,0.04,'lít'),(96,'DIG00000096',47,25,0.03,'kg'),(97,'DIG00000097',48,20,0.12,'kg'),(98,'DIG00000098',48,13,0.10,'kg'),(99,'DIG00000099',48,25,0.03,'kg'),(100,'DIG00000100',49,6,0.14,'kg'),(101,'DIG00000101',49,23,0.10,'kg'),(102,'DIG00000102',49,24,0.02,'lít'),(103,'DIG00000103',49,21,0.04,'kg'),(104,'DIG00000104',49,11,0.02,'kg'),(105,'DIG00000105',49,22,0.01,'kg'),(106,'DIG00000106',49,9,0.06,'kg'),(107,'DIG00000107',50,3,0.12,'kg'),(108,'DIG00000108',50,23,0.10,'kg'),(109,'DIG00000109',50,24,0.02,'lít'),(110,'DIG00000110',50,10,0.08,'kg'),(111,'DIG00000111',50,11,0.02,'kg'),(112,'DIG00000112',50,19,0.05,'kg');
/*!40000 ALTER TABLE `dish_ingredients` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_dish_ingredients_code` BEFORE INSERT ON `dish_ingredients` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('dish_ingredients'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `dishes`
--

DROP TABLE IF EXISTS `dishes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dishes` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `Name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Tên món ăn (tiếng Việt)',
  `NameEnglish` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Tên món ăn tiếng Anh (dùng cho AI grouping)',
  `Description` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mô tả',
  `CookingMethodId` int DEFAULT NULL COMMENT 'FK cooking_methods.Id (MethodKey = enum AI)',
  `Price` decimal(10,2) NOT NULL COMMENT 'Giá mỗi phần',
  `ImageUrl` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'ảnh món ăn',
  `DietaryLabel` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Nhãn dinh dưỡng',
  `Calories` decimal(10,2) DEFAULT NULL COMMENT 'Năng lượng (kcal)',
  `Protein` decimal(10,2) DEFAULT NULL COMMENT 'Đạm (g)',
  `Fat` decimal(10,2) DEFAULT NULL COMMENT 'Béo (g)',
  `Carbs` decimal(10,2) DEFAULT NULL COMMENT 'Bột đường (g)',
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_dishes_name` (`Name`),
  UNIQUE KEY `UK_dishes_code` (`Code`),
  KEY `IX_dishes_active` (`IsActive`),
  KEY `IX_dishes_cooking_method` (`CookingMethodId`),
  CONSTRAINT `FK_dishes_cooking_method` FOREIGN KEY (`CookingMethodId`) REFERENCES `cooking_methods` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Món ăn';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dishes`
--

LOCK TABLES `dishes` WRITE;
/*!40000 ALTER TABLE `dishes` DISABLE KEYS */;
INSERT INTO `dishes` VALUES (1,'DSH00000001','Cơm sườn nướng','Grilled Pork Rib Rice','Cơm trắng kèm sườn heo nướng sả ớt',5,25000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(2,'DSH00000002','Cơm gà chiên mắm','Fish Sauce Fried Chicken Rice','Cơm trắng kèm đùi gà chiên nước mắm',1,24000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(3,'DSH00000003','Cơm chiên dương châu','Yangzhou Fried Rice','Cơm chiên với tôm, trứng, cà rốt',4,22000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(4,'DSH00000004','Thịt heo kho trứng','Braised Pork with Egg','Thịt heo ba chỉ kho nước dừa với trứng gà',2,22000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(5,'DSH00000005','Gà kho gừng','Ginger Braised Chicken','Đùi gà ta kho gừng sả đậm đà',2,20000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(6,'DSH00000006','Cá lóc kho tộ','Claypot Braised Snakehead Fish','Cá lóc đồng kho tiêu đặc trưng Nam Bộ',2,24000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(7,'DSH00000007','Thịt bò xào hành tây','Beef Stir-Fried with Onion','Thịt bò nạc xào hành tây và ớt chuông',4,28000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(8,'DSH00000008','Cơm cá basa kho tộ','Braised Basa Claypot Rice','Cơm trắng kèm cá basa kho tiêu',2,23000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(9,'DSH00000009','Thịt heo xào cà chua','Pork Stir-Fry with Tomato','Thịt heo thái mỏng xào cà chua chua ngọt',4,21000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(10,'DSH00000010','Gà nướng sả','Lemongrass Grilled Chicken','Đùi gà nướng sả ớt thơm',5,26000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(11,'DSH00000011','Tôm rang me','Tamarind Glazed Shrimp','Tôm sú rang sốt me chua ngọt',4,32000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(12,'DSH00000012','Cá basa chiên giòn','Crispy Fried Basa','Cá basa phi lê chiên giòn',1,22000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(13,'DSH00000013','Bò xào cà chua','Beef Tomato Stir-Fry','Thịt bò xào cà chua tươi',4,27000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(14,'DSH00000014','Cơm rang thịt gà','Chicken Fried Rice','Cơm rang gạo ST25 với thịt gà xé',4,21000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(15,'DSH00000015','Thịt heo xào sả ớt','Lemongrass Chili Pork Stir-Fry','Thịt heo xào sả ớt đậm đà',4,23000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(16,'DSH00000016','Gà hấp gừng','Ginger Steamed Chicken','Đùi gà hấp gừng mềm ngọt',6,20000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(17,'DSH00000017','Canh chua tôm','Sour Shrimp Soup','Canh chua nấu tôm sú với cà chua dứa',3,12000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(18,'DSH00000018','Canh bí đỏ thịt bằm','Pumpkin Soup with Minced Pork','Canh bí đỏ nấu với thịt heo xay thơm ngọt',3,8000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(19,'DSH00000019','Canh cải ngọt thịt bằm','Bok Choy Soup with Minced Pork','Canh cải ngọt nấu thịt heo xay',3,7000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(20,'DSH00000020','Canh khổ qua nhồi thịt','Stuffed Bitter Melon Soup','Khổ qua xanh nhồi thịt heo xay nấu nước trong',3,10000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(21,'DSH00000021','Canh mồng tơi tôm','Malabar Spinach Shrimp Soup','Canh mồng tơi nấu tôm sú tươi',3,9000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(22,'DSH00000022','Canh rau củ thịt bằm','Mixed Vegetable Minced Pork Soup','Canh cà rốt khoai tây nấu thịt heo xay',3,7500.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(23,'DSH00000023','Canh cà chua trứng','Tomato Egg Drop Soup','Canh cà chua chua ngọt kèm trứng',3,6500.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(24,'DSH00000024','Canh đậu hũ cà chua','Tofu Tomato Soup','Đậu hũ non nấu cà chua thanh mát',3,7000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(25,'DSH00000025','Canh khoai tây thịt bằm','Potato Minced Pork Soup','Khoai tây nấu thịt heo xay bùi ngọt',3,8000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(26,'DSH00000026','Canh cải ngọt cà chua','Bok Choy Tomato Soup','Cải ngọt nấu cà chua tươi',3,7000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(27,'DSH00000027','Canh khổ qua cà chua','Bitter Melon Tomato Soup','Khổ qua nấu cà chua giảm đắng',3,9000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(28,'DSH00000028','Rau muống xào tỏi','Stir-Fried Water Spinach with Garlic','Rau muống xào tỏi phi thơm',4,6000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(29,'DSH00000029','Đậu hũ sốt cà chua','Tofu in Tomato Sauce','Đậu hũ non sốt cà chua tươi',2,8000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(30,'DSH00000030','Cải ngọt xào tỏi','Stir-Fried Bok Choy with Garlic','Cải ngọt tươi xào tỏi phi thơm',4,5000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(31,'DSH00000031','Đậu cove xào thịt','Green Bean Stir-Fried with Pork','Đậu cove xào thịt heo thái lát',4,8000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(32,'DSH00000032','Bí đỏ xào tôm','Pumpkin Stir-Fried with Shrimp','Bí đỏ non xào tôm sú tươi',4,7000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(33,'DSH00000033','Cà rốt xào trứng','Carrot Egg Stir-Fry','Cà rốt xào trứng mềm ngọt',4,5500.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(34,'DSH00000034','Khoai tây xào thịt','Potato Pork Stir-Fry','Khoai tây xào thịt heo thái lát',4,7500.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(35,'DSH00000035','Bí đỏ xào tỏi','Garlic Stir-Fried Pumpkin','Bí đỏ non xào tỏi phi',4,6000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(36,'DSH00000036','Cải ngọt luộc','Blanched Bok Choy','Cải ngọt luộc vừa chín giòn',3,4500.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(37,'DSH00000037','Cà chua xào trứng','Tomato Egg Stir-Fry','Cà chua xào trứng đậm đà',4,6500.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(38,'DSH00000038','Trứng chiên','Fried Egg','Trứng gà ta chiên vàng',1,6000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(39,'DSH00000039','Khoai tây chiên','Fried Potato','Khoai tây Đà Lạt chiên giòn',1,8000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(40,'DSH00000040','Đậu hũ chiên giòn','Crispy Fried Tofu','Đậu hũ non chiên vàng giòn',1,5000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(41,'DSH00000041','Trứng luộc','Boiled Egg','Trứng gà ta luộc chín vừa',3,4000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(42,'DSH00000042','Dưa leo trộn','Cucumber Salad','Dưa leo Đà Lạt trộn tỏi ớt chua ngọt',7,3000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(43,'DSH00000043','Salad dưa leo cà rốt','Cucumber Carrot Salad','Dưa leo cà rốt trộn chua ngọt',7,5000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(44,'DSH00000044','Sốt cà chua chấm','Tomato Dipping Sauce','Sốt cà chua nấu nhừ chấm kèm cơm',2,4000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(45,'DSH00000045','Khoai tây luộc','Boiled Potato','Khoai tây luộc chín mềm',3,5500.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(46,'DSH00000046','Cà rốt muối chua','Pickled Carrot','Cà rốt ngâm chua ngọt giòn',7,3500.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(47,'DSH00000047','Chè bí đỏ','Sweet Pumpkin Dessert','Bí đỏ hầm nước dừa đường thanh mát',2,8000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(48,'DSH00000048','Trái cây dầm','Fruit in Syrup','Dưa leo cà rốt dầm đường lạnh',7,7000.00,NULL,'vegan',NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(49,'DSH00000049','Phở bò','Beef Pho','Bánh phở, nước dùng hầm xương, thịt bò tái/chín, hành gừng; kèm rau thơm',3,45000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL),(50,'DSH00000050','Bánh canh cua','Crab Banh Canh','Sợi bánh canh gạo, nước dùng hải sản, tôm/chả cua kiểu Nam Bộ, rau thơm',3,42000.00,NULL,NULL,NULL,NULL,NULL,NULL,1,'2026-05-06 15:53:20',NULL);
/*!40000 ALTER TABLE `dishes` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_dishes_code` BEFORE INSERT ON `dishes` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('dishes'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `ingredient_actual_intake_lines`
--

DROP TABLE IF EXISTS `ingredient_actual_intake_lines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredient_actual_intake_lines` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `IntakeId` int NOT NULL,
  `IngredientId` int NOT NULL,
  `Quantity` decimal(12,2) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_actual_intake_lines_code` (`Code`),
  KEY `IX_actual_intake_lines_intake` (`IntakeId`),
  KEY `IX_actual_intake_lines_ingredient` (`IngredientId`),
  CONSTRAINT `FK_actual_intake_lines_ingredient` FOREIGN KEY (`IngredientId`) REFERENCES `ingredients` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_actual_intake_lines_intake` FOREIGN KEY (`IntakeId`) REFERENCES `ingredient_actual_intakes` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Chi tiết phiếu nhập kho';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient_actual_intake_lines`
--

LOCK TABLES `ingredient_actual_intake_lines` WRITE;
/*!40000 ALTER TABLE `ingredient_actual_intake_lines` DISABLE KEYS */;
INSERT INTO `ingredient_actual_intake_lines` VALUES (1,'IAL00000001',1,1,30.00),(2,'IAL00000002',1,2,20.00);
/*!40000 ALTER TABLE `ingredient_actual_intake_lines` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_ingredient_actual_intake_lines_code` BEFORE INSERT ON `ingredient_actual_intake_lines` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredient_actual_intake_lines'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `ingredient_actual_intakes`
--

DROP TABLE IF EXISTS `ingredient_actual_intakes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredient_actual_intakes` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `ReceiptCode` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ProposalId` int NOT NULL,
  `CreatedByUserId` int NOT NULL,
  `ReceivedAt` datetime NOT NULL,
  `Note` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_actual_intakes_receipt` (`ReceiptCode`),
  UNIQUE KEY `UK_actual_intakes_proposal` (`ProposalId`),
  UNIQUE KEY `UK_actual_intakes_code` (`Code`),
  KEY `FK_actual_intakes_user` (`CreatedByUserId`),
  CONSTRAINT `FK_actual_intakes_proposal` FOREIGN KEY (`ProposalId`) REFERENCES `ingredient_intake_proposals` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_actual_intakes_user` FOREIGN KEY (`CreatedByUserId`) REFERENCES `users` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Phiếu nhập kho thực tế';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient_actual_intakes`
--

LOCK TABLES `ingredient_actual_intakes` WRITE;
/*!40000 ALTER TABLE `ingredient_actual_intakes` DISABLE KEYS */;
INSERT INTO `ingredient_actual_intakes` VALUES (1,'IAI00000001','PNK-20250324-X1Y2',1,3,'2026-05-03 15:53:20','Nhận đủ hàng','2026-05-06 15:53:20');
/*!40000 ALTER TABLE `ingredient_actual_intakes` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_ingredient_actual_intakes_code` BEFORE INSERT ON `ingredient_actual_intakes` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredient_actual_intakes'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `ingredient_categories`
--

DROP TABLE IF EXISTS `ingredient_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredient_categories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Mã tự sinh',
  `Name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NameEnglish` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Identifier used in AI rules, e.g. meat, seafood',
  `Description` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQ_Cat_Code` (`Code`),
  UNIQUE KEY `UQ_Cat_NameEn` (`NameEnglish`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Danh mục nguyên liệu';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient_categories`
--

LOCK TABLES `ingredient_categories` WRITE;
/*!40000 ALTER TABLE `ingredient_categories` DISABLE KEYS */;
INSERT INTO `ingredient_categories` VALUES (1,'ICAT00000001','Thịt (Động vật)','meat','Thịt (Động vật)','2026-05-06 15:53:20'),(2,'ICAT00000002','Cá & Hải sản','seafood','Cá & Hải sản','2026-05-06 15:53:20'),(3,'ICAT00000003','Trứng','egg','Trứng','2026-05-06 15:53:20'),(4,'ICAT00000004','Đậu & Đạm thực vật','plant_protein','Đậu & Đạm thực vật','2026-05-06 15:53:20'),(5,'ICAT00000005','Rau củ quả','vegetable','Rau củ quả','2026-05-06 15:53:20'),(6,'ICAT00000006','Gia vị','spice','Gia vị','2026-05-06 15:53:20'),(7,'ICAT00000007','Tinh bột','carb','Tinh bột','2026-05-06 15:53:20');
/*!40000 ALTER TABLE `ingredient_categories` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_ingredient_categories_code` BEFORE INSERT ON `ingredient_categories` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredient_categories'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `ingredient_intake_proposal_lines`
--

DROP TABLE IF EXISTS `ingredient_intake_proposal_lines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredient_intake_proposal_lines` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `ProposalId` int NOT NULL,
  `IngredientId` int NOT NULL,
  `Quantity` decimal(12,2) NOT NULL,
  `LineNote` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_intake_proposal_lines_code` (`Code`),
  KEY `IX_intake_proposal_lines_proposal` (`ProposalId`),
  KEY `IX_intake_proposal_lines_ingredient` (`IngredientId`),
  CONSTRAINT `FK_intake_proposal_lines_ingredient` FOREIGN KEY (`IngredientId`) REFERENCES `ingredients` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_intake_proposal_lines_proposal` FOREIGN KEY (`ProposalId`) REFERENCES `ingredient_intake_proposals` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Chi tiết phiếu đề xuất nhập';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient_intake_proposal_lines`
--

LOCK TABLES `ingredient_intake_proposal_lines` WRITE;
/*!40000 ALTER TABLE `ingredient_intake_proposal_lines` DISABLE KEYS */;
INSERT INTO `ingredient_intake_proposal_lines` VALUES (1,'IPL00000001',1,1,30.00,'Tươi trong ngày'),(2,'IPL00000002',1,2,20.00,NULL),(3,'IPL00000003',2,9,20.00,NULL),(4,'IPL00000004',2,10,15.00,NULL),(5,'IPL00000005',3,24,10.00,NULL),(6,'IPL00000006',3,26,12.00,NULL);
/*!40000 ALTER TABLE `ingredient_intake_proposal_lines` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_ingredient_intake_proposal_lines_code` BEFORE INSERT ON `ingredient_intake_proposal_lines` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredient_intake_proposal_lines'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `ingredient_intake_proposals`
--

DROP TABLE IF EXISTS `ingredient_intake_proposals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredient_intake_proposals` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `ProposalCode` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `HeaderNote` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedByUserId` int NOT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ReviewedByUserId` int DEFAULT NULL,
  `ReviewedAt` datetime DEFAULT NULL,
  `ReviewNote` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_intake_proposals_proposal_code` (`ProposalCode`),
  UNIQUE KEY `UK_intake_proposals_code` (`Code`),
  KEY `IX_intake_proposals_status` (`Status`),
  KEY `IX_intake_proposals_creator` (`CreatedByUserId`),
  KEY `FK_intake_proposals_reviewer` (`ReviewedByUserId`),
  CONSTRAINT `FK_intake_proposals_creator` FOREIGN KEY (`CreatedByUserId`) REFERENCES `users` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_intake_proposals_reviewer` FOREIGN KEY (`ReviewedByUserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Phiếu đề xuất nhập nguyên liệu';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient_intake_proposals`
--

LOCK TABLES `ingredient_intake_proposals` WRITE;
/*!40000 ALTER TABLE `ingredient_intake_proposals` DISABLE KEYS */;
INSERT INTO `ingredient_intake_proposals` VALUES (1,'IIP00000001','DXN-20250324-A1B2','fulfilled','Đề xuất nhập thịt tuần 4/3',3,'2026-05-01 15:53:20',2,'2026-05-02 15:53:20','Duyệt'),(2,'IIP00000002','DXN-20250325-E5F6','approved','Đề xuất nhập rau củ tuần 4/3',3,'2026-05-01 15:53:20',2,'2026-05-04 15:53:20','Duyệt'),(3,'IIP00000003','DXN-20250326-I9J0','submitted','Đề xuất nhập gia vị tháng 4',3,'2026-05-01 15:53:20',NULL,NULL,NULL);
/*!40000 ALTER TABLE `ingredient_intake_proposals` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_ingredient_intake_proposals_code` BEFORE INSERT ON `ingredient_intake_proposals` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredient_intake_proposals'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `ingredient_sources`
--

DROP TABLE IF EXISTS `ingredient_sources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredient_sources` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `IngredientId` int NOT NULL,
  `PartnerId` int DEFAULT NULL,
  `BatchNumber` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `OriginDetails` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ProductionDate` date DEFAULT NULL,
  `ExpirationDate` date DEFAULT NULL,
  `Certification` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_ingredient_sources_code` (`Code`),
  KEY `IX_ingredient_sources_ingredient` (`IngredientId`),
  KEY `IX_ingredient_sources_partner` (`PartnerId`),
  CONSTRAINT `FK_ingredient_sources_ingredient` FOREIGN KEY (`IngredientId`) REFERENCES `ingredients` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_ingredient_sources_partner` FOREIGN KEY (`PartnerId`) REFERENCES `partners` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Nguồn gốc lô nguyên liệu';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient_sources`
--

LOCK TABLES `ingredient_sources` WRITE;
/*!40000 ALTER TABLE `ingredient_sources` DISABLE KEYS */;
INSERT INTO `ingredient_sources` VALUES (1,'IGS00000001',1,1,'LOT-TH-20250320','Trang trại Bình Dương','2025-03-20','2025-03-23','VietGAHP','2026-05-06 15:53:20'),(2,'IGS00000002',2,1,'LOT-GA-20250320','Trang trại Long An','2025-03-20','2025-03-22','VietGAHP','2026-05-06 15:53:20'),(3,'IGS00000003',3,1,'LOT-TOM-20250321','Vùng nuôi Cà Mau','2025-03-21','2025-03-25','ASC','2026-05-06 15:53:20'),(4,'IGS00000004',4,1,'LOT-CA-20250322','Ao nuôi Đồng Tháp','2025-03-22','2025-03-24','VietGAHP','2026-05-06 15:53:20'),(5,'IGS00000005',9,2,'LOT-RM-20250321','Vùng trồng rau Long An','2025-03-21','2025-03-24','VietGAP','2026-05-06 15:53:20'),(6,'IGS00000006',23,2,'LOT-G-20250301','Vùng lúa Sóc Trăng','2025-03-01','2025-09-01','GlobalGAP','2026-05-06 15:53:20'),(7,'IGS00000007',24,3,'LOT-NM-20250201','Nhà thùng Phú Quốc','2025-02-01','2027-02-01','HACCP, ISO 22000','2026-05-06 15:53:20');
/*!40000 ALTER TABLE `ingredient_sources` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_ingredient_sources_code` BEFORE INSERT ON `ingredient_sources` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredient_sources'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `ingredients`
--

DROP TABLE IF EXISTS `ingredients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredients` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `Name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Tên nguyên liệu (tiếng Việt)',
  `NameEnglish` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Tên nguyên liệu tiếng Anh (dùng cho AI grouping: pork|chicken|shrimp|...)',
  `Unit` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Đơn vị (kg, lít, ...)',
  `Description` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mô tả',
  `DefaultSupplierId` int DEFAULT NULL,
  `CostPerUnit` decimal(10,2) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `CategoryId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_ingredients_name` (`Name`),
  UNIQUE KEY `UK_ingredients_code` (`Code`),
  KEY `IX_ingredients_category` (`CategoryId`),
  KEY `IX_ingredients_supplier` (`DefaultSupplierId`),
  CONSTRAINT `FK_ingredients_category` FOREIGN KEY (`CategoryId`) REFERENCES `ingredient_categories` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_ingredients_supplier` FOREIGN KEY (`DefaultSupplierId`) REFERENCES `partners` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Nguyên liệu';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredients`
--

LOCK TABLES `ingredients` WRITE;
/*!40000 ALTER TABLE `ingredients` DISABLE KEYS */;
INSERT INTO `ingredients` VALUES (1,'IGR00000001','Thịt heo nạc vai','pork','kg','Thịt heo nạc vai tươi',1,120000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',1),(2,'IGR00000002','Thịt gà ta','chicken','kg','Gà ta nuôi thả vườn',1,95000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',1),(3,'IGR00000003','Tôm sú','shrimp','kg','Tôm sú size 30-35 con/kg',1,280000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',2),(4,'IGR00000004','Cá lóc','snakehead_fish','kg','Cá lóc đồng tươi',1,85000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',2),(5,'IGR00000005','Cá basa','basa_fish','kg','Cá basa phi lê đông lạnh',1,60000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',2),(6,'IGR00000006','Thịt bò','beef','kg','Thịt bò nạc vai',1,200000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',1),(7,'IGR00000007','Trứng gà','egg','quả','Trứng gà ta tươi',1,4000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',3),(8,'IGR00000008','Đậu hũ','tofu','miếng','Đậu hũ non tươi',1,5000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',4),(9,'IGR00000009','Rau muống','morning_glory','kg','Rau muống nước sạch',2,15000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(10,'IGR00000010','Cà chua','tomato','kg','Cà chua Đà Lạt organic',2,25000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(11,'IGR00000011','Hành tím','shallot','kg','Hành tím Sóc Trăng',2,35000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(12,'IGR00000012','Tỏi','garlic','kg','Tỏi Lý Sơn',2,80000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(13,'IGR00000013','Cà rốt','carrot','kg','Cà rốt Đà Lạt',2,20000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(14,'IGR00000014','Khoai tây','potato','kg','Khoai tây Đà Lạt',2,25000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(15,'IGR00000015','Bí đỏ','pumpkin','kg','Bí đỏ Đà Lạt',2,18000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(16,'IGR00000016','Khổ qua','bitter_melon','kg','Khổ qua xanh',2,25000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(17,'IGR00000017','Cải ngọt','bok_choy','kg','Cải ngọt tươi',2,20000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(18,'IGR00000018','Đậu cove','green_bean','kg','Đậu cove tươi',2,30000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(19,'IGR00000019','Mồng tơi','malabar_spinach','kg','Rau mồng tơi tươi',2,15000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(20,'IGR00000020','Dưa leo','cucumber','kg','Dưa leo Đà Lạt',2,15000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(21,'IGR00000021','Hành tây','onion','kg','Hành tây trắng',2,22000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(22,'IGR00000022','Gừng','ginger','kg','Gừng tươi Hưng Yên',2,40000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',5),(23,'IGR00000023','Gạo ST25','rice','kg','Gạo ST25 Sóc Trăng',2,22000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',7),(24,'IGR00000024','Nước mắm','fish_sauce','lít','Nước mắm Phú Quốc 40 độ đạm',3,65000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',6),(25,'IGR00000025','Đường cát trắng','sugar','kg','Đường tinh luyện',3,18000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',6),(26,'IGR00000026','Dầu thực vật','cooking_oil','lít','Dầu Neptune Gold',3,42000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',6),(27,'IGR00000027','Nước dừa tươi','coconut_water','lít','Nước dừa tươi Bến Tre',3,20000.00,1,'2026-05-06 15:53:20','2026-05-06 15:53:20',6);
/*!40000 ALTER TABLE `ingredients` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_ingredients_code` BEFORE INSERT ON `ingredients` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredients'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `internal_stock_issue_lines`
--

DROP TABLE IF EXISTS `internal_stock_issue_lines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `internal_stock_issue_lines` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `IssueId` int NOT NULL,
  `IngredientId` int NOT NULL,
  `Quantity` decimal(12,2) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_stock_issue_lines_code` (`Code`),
  KEY `IX_stock_issue_lines_issue` (`IssueId`),
  KEY `IX_stock_issue_lines_ingredient` (`IngredientId`),
  CONSTRAINT `FK_stock_issue_lines_ingredient` FOREIGN KEY (`IngredientId`) REFERENCES `ingredients` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_stock_issue_lines_issue` FOREIGN KEY (`IssueId`) REFERENCES `internal_stock_issues` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Chi tiết phiếu xuất kho';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `internal_stock_issue_lines`
--

LOCK TABLES `internal_stock_issue_lines` WRITE;
/*!40000 ALTER TABLE `internal_stock_issue_lines` DISABLE KEYS */;
INSERT INTO `internal_stock_issue_lines` VALUES (1,'ISL00000001',1,1,10.00),(2,'ISL00000002',1,9,5.00),(3,'ISL00000003',1,23,30.00),(4,'ISL00000004',2,2,8.00),(5,'ISL00000005',2,10,5.00);
/*!40000 ALTER TABLE `internal_stock_issue_lines` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_internal_stock_issue_lines_code` BEFORE INSERT ON `internal_stock_issue_lines` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('internal_stock_issue_lines'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `internal_stock_issues`
--

DROP TABLE IF EXISTS `internal_stock_issues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `internal_stock_issues` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `IssueCode` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IssuedAt` datetime NOT NULL,
  `Reason` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedByUserId` int DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_stock_issues_issue_code` (`IssueCode`),
  UNIQUE KEY `UK_stock_issues_code` (`Code`),
  KEY `IX_stock_issues_user` (`CreatedByUserId`),
  CONSTRAINT `FK_stock_issues_user` FOREIGN KEY (`CreatedByUserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Phiếu xuất kho nội bộ';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `internal_stock_issues`
--

LOCK TABLES `internal_stock_issues` WRITE;
/*!40000 ALTER TABLE `internal_stock_issues` DISABLE KEYS */;
INSERT INTO `internal_stock_issues` VALUES (1,'ISI00000001','PXK-20250321-A1B2','2025-03-21 06:00:00','Xuất kho nguyên liệu bếp trưa 21/03',NULL,'2026-05-06 15:53:20'),(2,'ISI00000002','PXK-20250322-E5F6','2025-03-22 06:00:00','Xuất kho nguyên liệu bếp trưa 22/03',NULL,'2026-05-06 15:53:20');
/*!40000 ALTER TABLE `internal_stock_issues` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_internal_stock_issues_code` BEFORE INSERT ON `internal_stock_issues` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('internal_stock_issues'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `inventory`
--

DROP TABLE IF EXISTS `inventory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inventory` (
  `IngredientId` int NOT NULL,
  `QuantityAvailable` decimal(12,2) NOT NULL DEFAULT '0.00',
  `ReorderLevel` decimal(12,2) DEFAULT NULL,
  `LastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IngredientId`),
  CONSTRAINT `FK_inventory_ingredient` FOREIGN KEY (`IngredientId`) REFERENCES `ingredients` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Tồn kho nguyên liệu';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inventory`
--

LOCK TABLES `inventory` WRITE;
/*!40000 ALTER TABLE `inventory` DISABLE KEYS */;
INSERT INTO `inventory` VALUES (1,50.00,20.00,'2026-05-06 15:53:20'),(2,35.00,15.00,'2026-05-06 15:53:20'),(3,20.00,10.00,'2026-05-06 15:53:20'),(4,30.00,10.00,'2026-05-06 15:53:20'),(5,40.00,15.00,'2026-05-06 15:53:20'),(6,20.00,8.00,'2026-05-06 15:53:20'),(7,500.00,100.00,'2026-05-06 15:53:20'),(8,100.00,30.00,'2026-05-06 15:53:20'),(9,30.00,10.00,'2026-05-06 15:53:20'),(10,25.00,10.00,'2026-05-06 15:53:20'),(11,15.00,5.00,'2026-05-06 15:53:20'),(12,8.00,3.00,'2026-05-06 15:53:20'),(13,40.00,10.00,'2026-05-06 15:53:20'),(14,35.00,10.00,'2026-05-06 15:53:20'),(15,35.00,10.00,'2026-05-06 15:53:20'),(16,25.00,8.00,'2026-05-06 15:53:20'),(17,40.00,12.00,'2026-05-06 15:53:20'),(18,30.00,10.00,'2026-05-06 15:53:20'),(19,25.00,8.00,'2026-05-06 15:53:20'),(20,30.00,10.00,'2026-05-06 15:53:20'),(21,20.00,5.00,'2026-05-06 15:53:20'),(22,10.00,3.00,'2026-05-06 15:53:20'),(23,200.00,50.00,'2026-05-06 15:53:20'),(24,20.00,5.00,'2026-05-06 15:53:20'),(25,30.00,10.00,'2026-05-06 15:53:20'),(26,25.00,8.00,'2026-05-06 15:53:20'),(27,15.00,5.00,'2026-05-06 15:53:20');
/*!40000 ALTER TABLE `inventory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `media_files`
--

DROP TABLE IF EXISTS `media_files`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `media_files` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'M? t? sinh (trigger)',
  `OwnerUserId` int NOT NULL,
  `Bucket` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ObjectName` varchar(1024) COLLATE utf8mb4_unicode_ci NOT NULL,
  `OriginalFileName` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ContentType` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SizeBytes` bigint NOT NULL,
  `Md5HashBase64` varchar(128) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MediaType` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'unknown',
  `IsPublic` tinyint(1) NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_media_files_bucket_object` (`Bucket`,`ObjectName`(191)),
  UNIQUE KEY `UK_media_files_code` (`Code`),
  KEY `IX_media_files_owner` (`OwnerUserId`),
  CONSTRAINT `FK_media_files_owner` FOREIGN KEY (`OwnerUserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Tệp media';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `media_files`
--

LOCK TABLES `media_files` WRITE;
/*!40000 ALTER TABLE `media_files` DISABLE KEYS */;
INSERT INTO `media_files` VALUES (1,'MDF00000001',1,'smartlunch-storage','avatars/admin_avatar.jpg','admin_avatar.jpg','image/jpeg',52480,NULL,'image',1,'2026-05-06 15:53:20',NULL),(2,'MDF00000002',1,'smartlunch-storage','menu/banner_weekly.jpg','banner_weekly.jpg','image/jpeg',204800,NULL,'image',1,'2026-05-06 15:53:20',NULL);
/*!40000 ALTER TABLE `media_files` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_media_files_code` BEFORE INSERT ON `media_files` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('media_files'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `menu_schedule`
--

DROP TABLE IF EXISTS `menu_schedule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu_schedule` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `MenuId` int NOT NULL,
  `Date` date NOT NULL,
  `MealSlot` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'lunch',
  `DishId` int NOT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_menu_schedule_combo` (`MenuId`,`Date`,`MealSlot`,`DishId`),
  UNIQUE KEY `UK_menu_schedule_code` (`Code`),
  KEY `IX_menu_schedule_dish` (`DishId`),
  KEY `IX_menu_schedule_date` (`Date`),
  CONSTRAINT `FK_menu_schedule_dish` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_menu_schedule_menu` FOREIGN KEY (`MenuId`) REFERENCES `weekly_menus` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Lịch thực đơn theo ngày';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu_schedule`
--

LOCK TABLES `menu_schedule` WRITE;
/*!40000 ALTER TABLE `menu_schedule` DISABLE KEYS */;
INSERT INTO `menu_schedule` VALUES (1,'MSC00000001',1,'2025-03-24','lunch',1,'2026-05-06 15:53:21'),(2,'MSC00000002',1,'2025-03-24','lunch',28,'2026-05-06 15:53:21'),(3,'MSC00000003',1,'2025-03-24','lunch',17,'2026-05-06 15:53:21'),(4,'MSC00000004',1,'2025-03-25','lunch',2,'2026-05-06 15:53:21'),(5,'MSC00000005',1,'2025-03-25','lunch',29,'2026-05-06 15:53:21'),(6,'MSC00000006',1,'2025-03-25','lunch',38,'2026-05-06 15:53:21'),(7,'MSC00000007',1,'2025-03-26','lunch',3,'2026-05-06 15:53:21'),(8,'MSC00000008',1,'2025-03-26','lunch',28,'2026-05-06 15:53:21'),(9,'MSC00000009',1,'2025-03-27','lunch',1,'2026-05-06 15:53:21'),(10,'MSC00000010',1,'2025-03-27','lunch',39,'2026-05-06 15:53:21'),(11,'MSC00000011',1,'2025-03-28','lunch',2,'2026-05-06 15:53:21'),(12,'MSC00000012',1,'2025-03-28','lunch',28,'2026-05-06 15:53:21'),(13,'MSC00000013',2,'2025-03-31','lunch',3,'2026-05-06 15:53:21'),(14,'MSC00000014',2,'2025-03-31','lunch',17,'2026-05-06 15:53:21'),(15,'MSC00000015',2,'2025-04-01','lunch',1,'2026-05-06 15:53:21'),(16,'MSC00000016',2,'2025-04-01','lunch',29,'2026-05-06 15:53:21');
/*!40000 ALTER TABLE `menu_schedule` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_menu_schedule_code` BEFORE INSERT ON `menu_schedule` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('menu_schedule'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `menu_suggestion_plan_days`
--

DROP TABLE IF EXISTS `menu_suggestion_plan_days`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu_suggestion_plan_days` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MenuSuggestionPlanId` int NOT NULL,
  `DayIndex` tinyint NOT NULL,
  `DayName` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_menu_suggestion_plan_days_plan_dayindex` (`MenuSuggestionPlanId`,`DayIndex`),
  KEY `IX_menu_suggestion_plan_days_plan` (`MenuSuggestionPlanId`),
  CONSTRAINT `FK_menu_suggestion_plan_days_plan` FOREIGN KEY (`MenuSuggestionPlanId`) REFERENCES `menu_suggestion_plans` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Các ngày trong 1 phương án thực đơn';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu_suggestion_plan_days`
--

LOCK TABLES `menu_suggestion_plan_days` WRITE;
/*!40000 ALTER TABLE `menu_suggestion_plan_days` DISABLE KEYS */;
/*!40000 ALTER TABLE `menu_suggestion_plan_days` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `menu_suggestion_plan_items`
--

DROP TABLE IF EXISTS `menu_suggestion_plan_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu_suggestion_plan_items` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MenuSuggestionPlanDayId` int NOT NULL,
  `SlotCategory` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'main/side/soup/dessert',
  `DishName` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `DishSourceCategory` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'category gốc của dish (nếu có)',
  `Score` decimal(6,3) NOT NULL,
  `CostPerServing` decimal(10,2) NOT NULL,
  `ReasonsJson` json DEFAULT NULL COMMENT 'Danh sách lý do lựa chọn (JSON array of strings)',
  PRIMARY KEY (`Id`),
  KEY `IX_menu_suggestion_plan_items_day` (`MenuSuggestionPlanDayId`),
  KEY `IX_menu_suggestion_plan_items_slot` (`SlotCategory`),
  CONSTRAINT `FK_menu_suggestion_plan_items_day` FOREIGN KEY (`MenuSuggestionPlanDayId`) REFERENCES `menu_suggestion_plan_days` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Các món/slot được chọn trong 1 ngày của phương án';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu_suggestion_plan_items`
--

LOCK TABLES `menu_suggestion_plan_items` WRITE;
/*!40000 ALTER TABLE `menu_suggestion_plan_items` DISABLE KEYS */;
/*!40000 ALTER TABLE `menu_suggestion_plan_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `menu_suggestion_plans`
--

DROP TABLE IF EXISTS `menu_suggestion_plans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu_suggestion_plans` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MenuSuggestionId` int NOT NULL,
  `Rank` int NOT NULL,
  `PlanScore` decimal(6,3) NOT NULL,
  `ObjectiveValue` decimal(18,3) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_menu_suggestion_plans_suggestion_rank` (`MenuSuggestionId`,`Rank`),
  KEY `IX_menu_suggestion_plans_suggestion` (`MenuSuggestionId`),
  CONSTRAINT `FK_menu_suggestion_plans_suggestion` FOREIGN KEY (`MenuSuggestionId`) REFERENCES `menu_suggestions` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Các phương án (top-K) cho 1 lần gợi ý thực đơn';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu_suggestion_plans`
--

LOCK TABLES `menu_suggestion_plans` WRITE;
/*!40000 ALTER TABLE `menu_suggestion_plans` DISABLE KEYS */;
/*!40000 ALTER TABLE `menu_suggestion_plans` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `menu_suggestions`
--

DROP TABLE IF EXISTS `menu_suggestions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu_suggestions` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `WeekStart` date NOT NULL,
  `GeneratedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Version` int NOT NULL DEFAULT '1',
  `RulesKey` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BudgetPerServing` decimal(10,2) DEFAULT NULL,
  `TopK` int DEFAULT NULL,
  `TimeLimitSeconds` decimal(5,2) DEFAULT NULL,
  `PlanCount` int DEFAULT NULL,
  `SuggestionText` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `AlgorithmVersion` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_menu_suggestions_code` (`Code`),
  KEY `IX_menu_suggestions_week` (`WeekStart`),
  KEY `IX_menu_suggestions_week_createdby` (`WeekStart`,`CreatedBy`,`Version`),
  KEY `FK_menu_suggestions_user` (`CreatedBy`),
  CONSTRAINT `FK_menu_suggestions_user` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Gợi ý thực đơn AI';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu_suggestions`
--

LOCK TABLES `menu_suggestions` WRITE;
/*!40000 ALTER TABLE `menu_suggestions` DISABLE KEYS */;
INSERT INTO `menu_suggestions` VALUES (1,'MSG00000001','2025-03-31','2026-05-06 15:53:21',1,NULL,NULL,NULL,NULL,NULL,'Dựa trên đánh giá tuần trước:\n- T2: Cơm sườn nướng (5★), Đậu hũ sốt cà chua\n- T3: Cơm gà chiên mắm, Khoai tây chiên\n- T4: Cơm chiên dương châu (5★), Rau muống xào tỏi\nLưu ý: Giảm nước mắm trong gà chiên.','SmartLunch-AI-v1.2',1),(2,'MSG00000002','2025-04-07','2026-05-06 15:53:21',1,NULL,NULL,NULL,NULL,NULL,'Thực đơn tuần 2/4:\n- Tăng món chay (xu hướng +15%)\n- Bổ sung Bún bò Huế\n- Giữ Cơm sườn nướng (top 1)','SmartLunch-AI-v1.2',1);
/*!40000 ALTER TABLE `menu_suggestions` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_menu_suggestions_code` BEFORE INSERT ON `menu_suggestions` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('menu_suggestions'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `news`
--

DROP TABLE IF EXISTS `news`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `news` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `Title` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Tiêu đề bài viết',
  `Slug` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Đường dẫn thân thiện',
  `Summary` text COLLATE utf8mb4_unicode_ci COMMENT 'Tóm tắt bài viết',
  `Content` longtext COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Nội dung chi tiết',
  `ThumbnailUrl` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Ảnh đại diện bài viết',
  `Category` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Danh mục (Tin tức, Sự kiện, Khuyến mãi)',
  `AuthorId` int DEFAULT NULL COMMENT 'Người viết bài',
  `IsPublished` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'Trạng thái xuất bản',
  `PublishedAt` datetime DEFAULT NULL COMMENT 'Thời điểm xuất bản',
  `ViewCount` int NOT NULL DEFAULT '0' COMMENT 'Lượt xem',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1' COMMENT 'Trạng thái hoạt động',
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_news_slug` (`Slug`),
  UNIQUE KEY `UK_news_code` (`Code`),
  KEY `IX_news_published` (`IsPublished`,`PublishedAt` DESC),
  KEY `FK_news_author` (`AuthorId`),
  CONSTRAINT `FK_news_author` FOREIGN KEY (`AuthorId`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Bản tin hệ thống';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `news`
--

LOCK TABLES `news` WRITE;
/*!40000 ALTER TABLE `news` DISABLE KEYS */;
/*!40000 ALTER TABLE `news` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_news_code` BEFORE INSERT ON `news` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('news'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `notifications`
--

DROP TABLE IF EXISTS `notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notifications` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `UserId` int NOT NULL COMMENT 'Người nhận',
  `Title` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Message` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `Type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'info' COMMENT 'info | warning | success | error',
  `IsRead` tinyint(1) NOT NULL DEFAULT '0',
  `Link` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Đường dẫn khi click vào thông báo',
  `SendAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `IX_notifications_user_read` (`UserId`,`IsRead`,`SendAt` DESC),
  CONSTRAINT `FK_notifications_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Thông báo người dùng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notifications`
--

LOCK TABLES `notifications` WRITE;
/*!40000 ALTER TABLE `notifications` DISABLE KEYS */;
/*!40000 ALTER TABLE `notifications` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_notifications_code` BEFORE INSERT ON `notifications` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('notifications'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `order_items`
--

DROP TABLE IF EXISTS `order_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order_items` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `OrderId` int NOT NULL,
  `DishId` int NOT NULL,
  `Quantity` int NOT NULL DEFAULT '1',
  `UnitPrice` decimal(10,2) NOT NULL,
  `TotalPrice` decimal(12,2) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_order_items_code` (`Code`),
  KEY `IX_order_items_order` (`OrderId`),
  KEY `IX_order_items_dish` (`DishId`),
  CONSTRAINT `FK_order_items_dish` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_order_items_order` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Chi tiết đơn hàng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order_items`
--

LOCK TABLES `order_items` WRITE;
/*!40000 ALTER TABLE `order_items` DISABLE KEYS */;
INSERT INTO `order_items` VALUES (1,'OIT00000001',1,1,50,45000.00,2250000.00),(2,'OIT00000002',1,28,50,20000.00,1000000.00),(3,'OIT00000003',1,17,50,25000.00,1250000.00),(4,'OIT00000004',2,2,40,42000.00,1680000.00),(5,'OIT00000005',2,29,40,22000.00,880000.00),(6,'OIT00000006',2,28,40,20000.00,800000.00),(7,'OIT00000007',3,1,1,45000.00,45000.00),(8,'OIT00000008',3,17,1,35000.00,35000.00),(9,'OIT00000009',4,2,1,42000.00,42000.00),(10,'OIT00000010',4,28,1,20000.00,20000.00);
/*!40000 ALTER TABLE `order_items` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_order_items_code` BEFORE INSERT ON `order_items` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('order_items'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `UserId` int DEFAULT NULL,
  `ContractId` int DEFAULT NULL COMMENT 'Hợp đồng liên quan',
  `OrderDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ScheduledDate` date NOT NULL,
  `Status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `TotalAmount` decimal(12,2) NOT NULL,
  `PaymentStatus` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'unpaid',
  `InvoiceCode` varchar(40) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedBySalesUserId` int DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_orders_code` (`Code`),
  UNIQUE KEY `UQ_orders_invoice_code` (`InvoiceCode`),
  KEY `IX_orders_user` (`UserId`),
  KEY `IX_orders_contract` (`ContractId`),
  KEY `IX_orders_scheduled_status` (`ScheduledDate`,`Status`),
  KEY `FK_orders_sales` (`CreatedBySalesUserId`),
  CONSTRAINT `FK_orders_contract` FOREIGN KEY (`ContractId`) REFERENCES `contracts` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_orders_sales` FOREIGN KEY (`CreatedBySalesUserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_orders_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Đơn hàng suất ăn';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (1,'ORD00000001',NULL,4,'2026-05-06 15:53:21','2026-05-04','delivered',4500000.00,'paid','INV-20250321-001',NULL,'2026-05-03 15:53:21',NULL),(2,'ORD00000002',NULL,5,'2026-05-06 15:53:21','2026-05-06','confirmed',3360000.00,'unpaid','INV-20250323-002',NULL,'2026-05-05 15:53:21',NULL),(3,'ORD00000003',NULL,4,'2026-05-06 15:53:21','2026-05-06','pending',80000.00,'unpaid',NULL,NULL,'2026-05-06 15:53:21',NULL),(4,'ORD00000004',NULL,5,'2026-05-06 15:53:21','2026-05-05','delivered',62000.00,'paid',NULL,NULL,'2026-05-04 15:53:21',NULL);
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_orders_code` BEFORE INSERT ON `orders` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('orders'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `organizations`
--

DROP TABLE IF EXISTS `organizations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `organizations` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `Name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Tên đơn vị',
  `TaxCode` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã số thuế',
  `LegalRepresentative` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Người đại diện pháp luật',
  `Address` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Phone` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ContactPerson` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ContactEmail` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LogoUrl` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Logo đơn vị',
  `Website` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Office' COMMENT 'Office | Factory | School',
  `EducationLevel` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mẫu giáo | Tiểu học | THCS | THPT (nếu là School)',
  `IsSubscriptionActive` tinyint(1) NOT NULL DEFAULT '0',
  `DefaultDailyMeals` int NOT NULL DEFAULT '0',
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_organizations_name` (`Name`),
  UNIQUE KEY `UK_organizations_code` (`Code`),
  KEY `IX_organizations_active` (`IsActive`),
  KEY `FK_organizations_created_by` (`CreatedBy`),
  KEY `FK_organizations_updated_by` (`UpdatedBy`),
  CONSTRAINT `FK_organizations_created_by` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_organizations_updated_by` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Đơn vị đặt hàng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `organizations`
--

LOCK TABLES `organizations` WRITE;
/*!40000 ALTER TABLE `organizations` DISABLE KEYS */;
INSERT INTO `organizations` VALUES (1,'ORG00000001','Công ty TNHH ABC Tech',NULL,NULL,'123 Nguyễn Huệ, Q.1, TP.HCM','+84281234567','Đại Diện ABC','contact@abctech.vn',NULL,NULL,'Office',NULL,0,0,1,'2026-05-06 15:53:20',NULL,1,NULL),(2,'ORG00000002','Trường THPT XYZ',NULL,NULL,'456 Lê Lợi, Q.3, TP.HCM','+84289876543','Hương Lê','contact@xyzschool.edu.vn',NULL,NULL,'Office',NULL,0,0,1,'2026-05-06 15:53:20',NULL,1,NULL),(3,'ORG00000003','Nhà máy Sản Xuất DEF',NULL,NULL,'789 KCN Tân Bình, TP.HCM','+84287654321','Tùng Phạm','tung@def-factory.vn',NULL,NULL,'Office',NULL,0,0,1,'2026-05-06 15:53:20',NULL,1,NULL),(4,'ORG00000004','Văn phòng GHI Corp',NULL,NULL,'321 Pasteur, Q.1, TP.HCM','+84283216549','Lan Nguyễn','lan@ghi-corp.vn',NULL,NULL,'Office',NULL,0,0,1,'2026-05-06 15:53:20',NULL,1,NULL);
/*!40000 ALTER TABLE `organizations` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_organizations_code` BEFORE INSERT ON `organizations` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('organizations'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `partner_documents`
--

DROP TABLE IF EXISTS `partner_documents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `partner_documents` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `PartnerId` int NOT NULL,
  `MediaFileId` int NOT NULL,
  `DocumentType` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'other' COMMENT 'business_license | tax_certificate | other',
  `IsVerified` tinyint(1) NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_partner_documents_code` (`Code`),
  KEY `IX_partner_documents_partner` (`PartnerId`,`CreatedAt`),
  KEY `IX_partner_documents_media` (`MediaFileId`),
  CONSTRAINT `FK_partner_documents_media` FOREIGN KEY (`MediaFileId`) REFERENCES `media_files` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_partner_documents_partner` FOREIGN KEY (`PartnerId`) REFERENCES `partners` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Tài liệu doanh nghiệp';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `partner_documents`
--

LOCK TABLES `partner_documents` WRITE;
/*!40000 ALTER TABLE `partner_documents` DISABLE KEYS */;
/*!40000 ALTER TABLE `partner_documents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `partner_payments`
--

DROP TABLE IF EXISTS `partner_payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `partner_payments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'M? t? sinh (trigger)',
  `ContractId` int NOT NULL,
  `PartnerId` int NOT NULL,
  `PaymentDate` date NOT NULL,
  `Amount` decimal(12,2) NOT NULL,
  `Method` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'bank_transfer',
  `Status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_partner_payments_code` (`Code`),
  KEY `IX_partner_payments_contract` (`ContractId`),
  KEY `IX_partner_payments_partner` (`PartnerId`),
  CONSTRAINT `FK_partner_payments_contract` FOREIGN KEY (`ContractId`) REFERENCES `contracts` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_partner_payments_partner` FOREIGN KEY (`PartnerId`) REFERENCES `partners` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Thanh toán nhà cung cấp';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `partner_payments`
--

LOCK TABLES `partner_payments` WRITE;
/*!40000 ALTER TABLE `partner_payments` DISABLE KEYS */;
INSERT INTO `partner_payments` VALUES (1,'PPM00000001',1,1,'2025-02-01',42000000.00,'bank_transfer','completed','2026-05-06 15:53:20'),(2,'PPM00000002',1,1,'2025-03-01',45000000.00,'bank_transfer','completed','2026-05-06 15:53:20'),(3,'PPM00000003',2,2,'2025-02-15',35000000.00,'bank_transfer','completed','2026-05-06 15:53:20'),(4,'PPM00000004',3,3,'2025-03-01',6500000.00,'cash','completed','2026-05-06 15:53:20');
/*!40000 ALTER TABLE `partner_payments` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_partner_payments_code` BEFORE INSERT ON `partner_payments` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('partner_payments'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `partners`
--

DROP TABLE IF EXISTS `partners`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `partners` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `LegalName` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Tên pháp lý',
  `BusinessRegistrationNumber` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Số ĐKKD',
  `TaxId` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã số thuế',
  `LegalRepresentative` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Người đại diện pháp luật',
  `Address` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ContactPerson` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Phone` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Email` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LogoUrl` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Logo đối tác',
  `Website` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PerformanceRating` decimal(3,2) DEFAULT NULL,
  `ComplianceInfo` varchar(2000) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Thông tin tuân thủ',
  `FinancialTerms` varchar(1000) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Điều khoản thanh toán',
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_partners_code` (`Code`),
  UNIQUE KEY `UK_partners_tax_id` (`TaxId`),
  KEY `IX_partners_active` (`IsActive`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Nhà cung cấp';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `partners`
--

LOCK TABLES `partners` WRITE;
/*!40000 ALTER TABLE `partners` DISABLE KEYS */;
INSERT INTO `partners` VALUES (1,'PTN00000001','Công ty TNHH Thực Phẩm Sạch Việt','DKKD-2020-001234','0301234567','Nguyễn Văn An','100 Trường Chinh, Tân Phú, TP.HCM','Trần Thị Bình','+84901234567','contact@thucphamsachviet.vn',NULL,NULL,4.50,'VietGAP, ISO 22000:2018','Thanh toán 30 ngày',1,'2026-05-06 15:53:20',NULL),(2,'PTN00000002','HTX Rau Củ Đà Lạt','DKKD-2019-005678','5801234568','Lê Minh Cường','22 Phan Đình Phùng, Đà Lạt','Phạm Văn Đức','+84909876543','sales@raucudalat.vn',NULL,NULL,4.80,'Organic, GlobalGAP','COD',1,'2026-05-06 15:53:20',NULL),(3,'PTN00000003','Công ty CP Gia Vị Miền Nam','DKKD-2018-009012','0309012345','Hoàng Thị Em','55 Nguyễn Thị Minh Khai, Q.1','Vũ Quốc Fong','+84905551234','order@giavimiennam.vn',NULL,NULL,4.20,'HACCP','Thanh toán 15 ngày',1,'2026-05-06 15:53:20',NULL);
/*!40000 ALTER TABLE `partners` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_partners_code` BEFORE INSERT ON `partners` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('partners'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `payments`
--

DROP TABLE IF EXISTS `payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `OrderId` int NOT NULL,
  `PayerId` int DEFAULT NULL,
  `PaymentDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Amount` decimal(12,2) NOT NULL,
  `Method` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'cash',
  `Status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_payments_code` (`Code`),
  KEY `IX_payments_order` (`OrderId`),
  KEY `IX_payments_payer` (`PayerId`),
  CONSTRAINT `FK_payments_order` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_payments_payer` FOREIGN KEY (`PayerId`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Thanh toán đơn hàng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payments`
--

LOCK TABLES `payments` WRITE;
/*!40000 ALTER TABLE `payments` DISABLE KEYS */;
INSERT INTO `payments` VALUES (1,'PMT00000001',1,NULL,'2026-05-04 15:53:21',4500000.00,'bank_transfer','paid','2026-05-06 15:53:21'),(2,'PMT00000002',4,NULL,'2026-05-05 15:53:21',62000.00,'e_wallet','paid','2026-05-06 15:53:21');
/*!40000 ALTER TABLE `payments` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_payments_code` BEFORE INSERT ON `payments` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('payments'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `permissions`
--

DROP TABLE IF EXISTS `permissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permissions` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'M? t? sinh (trigger)',
  `Name` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Resource` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Action` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_permissions_name` (`Name`),
  UNIQUE KEY `UK_permissions_code` (`Code`),
  KEY `IX_permissions_resource_action` (`Resource`,`Action`,`IsActive`),
  KEY `FK_permissions_created_by` (`CreatedBy`),
  KEY `FK_permissions_updated_by` (`UpdatedBy`),
  CONSTRAINT `FK_permissions_created_by` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_permissions_updated_by` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=191 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Quyền hạn hệ thống';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permissions`
--

LOCK TABLES `permissions` WRITE;
/*!40000 ALTER TABLE `permissions` DISABLE KEYS */;
INSERT INTO `permissions` VALUES (1,'PER00000001','users.create','Tạo người dùng','users','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(2,'PER00000002','users.read','Xem thông tin người dùng','users','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(3,'PER00000003','users.update','Cập nhật người dùng','users','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(4,'PER00000004','users.delete','Xóa người dùng','users','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(5,'PER00000005','users.list','Danh sách người dùng','users','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(6,'PER00000006','roles.create','Tạo vai trò','roles','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(7,'PER00000007','roles.read','Xem vai trò','roles','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(8,'PER00000008','roles.update','Cập nhật vai trò','roles','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(9,'PER00000009','roles.delete','Xóa vai trò','roles','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(10,'PER00000010','roles.list','Danh sách vai trò','roles','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(11,'PER00000011','permissions.create','Tạo quyền','permissions','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(12,'PER00000012','permissions.read','Xem quyền','permissions','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(13,'PER00000013','permissions.update','Cập nhật quyền','permissions','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(14,'PER00000014','permissions.delete','Xóa quyền','permissions','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(15,'PER00000015','permissions.list','Danh sách quyền','permissions','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(16,'PER00000016','user_roles.create','Gán vai trò cho user','user_roles','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(17,'PER00000017','user_roles.read','Xem gán vai trò','user_roles','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(18,'PER00000018','user_roles.update','Cập nhật gán vai trò','user_roles','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(19,'PER00000019','user_roles.delete','Xóa gán vai trò','user_roles','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(20,'PER00000020','user_roles.list','Danh sách gán vai trò','user_roles','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(21,'PER00000021','user_permissions.create','Gán quyền trực tiếp','user_permissions','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(22,'PER00000022','user_permissions.read','Xem quyền trực tiếp','user_permissions','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(23,'PER00000023','user_permissions.update','Cập nhật quyền trực tiếp','user_permissions','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(24,'PER00000024','user_permissions.delete','Xóa quyền trực tiếp','user_permissions','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(25,'PER00000025','user_permissions.list','Danh sách quyền trực tiếp','user_permissions','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(26,'PER00000026','role_permissions.create','Gán quyền cho vai trò','role_permissions','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(27,'PER00000027','role_permissions.read','Xem quyền vai trò','role_permissions','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(28,'PER00000028','role_permissions.update','Cập nhật quyền vai trò','role_permissions','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(29,'PER00000029','role_permissions.delete','Xóa quyền vai trò','role_permissions','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(30,'PER00000030','role_permissions.list','Danh sách quyền vai trò','role_permissions','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(31,'PER00000031','user_tokens.create','Tạo token','user_tokens','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(32,'PER00000032','user_tokens.read','Xem token','user_tokens','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(33,'PER00000033','user_tokens.update','Cập nhật token','user_tokens','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(34,'PER00000034','user_tokens.delete','Xóa/thu hồi token','user_tokens','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(35,'PER00000035','user_tokens.list','Danh sách token','user_tokens','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(36,'PER00000036','media_files.create','Upload tệp','media_files','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(37,'PER00000037','media_files.read','Xem tệp','media_files','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(38,'PER00000038','media_files.update','Cập nhật tệp','media_files','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(39,'PER00000039','media_files.delete','Xóa tệp','media_files','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(40,'PER00000040','media_files.list','Danh sách tệp','media_files','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(41,'PER00000041','organizations.create','Tạo đơn vị','organizations','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(42,'PER00000042','organizations.read','Xem đơn vị','organizations','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(43,'PER00000043','organizations.update','Cập nhật đơn vị','organizations','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(44,'PER00000044','organizations.delete','Xóa đơn vị','organizations','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(45,'PER00000045','organizations.list','Danh sách đơn vị','organizations','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(46,'PER00000046','user_organizations.create','Gán thành viên đơn vị','user_organizations','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(47,'PER00000047','user_organizations.read','Xem thành viên đơn vị','user_organizations','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(48,'PER00000048','user_organizations.update','Cập nhật thành viên','user_organizations','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(49,'PER00000049','user_organizations.delete','Xóa thành viên đơn vị','user_organizations','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(50,'PER00000050','user_organizations.list','Danh sách thành viên','user_organizations','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(51,'PER00000051','partners.create','Tạo nhà cung cấp','partners','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(52,'PER00000052','partners.read','Xem nhà cung cấp','partners','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(53,'PER00000053','partners.update','Cập nhật nhà cung cấp','partners','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(54,'PER00000054','partners.delete','Xóa nhà cung cấp','partners','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(55,'PER00000055','partners.list','Danh sách nhà cung cấp','partners','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(56,'PER00000056','contracts.create','Tạo hợp đồng','contracts','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(57,'PER00000057','contracts.read','Xem hợp đồng','contracts','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(58,'PER00000058','contracts.update','Cập nhật hợp đồng','contracts','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(59,'PER00000059','contracts.delete','Xóa hợp đồng','contracts','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(60,'PER00000060','contracts.list','Danh sách hợp đồng','contracts','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(61,'PER00000061','partner_payments.create','Tạo thanh toán NCC','partner_payments','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(62,'PER00000062','partner_payments.read','Xem thanh toán NCC','partner_payments','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(63,'PER00000063','partner_payments.update','Cập nhật thanh toán NCC','partner_payments','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(64,'PER00000064','partner_payments.delete','Xóa thanh toán NCC','partner_payments','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(65,'PER00000065','partner_payments.list','Danh sách thanh toán NCC','partner_payments','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(66,'PER00000066','ingredients.create','Tạo nguyên liệu','ingredients','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(67,'PER00000067','ingredients.read','Xem nguyên liệu','ingredients','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(68,'PER00000068','ingredients.update','Cập nhật nguyên liệu','ingredients','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(69,'PER00000069','ingredients.delete','Xóa nguyên liệu','ingredients','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(70,'PER00000070','ingredients.list','Danh sách nguyên liệu','ingredients','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(71,'PER00000071','ingredient_sources.create','Tạo nguồn gốc lô hàng','ingredient_sources','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(72,'PER00000072','ingredient_sources.read','Xem nguồn gốc lô hàng','ingredient_sources','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(73,'PER00000073','ingredient_sources.update','Cập nhật nguồn gốc','ingredient_sources','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(74,'PER00000074','ingredient_sources.delete','Xóa nguồn gốc','ingredient_sources','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(75,'PER00000075','ingredient_sources.list','Danh sách nguồn gốc','ingredient_sources','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(76,'PER00000076','inventory.create','Tạo tồn kho','inventory','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(77,'PER00000077','inventory.read','Xem tồn kho','inventory','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(78,'PER00000078','inventory.update','Cập nhật tồn kho','inventory','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(79,'PER00000079','inventory.delete','Xóa tồn kho','inventory','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(80,'PER00000080','inventory.list','Danh sách tồn kho','inventory','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(81,'PER00000081','internal_stock_issues.create','Tạo phiếu xuất kho','internal_stock_issues','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(82,'PER00000082','internal_stock_issues.read','Xem phiếu xuất kho','internal_stock_issues','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(83,'PER00000083','internal_stock_issues.update','Cập nhật phiếu xuất','internal_stock_issues','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(84,'PER00000084','internal_stock_issues.delete','Xóa phiếu xuất kho','internal_stock_issues','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(85,'PER00000085','internal_stock_issues.list','Danh sách phiếu xuất','internal_stock_issues','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(86,'PER00000086','internal_stock_issue_lines.create','Tạo chi tiết phiếu xuất','internal_stock_issue_lines','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(87,'PER00000087','internal_stock_issue_lines.read','Xem chi tiết phiếu xuất','internal_stock_issue_lines','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(88,'PER00000088','internal_stock_issue_lines.update','Cập nhật chi tiết xuất','internal_stock_issue_lines','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(89,'PER00000089','internal_stock_issue_lines.delete','Xóa chi tiết phiếu xuất','internal_stock_issue_lines','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(90,'PER00000090','internal_stock_issue_lines.list','Danh sách chi tiết xuất','internal_stock_issue_lines','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(91,'PER00000091','ingredient_intake_proposals.create','Tạo phiếu đề xuất nhập','ingredient_intake_proposals','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(92,'PER00000092','ingredient_intake_proposals.read','Xem phiếu đề xuất','ingredient_intake_proposals','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(93,'PER00000093','ingredient_intake_proposals.update','Cập nhật đề xuất nhập','ingredient_intake_proposals','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(94,'PER00000094','ingredient_intake_proposals.delete','Xóa phiếu đề xuất','ingredient_intake_proposals','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(95,'PER00000095','ingredient_intake_proposals.list','Danh sách đề xuất nhập','ingredient_intake_proposals','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(96,'PER00000096','ingredient_intake_proposal_lines.create','Tạo chi tiết đề xuất','ingredient_intake_proposal_lines','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(97,'PER00000097','ingredient_intake_proposal_lines.read','Xem chi tiết đề xuất','ingredient_intake_proposal_lines','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(98,'PER00000098','ingredient_intake_proposal_lines.update','Cập nhật chi tiết đề xuất','ingredient_intake_proposal_lines','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(99,'PER00000099','ingredient_intake_proposal_lines.delete','Xóa chi tiết đề xuất','ingredient_intake_proposal_lines','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(100,'PER00000100','ingredient_intake_proposal_lines.list','Danh sách chi tiết đề xuất','ingredient_intake_proposal_lines','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(101,'PER00000101','ingredient_actual_intakes.create','Tạo phiếu nhập kho','ingredient_actual_intakes','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(102,'PER00000102','ingredient_actual_intakes.read','Xem phiếu nhập kho','ingredient_actual_intakes','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(103,'PER00000103','ingredient_actual_intakes.update','Cập nhật phiếu nhập','ingredient_actual_intakes','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(104,'PER00000104','ingredient_actual_intakes.delete','Xóa phiếu nhập kho','ingredient_actual_intakes','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(105,'PER00000105','ingredient_actual_intakes.list','Danh sách phiếu nhập','ingredient_actual_intakes','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(106,'PER00000106','ingredient_actual_intake_lines.create','Tạo chi tiết phiếu nhập','ingredient_actual_intake_lines','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(107,'PER00000107','ingredient_actual_intake_lines.read','Xem chi tiết phiếu nhập','ingredient_actual_intake_lines','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(108,'PER00000108','ingredient_actual_intake_lines.update','Cập nhật chi tiết nhập','ingredient_actual_intake_lines','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(109,'PER00000109','ingredient_actual_intake_lines.delete','Xóa chi tiết phiếu nhập','ingredient_actual_intake_lines','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(110,'PER00000110','ingredient_actual_intake_lines.list','Danh sách chi tiết nhập','ingredient_actual_intake_lines','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(111,'PER00000111','dishes.create','Tạo món ăn','dishes','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(112,'PER00000112','dishes.read','Xem món ăn','dishes','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(113,'PER00000113','dishes.update','Cập nhật món ăn','dishes','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(114,'PER00000114','dishes.delete','Xóa món ăn','dishes','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(115,'PER00000115','dishes.list','Danh sách món ăn','dishes','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(116,'PER00000116','dish_ingredients.create','Thêm nguyên liệu món','dish_ingredients','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(117,'PER00000117','dish_ingredients.read','Xem nguyên liệu món','dish_ingredients','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(118,'PER00000118','dish_ingredients.update','Cập nhật nguyên liệu món','dish_ingredients','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(119,'PER00000119','dish_ingredients.delete','Xóa nguyên liệu món','dish_ingredients','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(120,'PER00000120','dish_ingredients.list','Danh sách nguyên liệu món','dish_ingredients','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(121,'PER00000121','weekly_menus.create','Tạo thực đơn tuần','weekly_menus','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(122,'PER00000122','weekly_menus.read','Xem thực đơn tuần','weekly_menus','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(123,'PER00000123','weekly_menus.update','Cập nhật thực đơn','weekly_menus','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(124,'PER00000124','weekly_menus.delete','Xóa thực đơn tuần','weekly_menus','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(125,'PER00000125','weekly_menus.list','Danh sách thực đơn','weekly_menus','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(126,'PER00000126','menu_schedule.create','Tạo lịch thực đơn','menu_schedule','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(127,'PER00000127','menu_schedule.read','Xem lịch thực đơn','menu_schedule','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(128,'PER00000128','menu_schedule.update','Cập nhật lịch thực đơn','menu_schedule','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(129,'PER00000129','menu_schedule.delete','Xóa lịch thực đơn','menu_schedule','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(130,'PER00000130','menu_schedule.list','Danh sách lịch thực đơn','menu_schedule','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(131,'PER00000131','orders.create','Tạo đơn hàng','orders','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(132,'PER00000132','orders.read','Xem đơn hàng','orders','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(133,'PER00000133','orders.update','Cập nhật đơn hàng','orders','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(134,'PER00000134','orders.delete','Xóa đơn hàng','orders','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(135,'PER00000135','orders.list','Danh sách đơn hàng','orders','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(136,'PER00000136','order_items.create','Thêm chi tiết đơn','order_items','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(137,'PER00000137','order_items.read','Xem chi tiết đơn','order_items','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(138,'PER00000138','order_items.update','Cập nhật chi tiết đơn','order_items','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(139,'PER00000139','order_items.delete','Xóa chi tiết đơn','order_items','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(140,'PER00000140','order_items.list','Danh sách chi tiết đơn','order_items','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(141,'PER00000141','deliveries.create','Tạo giao hàng','deliveries','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(142,'PER00000142','deliveries.read','Xem giao hàng','deliveries','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(143,'PER00000143','deliveries.update','Cập nhật giao hàng','deliveries','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(144,'PER00000144','deliveries.delete','Xóa giao hàng','deliveries','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(145,'PER00000145','deliveries.list','Danh sách giao hàng','deliveries','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(146,'PER00000146','payments.create','Tạo thanh toán','payments','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(147,'PER00000147','payments.read','Xem thanh toán','payments','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(148,'PER00000148','payments.update','Cập nhật thanh toán','payments','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(149,'PER00000149','payments.delete','Xóa thanh toán','payments','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(150,'PER00000150','payments.list','Danh sách thanh toán','payments','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(151,'PER00000151','transactions.create','Tạo giao dịch thu chi','transactions','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(152,'PER00000152','transactions.read','Xem giao dịch','transactions','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(153,'PER00000153','transactions.update','Cập nhật giao dịch','transactions','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(154,'PER00000154','transactions.delete','Xóa giao dịch','transactions','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(155,'PER00000155','transactions.list','Danh sách giao dịch','transactions','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(156,'PER00000156','reviews.create','Tạo đánh giá','reviews','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(157,'PER00000157','reviews.read','Xem đánh giá','reviews','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(158,'PER00000158','reviews.update','Cập nhật đánh giá','reviews','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(159,'PER00000159','reviews.delete','Xóa đánh giá','reviews','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(160,'PER00000160','reviews.list','Danh sách đánh giá','reviews','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(161,'PER00000161','sentiments.create','Tạo phân tích cảm xúc','sentiments','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(162,'PER00000162','sentiments.read','Xem phân tích cảm xúc','sentiments','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(163,'PER00000163','sentiments.update','Cập nhật phân tích','sentiments','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(164,'PER00000164','sentiments.delete','Xóa phân tích','sentiments','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(165,'PER00000165','sentiments.list','Danh sách phân tích','sentiments','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(166,'PER00000166','complaints.create','Tạo khiếu nại','complaints','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(167,'PER00000167','complaints.read','Xem khiếu nại','complaints','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(168,'PER00000168','complaints.update','Cập nhật khiếu nại','complaints','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(169,'PER00000169','complaints.delete','Xóa khiếu nại','complaints','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(170,'PER00000170','complaints.list','Danh sách khiếu nại','complaints','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(171,'PER00000171','chatbot_logs.create','Tạo log chatbot','chatbot_logs','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(172,'PER00000172','chatbot_logs.read','Xem log chatbot','chatbot_logs','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(173,'PER00000173','chatbot_logs.update','Cập nhật log chatbot','chatbot_logs','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(174,'PER00000174','chatbot_logs.delete','Xóa log chatbot','chatbot_logs','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(175,'PER00000175','chatbot_logs.list','Danh sách log chatbot','chatbot_logs','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(176,'PER00000176','menu_suggestions.create','Tạo gợi ý thực đơn','menu_suggestions','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(177,'PER00000177','menu_suggestions.read','Xem gợi ý thực đơn','menu_suggestions','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(178,'PER00000178','menu_suggestions.update','Cập nhật gợi ý','menu_suggestions','update',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(179,'PER00000179','menu_suggestions.delete','Xóa gợi ý thực đơn','menu_suggestions','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(180,'PER00000180','menu_suggestions.list','Danh sách gợi ý','menu_suggestions','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(181,'PER00000181','menu_suggestions.generate','Tạo gợi ý thực đơn','menu_suggestions','generate',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(182,'PER00000182','system_logs.create','Tạo log hệ thống','system_logs','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(183,'PER00000183','system_logs.read','Xem log hệ thống','system_logs','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(184,'PER00000184','system_logs.backup','Backup hệ thống','system_logs','backup',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(185,'PER00000185','system_logs.restore','Restore hệ thống','system_logs','restore',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(186,'PER00000186','system_logs.list','Danh sách log hệ thống','system_logs','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(187,'PER00000187','system_backups.create','Tạo backup hệ thống','system_backups','create',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(188,'PER00000188','system_backups.read','Xem backup hệ thống','system_backups','read',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(189,'PER00000189','system_backups.delete','Xóa backup hệ thống','system_backups','delete',1,'2026-05-06 15:53:20',NULL,NULL,NULL),(190,'PER00000190','system_backups.list','Danh sách backup hệ thống','system_backups','list',1,'2026-05-06 15:53:20',NULL,NULL,NULL);
/*!40000 ALTER TABLE `permissions` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_permissions_code` BEFORE INSERT ON `permissions` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('permissions'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `recruitment`
--

DROP TABLE IF EXISTS `recruitment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recruitment` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `Title` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Tiêu đề tuyển dụng',
  `Position` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Vị trí công việc',
  `Location` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Địa điểm làm việc',
  `JobType` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Full-time' COMMENT 'Full-time | Part-time | Freelance',
  `SalaryRange` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mức lương (VD: 10-15 triệu)',
  `Description` text COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Mô tả công việc',
  `Requirements` text COLLATE utf8mb4_unicode_ci COMMENT 'Yêu cầu ứng viên',
  `Benefits` text COLLATE utf8mb4_unicode_ci COMMENT 'Quyền lợi',
  `Deadline` date DEFAULT NULL COMMENT 'Hạn chót ứng tuyển',
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_recruitment_code` (`Code`),
  KEY `IX_recruitment_active` (`IsActive`,`CreatedAt` DESC)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Thông tin tuyển dụng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recruitment`
--

LOCK TABLES `recruitment` WRITE;
/*!40000 ALTER TABLE `recruitment` DISABLE KEYS */;
/*!40000 ALTER TABLE `recruitment` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_recruitment_code` BEFORE INSERT ON `recruitment` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('recruitment'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `reviews`
--

DROP TABLE IF EXISTS `reviews`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reviews` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `UserId` int NOT NULL,
  `DishId` int DEFAULT NULL,
  `OrderId` int DEFAULT NULL,
  `Rating` int NOT NULL COMMENT '1-5',
  `Comment` text COLLATE utf8mb4_unicode_ci,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_reviews_code` (`Code`),
  KEY `IX_reviews_user` (`UserId`),
  KEY `IX_reviews_dish` (`DishId`),
  KEY `FK_reviews_order` (`OrderId`),
  CONSTRAINT `FK_reviews_dish` FOREIGN KEY (`DishId`) REFERENCES `dishes` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_reviews_order` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_reviews_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Đánh giá';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reviews`
--

LOCK TABLES `reviews` WRITE;
/*!40000 ALTER TABLE `reviews` DISABLE KEYS */;
INSERT INTO `reviews` VALUES (1,'RVW00000001',10,1,3,5,'Cơm sườn nướng rất ngon, thịt mềm gia vị vừa ăn!','2026-05-06 15:53:21'),(2,'RVW00000002',11,2,4,4,'Gà chiên mắm ngon, hơi mặn. Cần giảm nước mắm.','2026-05-06 15:53:21'),(3,'RVW00000003',6,3,1,5,'Cơm chiên dương châu rất ngon, nhân viên công ty rất thích!','2026-05-06 15:53:21');
/*!40000 ALTER TABLE `reviews` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_reviews_code` BEFORE INSERT ON `reviews` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('reviews'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `role_permissions`
--

DROP TABLE IF EXISTS `role_permissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `role_permissions` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'M? t? sinh (trigger)',
  `RoleId` int NOT NULL,
  `PermissionId` int NOT NULL,
  `AssignedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `AssignedBy` int DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_role_permissions_role_perm` (`RoleId`,`PermissionId`),
  UNIQUE KEY `UK_role_permissions_code` (`Code`),
  KEY `IX_role_permissions_perm` (`PermissionId`),
  KEY `FK_role_permissions_assigned_by` (`AssignedBy`),
  CONSTRAINT `FK_role_permissions_assigned_by` FOREIGN KEY (`AssignedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_role_permissions_perm` FOREIGN KEY (`PermissionId`) REFERENCES `permissions` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_role_permissions_role` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=841 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Gán quyền cho vai trò';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role_permissions`
--

LOCK TABLES `role_permissions` WRITE;
/*!40000 ALTER TABLE `role_permissions` DISABLE KEYS */;
INSERT INTO `role_permissions` VALUES (1,'RPE00000001',1,1,'2026-05-06 15:53:20',NULL,1),(2,'RPE00000002',1,2,'2026-05-06 15:53:20',NULL,1),(3,'RPE00000003',1,3,'2026-05-06 15:53:20',NULL,1),(4,'RPE00000004',1,4,'2026-05-06 15:53:20',NULL,1),(5,'RPE00000005',1,5,'2026-05-06 15:53:20',NULL,1),(6,'RPE00000006',1,6,'2026-05-06 15:53:20',NULL,1),(7,'RPE00000007',1,7,'2026-05-06 15:53:20',NULL,1),(8,'RPE00000008',1,8,'2026-05-06 15:53:20',NULL,1),(9,'RPE00000009',1,9,'2026-05-06 15:53:20',NULL,1),(10,'RPE00000010',1,10,'2026-05-06 15:53:20',NULL,1),(11,'RPE00000011',1,11,'2026-05-06 15:53:20',NULL,1),(12,'RPE00000012',1,12,'2026-05-06 15:53:20',NULL,1),(13,'RPE00000013',1,13,'2026-05-06 15:53:20',NULL,1),(14,'RPE00000014',1,14,'2026-05-06 15:53:20',NULL,1),(15,'RPE00000015',1,15,'2026-05-06 15:53:20',NULL,1),(16,'RPE00000016',1,16,'2026-05-06 15:53:20',NULL,1),(17,'RPE00000017',1,17,'2026-05-06 15:53:20',NULL,1),(18,'RPE00000018',1,18,'2026-05-06 15:53:20',NULL,1),(19,'RPE00000019',1,19,'2026-05-06 15:53:20',NULL,1),(20,'RPE00000020',1,20,'2026-05-06 15:53:20',NULL,1),(21,'RPE00000021',1,21,'2026-05-06 15:53:20',NULL,1),(22,'RPE00000022',1,22,'2026-05-06 15:53:20',NULL,1),(23,'RPE00000023',1,23,'2026-05-06 15:53:20',NULL,1),(24,'RPE00000024',1,24,'2026-05-06 15:53:20',NULL,1),(25,'RPE00000025',1,25,'2026-05-06 15:53:20',NULL,1),(26,'RPE00000026',1,26,'2026-05-06 15:53:20',NULL,1),(27,'RPE00000027',1,27,'2026-05-06 15:53:20',NULL,1),(28,'RPE00000028',1,28,'2026-05-06 15:53:20',NULL,1),(29,'RPE00000029',1,29,'2026-05-06 15:53:20',NULL,1),(30,'RPE00000030',1,30,'2026-05-06 15:53:20',NULL,1),(31,'RPE00000031',1,31,'2026-05-06 15:53:20',NULL,1),(32,'RPE00000032',1,32,'2026-05-06 15:53:20',NULL,1),(33,'RPE00000033',1,33,'2026-05-06 15:53:20',NULL,1),(34,'RPE00000034',1,34,'2026-05-06 15:53:20',NULL,1),(35,'RPE00000035',1,35,'2026-05-06 15:53:20',NULL,1),(36,'RPE00000036',1,36,'2026-05-06 15:53:20',NULL,1),(37,'RPE00000037',1,37,'2026-05-06 15:53:20',NULL,1),(38,'RPE00000038',1,38,'2026-05-06 15:53:20',NULL,1),(39,'RPE00000039',1,39,'2026-05-06 15:53:20',NULL,1),(40,'RPE00000040',1,40,'2026-05-06 15:53:20',NULL,1),(41,'RPE00000041',1,41,'2026-05-06 15:53:20',NULL,1),(42,'RPE00000042',1,42,'2026-05-06 15:53:20',NULL,1),(43,'RPE00000043',1,43,'2026-05-06 15:53:20',NULL,1),(44,'RPE00000044',1,44,'2026-05-06 15:53:20',NULL,1),(45,'RPE00000045',1,45,'2026-05-06 15:53:20',NULL,1),(46,'RPE00000046',1,46,'2026-05-06 15:53:20',NULL,1),(47,'RPE00000047',1,47,'2026-05-06 15:53:20',NULL,1),(48,'RPE00000048',1,48,'2026-05-06 15:53:20',NULL,1),(49,'RPE00000049',1,49,'2026-05-06 15:53:20',NULL,1),(50,'RPE00000050',1,50,'2026-05-06 15:53:20',NULL,1),(51,'RPE00000051',1,51,'2026-05-06 15:53:20',NULL,1),(52,'RPE00000052',1,52,'2026-05-06 15:53:20',NULL,1),(53,'RPE00000053',1,53,'2026-05-06 15:53:20',NULL,1),(54,'RPE00000054',1,54,'2026-05-06 15:53:20',NULL,1),(55,'RPE00000055',1,55,'2026-05-06 15:53:20',NULL,1),(56,'RPE00000056',1,56,'2026-05-06 15:53:20',NULL,1),(57,'RPE00000057',1,57,'2026-05-06 15:53:20',NULL,1),(58,'RPE00000058',1,58,'2026-05-06 15:53:20',NULL,1),(59,'RPE00000059',1,59,'2026-05-06 15:53:20',NULL,1),(60,'RPE00000060',1,60,'2026-05-06 15:53:20',NULL,1),(61,'RPE00000061',1,61,'2026-05-06 15:53:20',NULL,1),(62,'RPE00000062',1,62,'2026-05-06 15:53:20',NULL,1),(63,'RPE00000063',1,63,'2026-05-06 15:53:20',NULL,1),(64,'RPE00000064',1,64,'2026-05-06 15:53:20',NULL,1),(65,'RPE00000065',1,65,'2026-05-06 15:53:20',NULL,1),(66,'RPE00000066',1,66,'2026-05-06 15:53:20',NULL,1),(67,'RPE00000067',1,67,'2026-05-06 15:53:20',NULL,1),(68,'RPE00000068',1,68,'2026-05-06 15:53:20',NULL,1),(69,'RPE00000069',1,69,'2026-05-06 15:53:20',NULL,1),(70,'RPE00000070',1,70,'2026-05-06 15:53:20',NULL,1),(71,'RPE00000071',1,71,'2026-05-06 15:53:20',NULL,1),(72,'RPE00000072',1,72,'2026-05-06 15:53:20',NULL,1),(73,'RPE00000073',1,73,'2026-05-06 15:53:20',NULL,1),(74,'RPE00000074',1,74,'2026-05-06 15:53:20',NULL,1),(75,'RPE00000075',1,75,'2026-05-06 15:53:20',NULL,1),(76,'RPE00000076',1,76,'2026-05-06 15:53:20',NULL,1),(77,'RPE00000077',1,77,'2026-05-06 15:53:20',NULL,1),(78,'RPE00000078',1,78,'2026-05-06 15:53:20',NULL,1),(79,'RPE00000079',1,79,'2026-05-06 15:53:20',NULL,1),(80,'RPE00000080',1,80,'2026-05-06 15:53:20',NULL,1),(81,'RPE00000081',1,81,'2026-05-06 15:53:20',NULL,1),(82,'RPE00000082',1,82,'2026-05-06 15:53:20',NULL,1),(83,'RPE00000083',1,83,'2026-05-06 15:53:20',NULL,1),(84,'RPE00000084',1,84,'2026-05-06 15:53:20',NULL,1),(85,'RPE00000085',1,85,'2026-05-06 15:53:20',NULL,1),(86,'RPE00000086',1,86,'2026-05-06 15:53:20',NULL,1),(87,'RPE00000087',1,87,'2026-05-06 15:53:20',NULL,1),(88,'RPE00000088',1,88,'2026-05-06 15:53:20',NULL,1),(89,'RPE00000089',1,89,'2026-05-06 15:53:20',NULL,1),(90,'RPE00000090',1,90,'2026-05-06 15:53:20',NULL,1),(91,'RPE00000091',1,91,'2026-05-06 15:53:20',NULL,1),(92,'RPE00000092',1,92,'2026-05-06 15:53:20',NULL,1),(93,'RPE00000093',1,93,'2026-05-06 15:53:20',NULL,1),(94,'RPE00000094',1,94,'2026-05-06 15:53:20',NULL,1),(95,'RPE00000095',1,95,'2026-05-06 15:53:20',NULL,1),(96,'RPE00000096',1,96,'2026-05-06 15:53:20',NULL,1),(97,'RPE00000097',1,97,'2026-05-06 15:53:20',NULL,1),(98,'RPE00000098',1,98,'2026-05-06 15:53:20',NULL,1),(99,'RPE00000099',1,99,'2026-05-06 15:53:20',NULL,1),(100,'RPE00000100',1,100,'2026-05-06 15:53:20',NULL,1),(101,'RPE00000101',1,101,'2026-05-06 15:53:20',NULL,1),(102,'RPE00000102',1,102,'2026-05-06 15:53:20',NULL,1),(103,'RPE00000103',1,103,'2026-05-06 15:53:20',NULL,1),(104,'RPE00000104',1,104,'2026-05-06 15:53:20',NULL,1),(105,'RPE00000105',1,105,'2026-05-06 15:53:20',NULL,1),(106,'RPE00000106',1,106,'2026-05-06 15:53:20',NULL,1),(107,'RPE00000107',1,107,'2026-05-06 15:53:20',NULL,1),(108,'RPE00000108',1,108,'2026-05-06 15:53:20',NULL,1),(109,'RPE00000109',1,109,'2026-05-06 15:53:20',NULL,1),(110,'RPE00000110',1,110,'2026-05-06 15:53:20',NULL,1),(111,'RPE00000111',1,111,'2026-05-06 15:53:20',NULL,1),(112,'RPE00000112',1,112,'2026-05-06 15:53:20',NULL,1),(113,'RPE00000113',1,113,'2026-05-06 15:53:20',NULL,1),(114,'RPE00000114',1,114,'2026-05-06 15:53:20',NULL,1),(115,'RPE00000115',1,115,'2026-05-06 15:53:20',NULL,1),(116,'RPE00000116',1,116,'2026-05-06 15:53:20',NULL,1),(117,'RPE00000117',1,117,'2026-05-06 15:53:20',NULL,1),(118,'RPE00000118',1,118,'2026-05-06 15:53:20',NULL,1),(119,'RPE00000119',1,119,'2026-05-06 15:53:20',NULL,1),(120,'RPE00000120',1,120,'2026-05-06 15:53:20',NULL,1),(121,'RPE00000121',1,121,'2026-05-06 15:53:20',NULL,1),(122,'RPE00000122',1,122,'2026-05-06 15:53:20',NULL,1),(123,'RPE00000123',1,123,'2026-05-06 15:53:20',NULL,1),(124,'RPE00000124',1,124,'2026-05-06 15:53:20',NULL,1),(125,'RPE00000125',1,125,'2026-05-06 15:53:20',NULL,1),(126,'RPE00000126',1,126,'2026-05-06 15:53:20',NULL,1),(127,'RPE00000127',1,127,'2026-05-06 15:53:20',NULL,1),(128,'RPE00000128',1,128,'2026-05-06 15:53:20',NULL,1),(129,'RPE00000129',1,129,'2026-05-06 15:53:20',NULL,1),(130,'RPE00000130',1,130,'2026-05-06 15:53:20',NULL,1),(131,'RPE00000131',1,131,'2026-05-06 15:53:20',NULL,1),(132,'RPE00000132',1,132,'2026-05-06 15:53:20',NULL,1),(133,'RPE00000133',1,133,'2026-05-06 15:53:20',NULL,1),(134,'RPE00000134',1,134,'2026-05-06 15:53:20',NULL,1),(135,'RPE00000135',1,135,'2026-05-06 15:53:20',NULL,1),(136,'RPE00000136',1,136,'2026-05-06 15:53:20',NULL,1),(137,'RPE00000137',1,137,'2026-05-06 15:53:20',NULL,1),(138,'RPE00000138',1,138,'2026-05-06 15:53:20',NULL,1),(139,'RPE00000139',1,139,'2026-05-06 15:53:20',NULL,1),(140,'RPE00000140',1,140,'2026-05-06 15:53:20',NULL,1),(141,'RPE00000141',1,141,'2026-05-06 15:53:20',NULL,1),(142,'RPE00000142',1,142,'2026-05-06 15:53:20',NULL,1),(143,'RPE00000143',1,143,'2026-05-06 15:53:20',NULL,1),(144,'RPE00000144',1,144,'2026-05-06 15:53:20',NULL,1),(145,'RPE00000145',1,145,'2026-05-06 15:53:20',NULL,1),(146,'RPE00000146',1,146,'2026-05-06 15:53:20',NULL,1),(147,'RPE00000147',1,147,'2026-05-06 15:53:20',NULL,1),(148,'RPE00000148',1,148,'2026-05-06 15:53:20',NULL,1),(149,'RPE00000149',1,149,'2026-05-06 15:53:20',NULL,1),(150,'RPE00000150',1,150,'2026-05-06 15:53:20',NULL,1),(151,'RPE00000151',1,151,'2026-05-06 15:53:20',NULL,1),(152,'RPE00000152',1,152,'2026-05-06 15:53:20',NULL,1),(153,'RPE00000153',1,153,'2026-05-06 15:53:20',NULL,1),(154,'RPE00000154',1,154,'2026-05-06 15:53:20',NULL,1),(155,'RPE00000155',1,155,'2026-05-06 15:53:20',NULL,1),(156,'RPE00000156',1,156,'2026-05-06 15:53:20',NULL,1),(157,'RPE00000157',1,157,'2026-05-06 15:53:20',NULL,1),(158,'RPE00000158',1,158,'2026-05-06 15:53:20',NULL,1),(159,'RPE00000159',1,159,'2026-05-06 15:53:20',NULL,1),(160,'RPE00000160',1,160,'2026-05-06 15:53:20',NULL,1),(161,'RPE00000161',1,161,'2026-05-06 15:53:20',NULL,1),(162,'RPE00000162',1,162,'2026-05-06 15:53:20',NULL,1),(163,'RPE00000163',1,163,'2026-05-06 15:53:20',NULL,1),(164,'RPE00000164',1,164,'2026-05-06 15:53:20',NULL,1),(165,'RPE00000165',1,165,'2026-05-06 15:53:20',NULL,1),(166,'RPE00000166',1,166,'2026-05-06 15:53:20',NULL,1),(167,'RPE00000167',1,167,'2026-05-06 15:53:20',NULL,1),(168,'RPE00000168',1,168,'2026-05-06 15:53:20',NULL,1),(169,'RPE00000169',1,169,'2026-05-06 15:53:20',NULL,1),(170,'RPE00000170',1,170,'2026-05-06 15:53:20',NULL,1),(171,'RPE00000171',1,171,'2026-05-06 15:53:20',NULL,1),(172,'RPE00000172',1,172,'2026-05-06 15:53:20',NULL,1),(173,'RPE00000173',1,173,'2026-05-06 15:53:20',NULL,1),(174,'RPE00000174',1,174,'2026-05-06 15:53:20',NULL,1),(175,'RPE00000175',1,175,'2026-05-06 15:53:20',NULL,1),(176,'RPE00000176',1,176,'2026-05-06 15:53:20',NULL,1),(177,'RPE00000177',1,177,'2026-05-06 15:53:20',NULL,1),(178,'RPE00000178',1,178,'2026-05-06 15:53:20',NULL,1),(179,'RPE00000179',1,179,'2026-05-06 15:53:20',NULL,1),(180,'RPE00000180',1,180,'2026-05-06 15:53:20',NULL,1),(181,'RPE00000181',1,181,'2026-05-06 15:53:20',NULL,1),(182,'RPE00000182',1,182,'2026-05-06 15:53:20',NULL,1),(183,'RPE00000183',1,183,'2026-05-06 15:53:20',NULL,1),(184,'RPE00000184',1,184,'2026-05-06 15:53:20',NULL,1),(185,'RPE00000185',1,185,'2026-05-06 15:53:20',NULL,1),(186,'RPE00000186',1,186,'2026-05-06 15:53:20',NULL,1),(187,'RPE00000187',1,187,'2026-05-06 15:53:20',NULL,1),(188,'RPE00000188',1,188,'2026-05-06 15:53:20',NULL,1),(189,'RPE00000189',1,189,'2026-05-06 15:53:20',NULL,1),(190,'RPE00000190',1,190,'2026-05-06 15:53:20',NULL,1),(256,'RPE00000191',2,166,'2026-05-06 15:53:20',NULL,1),(257,'RPE00000192',2,169,'2026-05-06 15:53:20',NULL,1),(258,'RPE00000193',2,170,'2026-05-06 15:53:20',NULL,1),(259,'RPE00000194',2,167,'2026-05-06 15:53:20',NULL,1),(260,'RPE00000195',2,168,'2026-05-06 15:53:20',NULL,1),(261,'RPE00000196',2,56,'2026-05-06 15:53:20',NULL,1),(262,'RPE00000197',2,59,'2026-05-06 15:53:20',NULL,1),(263,'RPE00000198',2,60,'2026-05-06 15:53:20',NULL,1),(264,'RPE00000199',2,57,'2026-05-06 15:53:20',NULL,1),(265,'RPE00000200',2,58,'2026-05-06 15:53:20',NULL,1),(266,'RPE00000201',2,141,'2026-05-06 15:53:20',NULL,1),(267,'RPE00000202',2,144,'2026-05-06 15:53:20',NULL,1),(268,'RPE00000203',2,145,'2026-05-06 15:53:20',NULL,1),(269,'RPE00000204',2,142,'2026-05-06 15:53:20',NULL,1),(270,'RPE00000205',2,143,'2026-05-06 15:53:20',NULL,1),(271,'RPE00000206',2,111,'2026-05-06 15:53:20',NULL,1),(272,'RPE00000207',2,114,'2026-05-06 15:53:20',NULL,1),(273,'RPE00000208',2,115,'2026-05-06 15:53:20',NULL,1),(274,'RPE00000209',2,112,'2026-05-06 15:53:20',NULL,1),(275,'RPE00000210',2,113,'2026-05-06 15:53:20',NULL,1),(276,'RPE00000211',2,66,'2026-05-06 15:53:20',NULL,1),(277,'RPE00000212',2,69,'2026-05-06 15:53:20',NULL,1),(278,'RPE00000213',2,70,'2026-05-06 15:53:20',NULL,1),(279,'RPE00000214',2,67,'2026-05-06 15:53:20',NULL,1),(280,'RPE00000215',2,68,'2026-05-06 15:53:20',NULL,1),(281,'RPE00000216',2,76,'2026-05-06 15:53:20',NULL,1),(282,'RPE00000217',2,79,'2026-05-06 15:53:20',NULL,1),(283,'RPE00000218',2,80,'2026-05-06 15:53:20',NULL,1),(284,'RPE00000219',2,77,'2026-05-06 15:53:20',NULL,1),(285,'RPE00000220',2,78,'2026-05-06 15:53:20',NULL,1),(286,'RPE00000221',2,126,'2026-05-06 15:53:20',NULL,1),(287,'RPE00000222',2,129,'2026-05-06 15:53:20',NULL,1),(288,'RPE00000223',2,130,'2026-05-06 15:53:20',NULL,1),(289,'RPE00000224',2,127,'2026-05-06 15:53:20',NULL,1),(290,'RPE00000225',2,128,'2026-05-06 15:53:20',NULL,1),(291,'RPE00000226',2,176,'2026-05-06 15:53:20',NULL,1),(292,'RPE00000227',2,179,'2026-05-06 15:53:20',NULL,1),(293,'RPE00000228',2,181,'2026-05-06 15:53:20',NULL,1),(294,'RPE00000229',2,180,'2026-05-06 15:53:20',NULL,1),(295,'RPE00000230',2,177,'2026-05-06 15:53:20',NULL,1),(296,'RPE00000231',2,178,'2026-05-06 15:53:20',NULL,1),(297,'RPE00000232',2,136,'2026-05-06 15:53:20',NULL,1),(298,'RPE00000233',2,139,'2026-05-06 15:53:20',NULL,1),(299,'RPE00000234',2,140,'2026-05-06 15:53:20',NULL,1),(300,'RPE00000235',2,137,'2026-05-06 15:53:20',NULL,1),(301,'RPE00000236',2,138,'2026-05-06 15:53:20',NULL,1),(302,'RPE00000237',2,131,'2026-05-06 15:53:20',NULL,1),(303,'RPE00000238',2,134,'2026-05-06 15:53:20',NULL,1),(304,'RPE00000239',2,135,'2026-05-06 15:53:20',NULL,1),(305,'RPE00000240',2,132,'2026-05-06 15:53:20',NULL,1),(306,'RPE00000241',2,133,'2026-05-06 15:53:20',NULL,1),(307,'RPE00000242',2,41,'2026-05-06 15:53:20',NULL,1),(308,'RPE00000243',2,44,'2026-05-06 15:53:20',NULL,1),(309,'RPE00000244',2,45,'2026-05-06 15:53:20',NULL,1),(310,'RPE00000245',2,42,'2026-05-06 15:53:20',NULL,1),(311,'RPE00000246',2,43,'2026-05-06 15:53:20',NULL,1),(312,'RPE00000247',2,61,'2026-05-06 15:53:20',NULL,1),(313,'RPE00000248',2,64,'2026-05-06 15:53:20',NULL,1),(314,'RPE00000249',2,65,'2026-05-06 15:53:20',NULL,1),(315,'RPE00000250',2,62,'2026-05-06 15:53:20',NULL,1),(316,'RPE00000251',2,63,'2026-05-06 15:53:20',NULL,1),(317,'RPE00000252',2,51,'2026-05-06 15:53:20',NULL,1),(318,'RPE00000253',2,54,'2026-05-06 15:53:20',NULL,1),(319,'RPE00000254',2,55,'2026-05-06 15:53:20',NULL,1),(320,'RPE00000255',2,52,'2026-05-06 15:53:20',NULL,1),(321,'RPE00000256',2,53,'2026-05-06 15:53:20',NULL,1),(322,'RPE00000257',2,146,'2026-05-06 15:53:20',NULL,1),(323,'RPE00000258',2,149,'2026-05-06 15:53:20',NULL,1),(324,'RPE00000259',2,150,'2026-05-06 15:53:20',NULL,1),(325,'RPE00000260',2,147,'2026-05-06 15:53:20',NULL,1),(326,'RPE00000261',2,148,'2026-05-06 15:53:20',NULL,1),(327,'RPE00000262',2,156,'2026-05-06 15:53:20',NULL,1),(328,'RPE00000263',2,159,'2026-05-06 15:53:20',NULL,1),(329,'RPE00000264',2,160,'2026-05-06 15:53:20',NULL,1),(330,'RPE00000265',2,157,'2026-05-06 15:53:20',NULL,1),(331,'RPE00000266',2,158,'2026-05-06 15:53:20',NULL,1),(332,'RPE00000267',2,161,'2026-05-06 15:53:20',NULL,1),(333,'RPE00000268',2,164,'2026-05-06 15:53:20',NULL,1),(334,'RPE00000269',2,165,'2026-05-06 15:53:20',NULL,1),(335,'RPE00000270',2,162,'2026-05-06 15:53:20',NULL,1),(336,'RPE00000271',2,163,'2026-05-06 15:53:20',NULL,1),(337,'RPE00000272',2,151,'2026-05-06 15:53:20',NULL,1),(338,'RPE00000273',2,154,'2026-05-06 15:53:20',NULL,1),(339,'RPE00000274',2,155,'2026-05-06 15:53:20',NULL,1),(340,'RPE00000275',2,152,'2026-05-06 15:53:20',NULL,1),(341,'RPE00000276',2,153,'2026-05-06 15:53:20',NULL,1),(342,'RPE00000277',2,46,'2026-05-06 15:53:20',NULL,1),(343,'RPE00000278',2,49,'2026-05-06 15:53:20',NULL,1),(344,'RPE00000279',2,50,'2026-05-06 15:53:20',NULL,1),(345,'RPE00000280',2,47,'2026-05-06 15:53:20',NULL,1),(346,'RPE00000281',2,48,'2026-05-06 15:53:20',NULL,1),(347,'RPE00000282',2,1,'2026-05-06 15:53:20',NULL,1),(348,'RPE00000283',2,4,'2026-05-06 15:53:20',NULL,1),(349,'RPE00000284',2,5,'2026-05-06 15:53:20',NULL,1),(350,'RPE00000285',2,2,'2026-05-06 15:53:20',NULL,1),(351,'RPE00000286',2,3,'2026-05-06 15:53:20',NULL,1),(352,'RPE00000287',2,121,'2026-05-06 15:53:20',NULL,1),(353,'RPE00000288',2,124,'2026-05-06 15:53:20',NULL,1),(354,'RPE00000289',2,125,'2026-05-06 15:53:20',NULL,1),(355,'RPE00000290',2,122,'2026-05-06 15:53:20',NULL,1),(356,'RPE00000291',2,123,'2026-05-06 15:53:20',NULL,1),(383,'RPE00000292',3,116,'2026-05-06 15:53:20',NULL,1),(384,'RPE00000293',3,119,'2026-05-06 15:53:20',NULL,1),(385,'RPE00000294',3,120,'2026-05-06 15:53:20',NULL,1),(386,'RPE00000295',3,117,'2026-05-06 15:53:20',NULL,1),(387,'RPE00000296',3,118,'2026-05-06 15:53:20',NULL,1),(388,'RPE00000297',3,111,'2026-05-06 15:53:20',NULL,1),(389,'RPE00000298',3,114,'2026-05-06 15:53:20',NULL,1),(390,'RPE00000299',3,115,'2026-05-06 15:53:20',NULL,1),(391,'RPE00000300',3,112,'2026-05-06 15:53:20',NULL,1),(392,'RPE00000301',3,113,'2026-05-06 15:53:20',NULL,1),(393,'RPE00000302',3,106,'2026-05-06 15:53:20',NULL,1),(394,'RPE00000303',3,109,'2026-05-06 15:53:20',NULL,1),(395,'RPE00000304',3,110,'2026-05-06 15:53:20',NULL,1),(396,'RPE00000305',3,107,'2026-05-06 15:53:20',NULL,1),(397,'RPE00000306',3,108,'2026-05-06 15:53:20',NULL,1),(398,'RPE00000307',3,101,'2026-05-06 15:53:20',NULL,1),(399,'RPE00000308',3,104,'2026-05-06 15:53:20',NULL,1),(400,'RPE00000309',3,105,'2026-05-06 15:53:20',NULL,1),(401,'RPE00000310',3,102,'2026-05-06 15:53:20',NULL,1),(402,'RPE00000311',3,103,'2026-05-06 15:53:20',NULL,1),(403,'RPE00000312',3,96,'2026-05-06 15:53:20',NULL,1),(404,'RPE00000313',3,99,'2026-05-06 15:53:20',NULL,1),(405,'RPE00000314',3,100,'2026-05-06 15:53:20',NULL,1),(406,'RPE00000315',3,97,'2026-05-06 15:53:20',NULL,1),(407,'RPE00000316',3,98,'2026-05-06 15:53:20',NULL,1),(408,'RPE00000317',3,91,'2026-05-06 15:53:20',NULL,1),(409,'RPE00000318',3,94,'2026-05-06 15:53:20',NULL,1),(410,'RPE00000319',3,95,'2026-05-06 15:53:20',NULL,1),(411,'RPE00000320',3,92,'2026-05-06 15:53:20',NULL,1),(412,'RPE00000321',3,93,'2026-05-06 15:53:20',NULL,1),(413,'RPE00000322',3,71,'2026-05-06 15:53:20',NULL,1),(414,'RPE00000323',3,74,'2026-05-06 15:53:20',NULL,1),(415,'RPE00000324',3,75,'2026-05-06 15:53:20',NULL,1),(416,'RPE00000325',3,72,'2026-05-06 15:53:20',NULL,1),(417,'RPE00000326',3,73,'2026-05-06 15:53:20',NULL,1),(418,'RPE00000327',3,66,'2026-05-06 15:53:20',NULL,1),(419,'RPE00000328',3,69,'2026-05-06 15:53:20',NULL,1),(420,'RPE00000329',3,70,'2026-05-06 15:53:20',NULL,1),(421,'RPE00000330',3,67,'2026-05-06 15:53:20',NULL,1),(422,'RPE00000331',3,68,'2026-05-06 15:53:20',NULL,1),(423,'RPE00000332',3,86,'2026-05-06 15:53:20',NULL,1),(424,'RPE00000333',3,89,'2026-05-06 15:53:20',NULL,1),(425,'RPE00000334',3,90,'2026-05-06 15:53:20',NULL,1),(426,'RPE00000335',3,87,'2026-05-06 15:53:20',NULL,1),(427,'RPE00000336',3,88,'2026-05-06 15:53:20',NULL,1),(428,'RPE00000337',3,81,'2026-05-06 15:53:20',NULL,1),(429,'RPE00000338',3,84,'2026-05-06 15:53:20',NULL,1),(430,'RPE00000339',3,85,'2026-05-06 15:53:20',NULL,1),(431,'RPE00000340',3,82,'2026-05-06 15:53:20',NULL,1),(432,'RPE00000341',3,83,'2026-05-06 15:53:20',NULL,1),(433,'RPE00000342',3,76,'2026-05-06 15:53:20',NULL,1),(434,'RPE00000343',3,79,'2026-05-06 15:53:20',NULL,1),(435,'RPE00000344',3,80,'2026-05-06 15:53:20',NULL,1),(436,'RPE00000345',3,77,'2026-05-06 15:53:20',NULL,1),(437,'RPE00000346',3,78,'2026-05-06 15:53:20',NULL,1),(438,'RPE00000347',3,36,'2026-05-06 15:53:20',NULL,1),(439,'RPE00000348',3,39,'2026-05-06 15:53:20',NULL,1),(440,'RPE00000349',3,40,'2026-05-06 15:53:20',NULL,1),(441,'RPE00000350',3,37,'2026-05-06 15:53:20',NULL,1),(442,'RPE00000351',3,38,'2026-05-06 15:53:20',NULL,1),(443,'RPE00000352',3,126,'2026-05-06 15:53:20',NULL,1),(444,'RPE00000353',3,129,'2026-05-06 15:53:20',NULL,1),(445,'RPE00000354',3,130,'2026-05-06 15:53:20',NULL,1),(446,'RPE00000355',3,127,'2026-05-06 15:53:20',NULL,1),(447,'RPE00000356',3,128,'2026-05-06 15:53:20',NULL,1),(448,'RPE00000357',3,136,'2026-05-06 15:53:20',NULL,1),(449,'RPE00000358',3,139,'2026-05-06 15:53:20',NULL,1),(450,'RPE00000359',3,140,'2026-05-06 15:53:20',NULL,1),(451,'RPE00000360',3,137,'2026-05-06 15:53:20',NULL,1),(452,'RPE00000361',3,138,'2026-05-06 15:53:20',NULL,1),(453,'RPE00000362',3,131,'2026-05-06 15:53:20',NULL,1),(454,'RPE00000363',3,134,'2026-05-06 15:53:20',NULL,1),(455,'RPE00000364',3,135,'2026-05-06 15:53:20',NULL,1),(456,'RPE00000365',3,132,'2026-05-06 15:53:20',NULL,1),(457,'RPE00000366',3,133,'2026-05-06 15:53:20',NULL,1),(458,'RPE00000367',3,121,'2026-05-06 15:53:20',NULL,1),(459,'RPE00000368',3,124,'2026-05-06 15:53:20',NULL,1),(460,'RPE00000369',3,125,'2026-05-06 15:53:20',NULL,1),(461,'RPE00000370',3,122,'2026-05-06 15:53:20',NULL,1),(462,'RPE00000371',3,123,'2026-05-06 15:53:20',NULL,1),(510,'RPE00000372',4,116,'2026-05-06 15:53:20',NULL,1),(511,'RPE00000373',4,119,'2026-05-06 15:53:20',NULL,1),(512,'RPE00000374',4,120,'2026-05-06 15:53:20',NULL,1),(513,'RPE00000375',4,117,'2026-05-06 15:53:20',NULL,1),(514,'RPE00000376',4,118,'2026-05-06 15:53:20',NULL,1),(515,'RPE00000377',4,111,'2026-05-06 15:53:20',NULL,1),(516,'RPE00000378',4,114,'2026-05-06 15:53:20',NULL,1),(517,'RPE00000379',4,115,'2026-05-06 15:53:20',NULL,1),(518,'RPE00000380',4,112,'2026-05-06 15:53:20',NULL,1),(519,'RPE00000381',4,113,'2026-05-06 15:53:20',NULL,1),(520,'RPE00000382',4,106,'2026-05-06 15:53:20',NULL,1),(521,'RPE00000383',4,109,'2026-05-06 15:53:20',NULL,1),(522,'RPE00000384',4,110,'2026-05-06 15:53:20',NULL,1),(523,'RPE00000385',4,107,'2026-05-06 15:53:20',NULL,1),(524,'RPE00000386',4,108,'2026-05-06 15:53:20',NULL,1),(525,'RPE00000387',4,101,'2026-05-06 15:53:20',NULL,1),(526,'RPE00000388',4,104,'2026-05-06 15:53:20',NULL,1),(527,'RPE00000389',4,105,'2026-05-06 15:53:20',NULL,1),(528,'RPE00000390',4,102,'2026-05-06 15:53:20',NULL,1),(529,'RPE00000391',4,103,'2026-05-06 15:53:20',NULL,1),(530,'RPE00000392',4,96,'2026-05-06 15:53:20',NULL,1),(531,'RPE00000393',4,99,'2026-05-06 15:53:20',NULL,1),(532,'RPE00000394',4,100,'2026-05-06 15:53:20',NULL,1),(533,'RPE00000395',4,97,'2026-05-06 15:53:20',NULL,1),(534,'RPE00000396',4,98,'2026-05-06 15:53:20',NULL,1),(535,'RPE00000397',4,91,'2026-05-06 15:53:20',NULL,1),(536,'RPE00000398',4,94,'2026-05-06 15:53:20',NULL,1),(537,'RPE00000399',4,95,'2026-05-06 15:53:20',NULL,1),(538,'RPE00000400',4,92,'2026-05-06 15:53:20',NULL,1),(539,'RPE00000401',4,93,'2026-05-06 15:53:20',NULL,1),(540,'RPE00000402',4,71,'2026-05-06 15:53:20',NULL,1),(541,'RPE00000403',4,74,'2026-05-06 15:53:20',NULL,1),(542,'RPE00000404',4,75,'2026-05-06 15:53:20',NULL,1),(543,'RPE00000405',4,72,'2026-05-06 15:53:20',NULL,1),(544,'RPE00000406',4,73,'2026-05-06 15:53:20',NULL,1),(545,'RPE00000407',4,66,'2026-05-06 15:53:20',NULL,1),(546,'RPE00000408',4,69,'2026-05-06 15:53:20',NULL,1),(547,'RPE00000409',4,70,'2026-05-06 15:53:20',NULL,1),(548,'RPE00000410',4,67,'2026-05-06 15:53:20',NULL,1),(549,'RPE00000411',4,68,'2026-05-06 15:53:20',NULL,1),(550,'RPE00000412',4,86,'2026-05-06 15:53:20',NULL,1),(551,'RPE00000413',4,89,'2026-05-06 15:53:20',NULL,1),(552,'RPE00000414',4,90,'2026-05-06 15:53:20',NULL,1),(553,'RPE00000415',4,87,'2026-05-06 15:53:20',NULL,1),(554,'RPE00000416',4,88,'2026-05-06 15:53:20',NULL,1),(555,'RPE00000417',4,81,'2026-05-06 15:53:20',NULL,1),(556,'RPE00000418',4,84,'2026-05-06 15:53:20',NULL,1),(557,'RPE00000419',4,85,'2026-05-06 15:53:20',NULL,1),(558,'RPE00000420',4,82,'2026-05-06 15:53:20',NULL,1),(559,'RPE00000421',4,83,'2026-05-06 15:53:20',NULL,1),(560,'RPE00000422',4,76,'2026-05-06 15:53:20',NULL,1),(561,'RPE00000423',4,79,'2026-05-06 15:53:20',NULL,1),(562,'RPE00000424',4,80,'2026-05-06 15:53:20',NULL,1),(563,'RPE00000425',4,77,'2026-05-06 15:53:20',NULL,1),(564,'RPE00000426',4,78,'2026-05-06 15:53:20',NULL,1),(565,'RPE00000427',4,36,'2026-05-06 15:53:20',NULL,1),(566,'RPE00000428',4,39,'2026-05-06 15:53:20',NULL,1),(567,'RPE00000429',4,40,'2026-05-06 15:53:20',NULL,1),(568,'RPE00000430',4,37,'2026-05-06 15:53:20',NULL,1),(569,'RPE00000431',4,38,'2026-05-06 15:53:20',NULL,1),(570,'RPE00000432',4,126,'2026-05-06 15:53:20',NULL,1),(571,'RPE00000433',4,129,'2026-05-06 15:53:20',NULL,1),(572,'RPE00000434',4,130,'2026-05-06 15:53:20',NULL,1),(573,'RPE00000435',4,127,'2026-05-06 15:53:20',NULL,1),(574,'RPE00000436',4,128,'2026-05-06 15:53:20',NULL,1),(575,'RPE00000437',4,136,'2026-05-06 15:53:20',NULL,1),(576,'RPE00000438',4,139,'2026-05-06 15:53:20',NULL,1),(577,'RPE00000439',4,140,'2026-05-06 15:53:20',NULL,1),(578,'RPE00000440',4,137,'2026-05-06 15:53:20',NULL,1),(579,'RPE00000441',4,138,'2026-05-06 15:53:20',NULL,1),(580,'RPE00000442',4,131,'2026-05-06 15:53:20',NULL,1),(581,'RPE00000443',4,134,'2026-05-06 15:53:20',NULL,1),(582,'RPE00000444',4,135,'2026-05-06 15:53:20',NULL,1),(583,'RPE00000445',4,132,'2026-05-06 15:53:20',NULL,1),(584,'RPE00000446',4,133,'2026-05-06 15:53:20',NULL,1),(585,'RPE00000447',4,121,'2026-05-06 15:53:20',NULL,1),(586,'RPE00000448',4,124,'2026-05-06 15:53:20',NULL,1),(587,'RPE00000449',4,125,'2026-05-06 15:53:20',NULL,1),(588,'RPE00000450',4,122,'2026-05-06 15:53:20',NULL,1),(589,'RPE00000451',4,123,'2026-05-06 15:53:20',NULL,1),(637,'RPE00000452',5,116,'2026-05-06 15:53:20',NULL,1),(638,'RPE00000453',5,119,'2026-05-06 15:53:20',NULL,1),(639,'RPE00000454',5,120,'2026-05-06 15:53:20',NULL,1),(640,'RPE00000455',5,117,'2026-05-06 15:53:20',NULL,1),(641,'RPE00000456',5,118,'2026-05-06 15:53:20',NULL,1),(642,'RPE00000457',5,111,'2026-05-06 15:53:20',NULL,1),(643,'RPE00000458',5,114,'2026-05-06 15:53:20',NULL,1),(644,'RPE00000459',5,115,'2026-05-06 15:53:20',NULL,1),(645,'RPE00000460',5,112,'2026-05-06 15:53:20',NULL,1),(646,'RPE00000461',5,113,'2026-05-06 15:53:20',NULL,1),(647,'RPE00000462',5,106,'2026-05-06 15:53:20',NULL,1),(648,'RPE00000463',5,109,'2026-05-06 15:53:20',NULL,1),(649,'RPE00000464',5,110,'2026-05-06 15:53:20',NULL,1),(650,'RPE00000465',5,107,'2026-05-06 15:53:20',NULL,1),(651,'RPE00000466',5,108,'2026-05-06 15:53:20',NULL,1),(652,'RPE00000467',5,101,'2026-05-06 15:53:20',NULL,1),(653,'RPE00000468',5,104,'2026-05-06 15:53:20',NULL,1),(654,'RPE00000469',5,105,'2026-05-06 15:53:20',NULL,1),(655,'RPE00000470',5,102,'2026-05-06 15:53:20',NULL,1),(656,'RPE00000471',5,103,'2026-05-06 15:53:20',NULL,1),(657,'RPE00000472',5,96,'2026-05-06 15:53:20',NULL,1),(658,'RPE00000473',5,99,'2026-05-06 15:53:20',NULL,1),(659,'RPE00000474',5,100,'2026-05-06 15:53:20',NULL,1),(660,'RPE00000475',5,97,'2026-05-06 15:53:20',NULL,1),(661,'RPE00000476',5,98,'2026-05-06 15:53:20',NULL,1),(662,'RPE00000477',5,91,'2026-05-06 15:53:20',NULL,1),(663,'RPE00000478',5,94,'2026-05-06 15:53:20',NULL,1),(664,'RPE00000479',5,95,'2026-05-06 15:53:20',NULL,1),(665,'RPE00000480',5,92,'2026-05-06 15:53:20',NULL,1),(666,'RPE00000481',5,93,'2026-05-06 15:53:20',NULL,1),(667,'RPE00000482',5,71,'2026-05-06 15:53:20',NULL,1),(668,'RPE00000483',5,74,'2026-05-06 15:53:20',NULL,1),(669,'RPE00000484',5,75,'2026-05-06 15:53:20',NULL,1),(670,'RPE00000485',5,72,'2026-05-06 15:53:20',NULL,1),(671,'RPE00000486',5,73,'2026-05-06 15:53:20',NULL,1),(672,'RPE00000487',5,66,'2026-05-06 15:53:20',NULL,1),(673,'RPE00000488',5,69,'2026-05-06 15:53:20',NULL,1),(674,'RPE00000489',5,70,'2026-05-06 15:53:20',NULL,1),(675,'RPE00000490',5,67,'2026-05-06 15:53:20',NULL,1),(676,'RPE00000491',5,68,'2026-05-06 15:53:20',NULL,1),(677,'RPE00000492',5,86,'2026-05-06 15:53:20',NULL,1),(678,'RPE00000493',5,89,'2026-05-06 15:53:20',NULL,1),(679,'RPE00000494',5,90,'2026-05-06 15:53:20',NULL,1),(680,'RPE00000495',5,87,'2026-05-06 15:53:20',NULL,1),(681,'RPE00000496',5,88,'2026-05-06 15:53:20',NULL,1),(682,'RPE00000497',5,81,'2026-05-06 15:53:20',NULL,1),(683,'RPE00000498',5,84,'2026-05-06 15:53:20',NULL,1),(684,'RPE00000499',5,85,'2026-05-06 15:53:20',NULL,1),(685,'RPE00000500',5,82,'2026-05-06 15:53:20',NULL,1),(686,'RPE00000501',5,83,'2026-05-06 15:53:20',NULL,1),(687,'RPE00000502',5,76,'2026-05-06 15:53:20',NULL,1),(688,'RPE00000503',5,79,'2026-05-06 15:53:20',NULL,1),(689,'RPE00000504',5,80,'2026-05-06 15:53:20',NULL,1),(690,'RPE00000505',5,77,'2026-05-06 15:53:20',NULL,1),(691,'RPE00000506',5,78,'2026-05-06 15:53:20',NULL,1),(692,'RPE00000507',5,36,'2026-05-06 15:53:20',NULL,1),(693,'RPE00000508',5,39,'2026-05-06 15:53:20',NULL,1),(694,'RPE00000509',5,40,'2026-05-06 15:53:20',NULL,1),(695,'RPE00000510',5,37,'2026-05-06 15:53:20',NULL,1),(696,'RPE00000511',5,38,'2026-05-06 15:53:20',NULL,1),(697,'RPE00000512',5,126,'2026-05-06 15:53:20',NULL,1),(698,'RPE00000513',5,129,'2026-05-06 15:53:20',NULL,1),(699,'RPE00000514',5,130,'2026-05-06 15:53:20',NULL,1),(700,'RPE00000515',5,127,'2026-05-06 15:53:20',NULL,1),(701,'RPE00000516',5,128,'2026-05-06 15:53:20',NULL,1),(702,'RPE00000517',5,136,'2026-05-06 15:53:20',NULL,1),(703,'RPE00000518',5,139,'2026-05-06 15:53:20',NULL,1),(704,'RPE00000519',5,140,'2026-05-06 15:53:20',NULL,1),(705,'RPE00000520',5,137,'2026-05-06 15:53:20',NULL,1),(706,'RPE00000521',5,138,'2026-05-06 15:53:20',NULL,1),(707,'RPE00000522',5,131,'2026-05-06 15:53:20',NULL,1),(708,'RPE00000523',5,134,'2026-05-06 15:53:20',NULL,1),(709,'RPE00000524',5,135,'2026-05-06 15:53:20',NULL,1),(710,'RPE00000525',5,132,'2026-05-06 15:53:20',NULL,1),(711,'RPE00000526',5,133,'2026-05-06 15:53:20',NULL,1),(712,'RPE00000527',5,121,'2026-05-06 15:53:20',NULL,1),(713,'RPE00000528',5,124,'2026-05-06 15:53:20',NULL,1),(714,'RPE00000529',5,125,'2026-05-06 15:53:20',NULL,1),(715,'RPE00000530',5,122,'2026-05-06 15:53:20',NULL,1),(716,'RPE00000531',5,123,'2026-05-06 15:53:20',NULL,1),(764,'RPE00000532',6,171,'2026-05-06 15:53:20',NULL,1),(765,'RPE00000533',6,172,'2026-05-06 15:53:20',NULL,1),(766,'RPE00000534',6,166,'2026-05-06 15:53:20',NULL,1),(767,'RPE00000535',6,170,'2026-05-06 15:53:20',NULL,1),(768,'RPE00000536',6,167,'2026-05-06 15:53:20',NULL,1),(769,'RPE00000537',6,145,'2026-05-06 15:53:20',NULL,1),(770,'RPE00000538',6,142,'2026-05-06 15:53:20',NULL,1),(771,'RPE00000539',6,115,'2026-05-06 15:53:20',NULL,1),(772,'RPE00000540',6,112,'2026-05-06 15:53:20',NULL,1),(773,'RPE00000541',6,130,'2026-05-06 15:53:20',NULL,1),(774,'RPE00000542',6,127,'2026-05-06 15:53:20',NULL,1),(775,'RPE00000543',6,136,'2026-05-06 15:53:20',NULL,1),(776,'RPE00000544',6,140,'2026-05-06 15:53:20',NULL,1),(777,'RPE00000545',6,137,'2026-05-06 15:53:20',NULL,1),(778,'RPE00000546',6,131,'2026-05-06 15:53:20',NULL,1),(779,'RPE00000547',6,135,'2026-05-06 15:53:20',NULL,1),(780,'RPE00000548',6,132,'2026-05-06 15:53:20',NULL,1),(781,'RPE00000549',6,156,'2026-05-06 15:53:20',NULL,1),(782,'RPE00000550',6,160,'2026-05-06 15:53:20',NULL,1),(783,'RPE00000551',6,157,'2026-05-06 15:53:20',NULL,1),(784,'RPE00000552',6,125,'2026-05-06 15:53:20',NULL,1),(785,'RPE00000553',6,122,'2026-05-06 15:53:20',NULL,1),(795,'RPE00000554',7,141,'2026-05-06 15:53:20',NULL,1),(796,'RPE00000555',7,144,'2026-05-06 15:53:20',NULL,1),(797,'RPE00000556',7,145,'2026-05-06 15:53:20',NULL,1),(798,'RPE00000557',7,142,'2026-05-06 15:53:20',NULL,1),(799,'RPE00000558',7,143,'2026-05-06 15:53:20',NULL,1),(800,'RPE00000559',7,135,'2026-05-06 15:53:20',NULL,1),(801,'RPE00000560',7,132,'2026-05-06 15:53:20',NULL,1),(802,'RPE00000561',7,133,'2026-05-06 15:53:20',NULL,1),(810,'RPE00000562',8,171,'2026-05-06 15:53:20',NULL,1),(811,'RPE00000563',8,172,'2026-05-06 15:53:20',NULL,1),(812,'RPE00000564',8,142,'2026-05-06 15:53:20',NULL,1),(813,'RPE00000565',8,115,'2026-05-06 15:53:20',NULL,1),(814,'RPE00000566',8,112,'2026-05-06 15:53:20',NULL,1),(815,'RPE00000567',8,130,'2026-05-06 15:53:20',NULL,1),(816,'RPE00000568',8,127,'2026-05-06 15:53:20',NULL,1),(817,'RPE00000569',8,136,'2026-05-06 15:53:20',NULL,1),(818,'RPE00000570',8,140,'2026-05-06 15:53:20',NULL,1),(819,'RPE00000571',8,137,'2026-05-06 15:53:20',NULL,1),(820,'RPE00000572',8,131,'2026-05-06 15:53:20',NULL,1),(821,'RPE00000573',8,135,'2026-05-06 15:53:20',NULL,1),(822,'RPE00000574',8,132,'2026-05-06 15:53:20',NULL,1),(823,'RPE00000575',8,156,'2026-05-06 15:53:20',NULL,1),(824,'RPE00000576',8,160,'2026-05-06 15:53:20',NULL,1),(825,'RPE00000577',8,157,'2026-05-06 15:53:20',NULL,1),(826,'RPE00000578',8,125,'2026-05-06 15:53:20',NULL,1),(827,'RPE00000579',8,122,'2026-05-06 15:53:20',NULL,1);
/*!40000 ALTER TABLE `role_permissions` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_role_permissions_code` BEFORE INSERT ON `role_permissions` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('role_permissions'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `Id` int NOT NULL AUTO_INCREMENT COMMENT 'Auto-increment Primary Key',
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'M? t? sinh (trigger)',
  `Name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Tên vai trò',
  `Description` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mô tả',
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `IsSystemRole` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'Vai trò hệ thống (không xóa được)',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `CreatedBy` int DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_roles_name` (`Name`),
  UNIQUE KEY `UK_roles_code` (`Code`),
  KEY `IX_roles_active_system` (`IsActive`,`IsSystemRole`),
  KEY `FK_roles_created_by` (`CreatedBy`),
  KEY `FK_roles_updated_by` (`UpdatedBy`),
  CONSTRAINT `FK_roles_created_by` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_roles_updated_by` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Vai trò hệ thống';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'ROL00000001','Admin','Quản trị viên, quản lý nghiệp vụ chính',1,1,'2026-05-06 15:53:20',NULL,NULL,NULL),(2,'ROL00000002','Manager','Quản lý công ty suất ăn, xem báo cáo tổng hợp',1,1,'2026-05-06 15:53:20',NULL,NULL,NULL),(3,'ROL00000003','WarehouseStaff','Nhân viên bếp/kho/bán hàng',1,1,'2026-05-06 15:53:20',NULL,NULL,NULL),(4,'ROL00000004','ChefStaff','Nhân viên bếp/kho/bán hàng',1,1,'2026-05-06 15:53:20',NULL,NULL,NULL),(5,'ROL00000005','SalesStaff','Nhân viên bếp/kho/bán hàng',1,1,'2026-05-06 15:53:20',NULL,NULL,NULL),(6,'ROL00000006','Organization','Đại diện đơn vị đặt suất ăn (B2B)',1,1,'2026-05-06 15:53:20',NULL,NULL,NULL),(7,'ROL00000007','Shipper','Nhân viên giao hàng',1,1,'2026-05-06 15:53:20',NULL,NULL,NULL),(8,'ROL00000008','Customer','Người dùng cuối, đặt cơm cá nhân',1,1,'2026-05-06 15:53:20',NULL,NULL,NULL);
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_roles_code` BEFORE INSERT ON `roles` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('roles'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `sentiments`
--

DROP TABLE IF EXISTS `sentiments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sentiments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `ReviewId` int NOT NULL,
  `SentimentLabel` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Confidence` decimal(4,2) DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_sentiments_review` (`ReviewId`),
  UNIQUE KEY `UK_sentiments_code` (`Code`),
  CONSTRAINT `FK_sentiments_review` FOREIGN KEY (`ReviewId`) REFERENCES `reviews` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Phân tích cảm xúc';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sentiments`
--

LOCK TABLES `sentiments` WRITE;
/*!40000 ALTER TABLE `sentiments` DISABLE KEYS */;
INSERT INTO `sentiments` VALUES (1,'STM00000001',1,'positive',0.95,'2026-05-06 15:53:21'),(2,'STM00000002',2,'positive',0.72,'2026-05-06 15:53:21'),(3,'STM00000003',3,'positive',0.98,'2026-05-06 15:53:21');
/*!40000 ALTER TABLE `sentiments` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_sentiments_code` BEFORE INSERT ON `sentiments` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('sentiments'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `system_backups`
--

DROP TABLE IF EXISTS `system_backups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `system_backups` (
  `Id` int NOT NULL AUTO_INCREMENT COMMENT 'Auto-increment Primary Key',
  `FileName` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `FilePath` varchar(1024) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SizeBytes` bigint NOT NULL DEFAULT '0',
  `CreatedAtUtc` datetime NOT NULL,
  `RestoredAtUtc` datetime DEFAULT NULL,
  `DeletedAtUtc` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `idx_system_backups_created_at` (`CreatedAtUtc`),
  KEY `idx_system_backups_is_deleted` (`IsDeleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Backup files metadata';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `system_backups`
--

LOCK TABLES `system_backups` WRITE;
/*!40000 ALTER TABLE `system_backups` DISABLE KEYS */;
/*!40000 ALTER TABLE `system_backups` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `system_logs`
--

DROP TABLE IF EXISTS `system_logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `system_logs` (
  `Id` int NOT NULL AUTO_INCREMENT COMMENT 'Auto-increment Primary Key',
  `Timestamp` datetime DEFAULT NULL,
  `Level` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Template` text COLLATE utf8mb4_unicode_ci,
  `Message` text COLLATE utf8mb4_unicode_ci,
  `Exception` text COLLATE utf8mb4_unicode_ci,
  `Properties` text COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Log hệ thống';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `system_logs`
--

LOCK TABLES `system_logs` WRITE;
/*!40000 ALTER TABLE `system_logs` DISABLE KEYS */;
INSERT INTO `system_logs` VALUES (1,'2026-05-06 15:53:26','Information','Application is shutting down...','Application is shutting down...',NULL,'{\"SourceContext\":\"Microsoft.Hosting.Lifetime\",\"MachineName\":\"AILATUI25\",\"ThreadId\":32,\"Application\":\"SmartLunch-Backend-Service\"}'),(2,'2026-05-06 15:53:26','Information','RabbitMQ Consumer Background Service is stopping.','RabbitMQ Consumer Background Service is stopping.',NULL,'{\"SourceContext\":\"SmartLunch.Backend.Service.API.Services.RabbitMQConsumerBackgroundService\",\"MachineName\":\"AILATUI25\",\"ThreadId\":34,\"Application\":\"SmartLunch-Backend-Service\"}'),(3,'2026-05-06 15:53:26','Information','Consumer closed.','Consumer closed.',NULL,'{\"SourceContext\":\"SmartLunch.Shared.MessageQueue.Dotnet.Services.RabbitMQEventConsumer\",\"MachineName\":\"AILATUI25\",\"ThreadId\":27,\"Application\":\"SmartLunch-Backend-Service\"}'),(4,'2026-05-06 22:53:37','Information','SmartLunch Backend Service starting','SmartLunch Backend Service starting',NULL,'{\"MachineName\":\"AILATUI25\",\"ThreadId\":10,\"Application\":\"SmartLunch-Backend-Service\"}'),(5,'2026-05-06 22:53:37','Information','RabbitMQ Consumer Background Service is starting.','RabbitMQ Consumer Background Service is starting.',NULL,'{\"SourceContext\":\"SmartLunch.Backend.Service.API.Services.RabbitMQConsumerBackgroundService\",\"MachineName\":\"AILATUI25\",\"ThreadId\":11,\"Application\":\"SmartLunch-Backend-Service\"}'),(6,'2026-05-06 22:53:37','Information','DatabaseBackupHostedService disabled.','DatabaseBackupHostedService disabled.',NULL,'{\"SourceContext\":\"SmartLunch.Backend.Service.API.Services.DatabaseBackupHostedService\",\"MachineName\":\"AILATUI25\",\"ThreadId\":12,\"Application\":\"SmartLunch-Backend-Service\"}'),(7,'2026-05-06 22:53:37','Information','Subscribed to queue {QueueName}, routing key {Topic}','Subscribed to queue \"smartlunch.backend\", routing key \"user.created\"',NULL,'{\"QueueName\":\"smartlunch.backend\",\"Topic\":\"user.created\",\"SourceContext\":\"SmartLunch.Shared.MessageQueue.Dotnet.Services.RabbitMQEventConsumer\",\"MachineName\":\"AILATUI25\",\"ThreadId\":11,\"Application\":\"SmartLunch-Backend-Service\"}'),(8,'2026-05-06 22:53:37','Information','Now listening on: {address}','Now listening on: \"http://localhost:5001\"',NULL,'{\"address\":\"http://localhost:5001\",\"EventId\":{\"Id\":14,\"Name\":\"ListeningOnAddress\"},\"SourceContext\":\"Microsoft.Hosting.Lifetime\",\"MachineName\":\"AILATUI25\",\"ThreadId\":10,\"Application\":\"SmartLunch-Backend-Service\"}'),(9,'2026-05-06 22:53:37','Information','Application started. Press Ctrl+C to shut down.','Application started. Press Ctrl+C to shut down.',NULL,'{\"SourceContext\":\"Microsoft.Hosting.Lifetime\",\"MachineName\":\"AILATUI25\",\"ThreadId\":10,\"Application\":\"SmartLunch-Backend-Service\"}'),(10,'2026-05-06 22:53:37','Information','Hosting environment: {EnvName}','Hosting environment: \"Development\"',NULL,'{\"EnvName\":\"Development\",\"SourceContext\":\"Microsoft.Hosting.Lifetime\",\"MachineName\":\"AILATUI25\",\"ThreadId\":10,\"Application\":\"SmartLunch-Backend-Service\"}'),(11,'2026-05-06 22:53:37','Information','Content root path: {ContentRoot}','Content root path: \"H:\\EngineeringThesis\\Khoa_Luan_KS_BE\\SmartLunch-Backend-Service\\SmartLunch-Backend-Service-API\"',NULL,'{\"ContentRoot\":\"H:\\\\EngineeringThesis\\\\Khoa_Luan_KS_BE\\\\SmartLunch-Backend-Service\\\\SmartLunch-Backend-Service-API\",\"SourceContext\":\"Microsoft.Hosting.Lifetime\",\"MachineName\":\"AILATUI25\",\"ThreadId\":10,\"Application\":\"SmartLunch-Backend-Service\"}'),(12,'2026-05-06 15:53:50','Warning','Failed to determine the https port for redirect.','Failed to determine the https port for redirect.',NULL,'{\"EventId\":{\"Id\":3,\"Name\":\"FailedToDeterminePort\"},\"SourceContext\":\"Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware\",\"RequestId\":\"0HNLBGUL7DVAB:00000001\",\"RequestPath\":\"/api/v1/Auth/login-admin\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":22,\"Application\":\"SmartLunch-Backend-Service\"}'),(13,'2026-05-06 15:53:51','Information','SaveChanges completed successfully. {EntityCount} entities affected.','SaveChanges completed successfully. 1 entities affected.',NULL,'{\"EntityCount\":1,\"SourceContext\":\"SmartLunch.Backend.Service.Infrastructure.Data.SmartLunchDBContext\",\"ActionId\":\"4c67b1c7-7272-4ae7-b14f-e8ae05502357\",\"ActionName\":\"SmartLunch.Backend.Service.API.Controllers.AuthController.LoginAdmin (SmartLunch.Backend.Service.API)\",\"RequestId\":\"0HNLBGUL7DVAB:00000001\",\"RequestPath\":\"/api/v1/Auth/login-admin\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":12,\"Application\":\"SmartLunch-Backend-Service\"}'),(14,'2026-05-06 15:53:51','Information','SaveChanges completed successfully. {EntityCount} entities affected.','SaveChanges completed successfully. 0 entities affected.',NULL,'{\"EntityCount\":0,\"SourceContext\":\"SmartLunch.Backend.Service.Infrastructure.Data.SmartLunchDBContext\",\"ActionId\":\"4c67b1c7-7272-4ae7-b14f-e8ae05502357\",\"ActionName\":\"SmartLunch.Backend.Service.API.Controllers.AuthController.LoginAdmin (SmartLunch.Backend.Service.API)\",\"RequestId\":\"0HNLBGUL7DVAB:00000001\",\"RequestPath\":\"/api/v1/Auth/login-admin\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":22,\"Application\":\"SmartLunch-Backend-Service\"}'),(15,'2026-05-06 15:53:51','Information','SaveChanges completed successfully. {EntityCount} entities affected.','SaveChanges completed successfully. 1 entities affected.',NULL,'{\"EntityCount\":1,\"SourceContext\":\"SmartLunch.Backend.Service.Infrastructure.Data.SmartLunchDBContext\",\"ActionId\":\"4c67b1c7-7272-4ae7-b14f-e8ae05502357\",\"ActionName\":\"SmartLunch.Backend.Service.API.Controllers.AuthController.LoginAdmin (SmartLunch.Backend.Service.API)\",\"RequestId\":\"0HNLBGUL7DVAB:00000001\",\"RequestPath\":\"/api/v1/Auth/login-admin\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":22,\"Application\":\"SmartLunch-Backend-Service\"}'),(16,'2026-05-06 15:53:51','Information','User logged in successfully: {Username}','User logged in successfully: \"admin\"',NULL,'{\"Username\":\"admin\",\"SourceContext\":\"SmartLunch.Backend.Service.Application.Handlers.Auth.LoginAdmin.LoginAdminCommandHandler\",\"ActionId\":\"4c67b1c7-7272-4ae7-b14f-e8ae05502357\",\"ActionName\":\"SmartLunch.Backend.Service.API.Controllers.AuthController.LoginAdmin (SmartLunch.Backend.Service.API)\",\"RequestId\":\"0HNLBGUL7DVAB:00000001\",\"RequestPath\":\"/api/v1/Auth/login-admin\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":22,\"Application\":\"SmartLunch-Backend-Service\"}'),(17,'2026-05-06 15:53:51','Information','HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms','HTTP \"POST\" \"/api/v1/Auth/login-admin\" responded 200 in 930.8060 ms',NULL,'{\"RequestHost\":\"localhost:5001\",\"UserAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36\",\"RequestMethod\":\"POST\",\"RequestPath\":\"/api/v1/Auth/login-admin\",\"StatusCode\":200,\"Elapsed\":930.806,\"SourceContext\":\"Serilog.AspNetCore.RequestLoggingMiddleware\",\"RequestId\":\"0HNLBGUL7DVAB:00000001\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":22,\"Application\":\"SmartLunch-Backend-Service\"}'),(18,'2026-05-06 22:54:10','Warning','User ID claim not found in token','User ID claim not found in token',NULL,'{\"SourceContext\":\"SmartLunch.Backend.Service.API.Authorization.Role.RoleRequirementHandler\",\"RequestId\":\"0HNLBGUL7DVAB:00000002\",\"RequestPath\":\"/api/v1/master-data/System/log\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":22,\"Application\":\"SmartLunch-Backend-Service\"}'),(19,'2026-05-06 22:54:10','Warning','User ID claim not found in token','User ID claim not found in token',NULL,'{\"SourceContext\":\"SmartLunch.Backend.Service.API.Authorization.Permission.PermissionRequirementHandler\",\"RequestId\":\"0HNLBGUL7DVAB:00000002\",\"RequestPath\":\"/api/v1/master-data/System/log\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":22,\"Application\":\"SmartLunch-Backend-Service\"}'),(20,'2026-05-06 22:54:10','Information','HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms','HTTP \"GET\" \"/api/v1/master-data/System/log\" responded 401 in 28.9764 ms',NULL,'{\"RequestHost\":\"localhost:5001\",\"UserAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36\",\"RequestMethod\":\"GET\",\"RequestPath\":\"/api/v1/master-data/System/log\",\"StatusCode\":401,\"Elapsed\":28.9764,\"SourceContext\":\"Serilog.AspNetCore.RequestLoggingMiddleware\",\"RequestId\":\"0HNLBGUL7DVAB:00000002\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":22,\"Application\":\"SmartLunch-Backend-Service\"}'),(21,'2026-05-06 15:54:18','Information','Retrieved {Count} roles for user: {UserId}','Retrieved 1 roles for user: 1',NULL,'{\"Count\":1,\"UserId\":1,\"SourceContext\":\"SmartLunch.Backend.Service.Application.Handlers.Auth.GetUserRolesQueryHandler\",\"RequestId\":\"0HNLBGUL7DVAB:00000003\",\"RequestPath\":\"/api/v1/master-data/System/log\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":22,\"Application\":\"SmartLunch-Backend-Service\"}'),(22,'2026-05-06 15:54:18','Information','Retrieved {Count} permissions for user: {UserId}','Retrieved 190 permissions for user: 1',NULL,'{\"Count\":190,\"UserId\":1,\"SourceContext\":\"SmartLunch.Backend.Service.Application.Handlers.Queries.Auth.GetUserPermissionsQueryHandler\",\"RequestId\":\"0HNLBGUL7DVAB:00000003\",\"RequestPath\":\"/api/v1/master-data/System/log\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":22,\"Application\":\"SmartLunch-Backend-Service\"}'),(23,'2026-05-06 15:54:18','Information','Retrieved {Count} system logs (Page {Page}, PageSize {PageSize})','Retrieved 10 system logs (Page 1, PageSize 10)',NULL,'{\"Count\":10,\"Page\":1,\"PageSize\":10,\"SourceContext\":\"SmartLunch.Backend.Service.Application.Queries.Systems.GetSystemLogs.GetSystemLogsQueryHandler\",\"ActionId\":\"e5d35a08-eca0-4e8e-9095-bd202d2333cc\",\"ActionName\":\"SmartLunch.Backend.Service.API.Controllers.MasterData.SystemController.GetSystemLogs (SmartLunch.Backend.Service.API)\",\"RequestId\":\"0HNLBGUL7DVAB:00000003\",\"RequestPath\":\"/api/v1/master-data/System/log\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":8,\"Application\":\"SmartLunch-Backend-Service\"}'),(24,'2026-05-06 15:54:18','Information','HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms','HTTP \"GET\" \"/api/v1/master-data/System/log\" responded 200 in 192.0578 ms',NULL,'{\"RequestHost\":\"localhost:5001\",\"UserAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36\",\"RequestMethod\":\"GET\",\"RequestPath\":\"/api/v1/master-data/System/log\",\"StatusCode\":200,\"Elapsed\":192.0578,\"SourceContext\":\"Serilog.AspNetCore.RequestLoggingMiddleware\",\"RequestId\":\"0HNLBGUL7DVAB:00000003\",\"ConnectionId\":\"0HNLBGUL7DVAB\",\"MachineName\":\"AILATUI25\",\"ThreadId\":8,\"Application\":\"SmartLunch-Backend-Service\"}'),(25,'2026-05-06 22:56:35','Information','Retrieved {Count} roles for user: {UserId}','Retrieved 1 roles for user: 1',NULL,'{\"Count\":1,\"UserId\":1,\"SourceContext\":\"SmartLunch.Backend.Service.Application.Handlers.Auth.GetUserRolesQueryHandler\",\"RequestId\":\"0HNLBGUL7DVAC:00000001\",\"RequestPath\":\"/api/v1/master-data/System/backup\",\"ConnectionId\":\"0HNLBGUL7DVAC\",\"MachineName\":\"AILATUI25\",\"ThreadId\":23,\"Application\":\"SmartLunch-Backend-Service\"}'),(26,'2026-05-06 22:56:35','Information','Retrieved {Count} permissions for user: {UserId}','Retrieved 190 permissions for user: 1',NULL,'{\"Count\":190,\"UserId\":1,\"SourceContext\":\"SmartLunch.Backend.Service.Application.Handlers.Queries.Auth.GetUserPermissionsQueryHandler\",\"RequestId\":\"0HNLBGUL7DVAC:00000001\",\"RequestPath\":\"/api/v1/master-data/System/backup\",\"ConnectionId\":\"0HNLBGUL7DVAC\",\"MachineName\":\"AILATUI25\",\"ThreadId\":23,\"Application\":\"SmartLunch-Backend-Service\"}'),(27,'2026-05-06 22:56:35','Warning','User {UserId} does not have required permission. User permissions: {UserPermissions}, Required: {RequiredPermissions}','User 1 does not have required permission. User permissions: \"users.create, users.read, users.update, users.delete, users.list, roles.create, roles.read, roles.update, roles.delete, roles.list, permissions.create, permissions.read, permissions.update, permissions.delete, permissions.list, user_roles.create, user_roles.read, user_roles.update, user_roles.delete, user_roles.list, user_permissions.create, user_permissions.read, user_permissions.update, user_permissions.delete, user_permissions.list, role_permissions.create, role_permissions.read, role_permissions.update, role_permissions.delete, role_permissions.list, user_tokens.create, user_tokens.read, user_tokens.update, user_tokens.delete, user_tokens.list, media_files.create, media_files.read, media_files.update, media_files.delete, media_files.list, organizations.create, organizations.read, organizations.update, organizations.delete, organizations.list, user_organizations.create, user_organizations.read, user_organizations.update, user_organizations.delete, user_organizations.list, partners.create, partners.read, partners.update, partners.delete, partners.list, contracts.create, contracts.read, contracts.update, contracts.delete, contracts.list, partner_payments.create, partner_payments.read, partner_payments.update, partner_payments.delete, partner_payments.list, ingredients.create, ingredients.read, ingredients.update, ingredients.delete, ingredients.list, ingredient_sources.create, ingredient_sources.read, ingredient_sources.update, ingredient_sources.delete, ingredient_sources.list, inventory.create, inventory.read, inventory.update, inventory.delete, inventory.list, internal_stock_issues.create, internal_stock_issues.read, internal_stock_issues.update, internal_stock_issues.delete, internal_stock_issues.list, internal_stock_issue_lines.create, internal_stock_issue_lines.read, internal_stock_issue_lines.update, internal_stock_issue_lines.delete, internal_stock_issue_lines.list, ingredient_intake_proposals.create, ingredient_intake_proposals.read, ingredient_intake_proposals.update, ingredient_intake_proposals.delete, ingredient_intake_proposals.list, ingredient_intake_proposal_lines.create, ingredient_intake_proposal_lines.read, ingredient_intake_proposal_lines.update, ingredient_intake_proposal_lines.delete, ingredient_intake_proposal_lines.list, ingredient_actual_intakes.create, ingredient_actual_intakes.read, ingredient_actual_intakes.update, ingredient_actual_intakes.delete, ingredient_actual_intakes.list, ingredient_actual_intake_lines.create, ingredient_actual_intake_lines.read, ingredient_actual_intake_lines.update, ingredient_actual_intake_lines.delete, ingredient_actual_intake_lines.list, dishes.create, dishes.read, dishes.update, dishes.delete, dishes.list, dish_ingredients.create, dish_ingredients.read, dish_ingredients.update, dish_ingredients.delete, dish_ingredients.list, weekly_menus.create, weekly_menus.read, weekly_menus.update, weekly_menus.delete, weekly_menus.list, menu_schedule.create, menu_schedule.read, menu_schedule.update, menu_schedule.delete, menu_schedule.list, orders.create, orders.read, orders.update, orders.delete, orders.list, order_items.create, order_items.read, order_items.update, order_items.delete, order_items.list, deliveries.create, deliveries.read, deliveries.update, deliveries.delete, deliveries.list, payments.create, payments.read, payments.update, payments.delete, payments.list, transactions.create, transactions.read, transactions.update, transactions.delete, transactions.list, reviews.create, reviews.read, reviews.update, reviews.delete, reviews.list, sentiments.create, sentiments.read, sentiments.update, sentiments.delete, sentiments.list, complaints.create, complaints.read, complaints.update, complaints.delete, complaints.list, chatbot_logs.create, chatbot_logs.read, chatbot_logs.update, chatbot_logs.delete, chatbot_logs.list, menu_suggestions.create, menu_suggestions.read, menu_suggestions.update, menu_suggestions.delete, menu_suggestions.list, menu_suggestions.generate, system_logs.create, system_logs.read, system_logs.backup, system_logs.restore, system_logs.list, system_backups.create, system_backups.read, system_backups.delete, system_backups.list\", Required: \"systems.backup\"',NULL,'{\"UserId\":1,\"UserPermissions\":\"users.create, users.read, users.update, users.delete, users.list, roles.create, roles.read, roles.update, roles.delete, roles.list, permissions.create, permissions.read, permissions.update, permissions.delete, permissions.list, user_roles.create, user_roles.read, user_roles.update, user_roles.delete, user_roles.list, user_permissions.create, user_permissions.read, user_permissions.update, user_permissions.delete, user_permissions.list, role_permissions.create, role_permissions.read, role_permissions.update, role_permissions.delete, role_permissions.list, user_tokens.create, user_tokens.read, user_tokens.update, user_tokens.delete, user_tokens.list, media_files.create, media_files.read, media_files.update, media_files.delete, media_files.list, organizations.create, organizations.read, organizations.update, organizations.delete, organizations.list, user_organizations.create, user_organizations.read, user_organizations.update, user_organizations.delete, user_organizations.list, partners.create, partners.read, partners.update, partners.delete, partners.list, contracts.create, contracts.read, contracts.update, contracts.delete, contracts.list, partner_payments.create, partner_payments.read, partner_payments.update, partner_payments.delete, partner_payments.list, ingredients.create, ingredients.read, ingredients.update, ingredients.delete, ingredients.list, ingredient_sources.create, ingredient_sources.read, ingredient_sources.update, ingredient_sources.delete, ingredient_sources.list, inventory.create, inventory.read, inventory.update, inventory.delete, inventory.list, internal_stock_issues.create, internal_stock_issues.read, internal_stock_issues.update, internal_stock_issues.delete, internal_stock_issues.list, internal_stock_issue_lines.create, internal_stock_issue_lines.read, internal_stock_issue_lines.update, internal_stock_issue_lines.delete, internal_stock_issue_lines.list, ingredient_intake_proposals.create, ingredient_intake_proposals.read, ingredient_intake_proposals.update, ingredient_intake_proposals.delete, ingredient_intake_proposals.list, ingredient_intake_proposal_lines.create, ingredient_intake_proposal_lines.read, ingredient_intake_proposal_lines.update, ingredient_intake_proposal_lines.delete, ingredient_intake_proposal_lines.list, ingredient_actual_intakes.create, ingredient_actual_intakes.read, ingredient_actual_intakes.update, ingredient_actual_intakes.delete, ingredient_actual_intakes.list, ingredient_actual_intake_lines.create, ingredient_actual_intake_lines.read, ingredient_actual_intake_lines.update, ingredient_actual_intake_lines.delete, ingredient_actual_intake_lines.list, dishes.create, dishes.read, dishes.update, dishes.delete, dishes.list, dish_ingredients.create, dish_ingredients.read, dish_ingredients.update, dish_ingredients.delete, dish_ingredients.list, weekly_menus.create, weekly_menus.read, weekly_menus.update, weekly_menus.delete, weekly_menus.list, menu_schedule.create, menu_schedule.read, menu_schedule.update, menu_schedule.delete, menu_schedule.list, orders.create, orders.read, orders.update, orders.delete, orders.list, order_items.create, order_items.read, order_items.update, order_items.delete, order_items.list, deliveries.create, deliveries.read, deliveries.update, deliveries.delete, deliveries.list, payments.create, payments.read, payments.update, payments.delete, payments.list, transactions.create, transactions.read, transactions.update, transactions.delete, transactions.list, reviews.create, reviews.read, reviews.update, reviews.delete, reviews.list, sentiments.create, sentiments.read, sentiments.update, sentiments.delete, sentiments.list, complaints.create, complaints.read, complaints.update, complaints.delete, complaints.list, chatbot_logs.create, chatbot_logs.read, chatbot_logs.update, chatbot_logs.delete, chatbot_logs.list, menu_suggestions.create, menu_suggestions.read, menu_suggestions.update, menu_suggestions.delete, menu_suggestions.list, menu_suggestions.generate, system_logs.create, system_logs.read, system_logs.backup, system_logs.restore, system_logs.list, system_backups.create, system_backups.read, system_backups.delete, system_backups.list\",\"RequiredPermissions\":\"systems.backup\",\"SourceContext\":\"SmartLunch.Backend.Service.API.Authorization.Permission.PermissionRequirementHandler\",\"RequestId\":\"0HNLBGUL7DVAC:00000001\",\"RequestPath\":\"/api/v1/master-data/System/backup\",\"ConnectionId\":\"0HNLBGUL7DVAC\",\"MachineName\":\"AILATUI25\",\"ThreadId\":23,\"Application\":\"SmartLunch-Backend-Service\"}'),(28,'2026-05-06 22:56:35','Information','HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms','HTTP \"POST\" \"/api/v1/master-data/System/backup\" responded 403 in 39.9617 ms',NULL,'{\"RequestHost\":\"localhost:5001\",\"UserAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36\",\"RequestMethod\":\"POST\",\"RequestPath\":\"/api/v1/master-data/System/backup\",\"StatusCode\":403,\"Elapsed\":39.9617,\"SourceContext\":\"Serilog.AspNetCore.RequestLoggingMiddleware\",\"RequestId\":\"0HNLBGUL7DVAC:00000001\",\"ConnectionId\":\"0HNLBGUL7DVAC\",\"MachineName\":\"AILATUI25\",\"ThreadId\":23,\"Application\":\"SmartLunch-Backend-Service\"}'),(29,'2026-05-06 15:57:02','Information','Application is shutting down...','Application is shutting down...',NULL,'{\"SourceContext\":\"Microsoft.Hosting.Lifetime\",\"MachineName\":\"AILATUI25\",\"ThreadId\":7,\"Application\":\"SmartLunch-Backend-Service\"}'),(30,'2026-05-06 15:57:02','Information','RabbitMQ Consumer Background Service is stopping.','RabbitMQ Consumer Background Service is stopping.',NULL,'{\"SourceContext\":\"SmartLunch.Backend.Service.API.Services.RabbitMQConsumerBackgroundService\",\"MachineName\":\"AILATUI25\",\"ThreadId\":27,\"Application\":\"SmartLunch-Backend-Service\"}'),(31,'2026-05-06 15:57:02','Information','Consumer closed.','Consumer closed.',NULL,'{\"SourceContext\":\"SmartLunch.Shared.MessageQueue.Dotnet.Services.RabbitMQEventConsumer\",\"MachineName\":\"AILATUI25\",\"ThreadId\":20,\"Application\":\"SmartLunch-Backend-Service\"}'),(32,'2026-05-06 22:57:09','Information','SmartLunch Backend Service starting','SmartLunch Backend Service starting',NULL,'{\"MachineName\":\"AILATUI25\",\"ThreadId\":10,\"Application\":\"SmartLunch-Backend-Service\"}'),(33,'2026-05-06 22:57:09','Information','RabbitMQ Consumer Background Service is starting.','RabbitMQ Consumer Background Service is starting.',NULL,'{\"SourceContext\":\"SmartLunch.Backend.Service.API.Services.RabbitMQConsumerBackgroundService\",\"MachineName\":\"AILATUI25\",\"ThreadId\":11,\"Application\":\"SmartLunch-Backend-Service\"}'),(34,'2026-05-06 22:57:09','Information','DatabaseBackupHostedService disabled.','DatabaseBackupHostedService disabled.',NULL,'{\"SourceContext\":\"SmartLunch.Backend.Service.API.Services.DatabaseBackupHostedService\",\"MachineName\":\"AILATUI25\",\"ThreadId\":8,\"Application\":\"SmartLunch-Backend-Service\"}'),(35,'2026-05-06 22:57:09','Information','Subscribed to queue {QueueName}, routing key {Topic}','Subscribed to queue \"smartlunch.backend\", routing key \"user.created\"',NULL,'{\"QueueName\":\"smartlunch.backend\",\"Topic\":\"user.created\",\"SourceContext\":\"SmartLunch.Shared.MessageQueue.Dotnet.Services.RabbitMQEventConsumer\",\"MachineName\":\"AILATUI25\",\"ThreadId\":11,\"Application\":\"SmartLunch-Backend-Service\"}'),(36,'2026-05-06 22:57:09','Information','Now listening on: {address}','Now listening on: \"http://localhost:5001\"',NULL,'{\"address\":\"http://localhost:5001\",\"EventId\":{\"Id\":14,\"Name\":\"ListeningOnAddress\"},\"SourceContext\":\"Microsoft.Hosting.Lifetime\",\"MachineName\":\"AILATUI25\",\"ThreadId\":10,\"Application\":\"SmartLunch-Backend-Service\"}'),(37,'2026-05-06 22:57:09','Information','Application started. Press Ctrl+C to shut down.','Application started. Press Ctrl+C to shut down.',NULL,'{\"SourceContext\":\"Microsoft.Hosting.Lifetime\",\"MachineName\":\"AILATUI25\",\"ThreadId\":10,\"Application\":\"SmartLunch-Backend-Service\"}'),(38,'2026-05-06 22:57:09','Information','Hosting environment: {EnvName}','Hosting environment: \"Development\"',NULL,'{\"EnvName\":\"Development\",\"SourceContext\":\"Microsoft.Hosting.Lifetime\",\"MachineName\":\"AILATUI25\",\"ThreadId\":10,\"Application\":\"SmartLunch-Backend-Service\"}'),(39,'2026-05-06 22:57:09','Information','Content root path: {ContentRoot}','Content root path: \"H:\\EngineeringThesis\\Khoa_Luan_KS_BE\\SmartLunch-Backend-Service\\SmartLunch-Backend-Service-API\"',NULL,'{\"ContentRoot\":\"H:\\\\EngineeringThesis\\\\Khoa_Luan_KS_BE\\\\SmartLunch-Backend-Service\\\\SmartLunch-Backend-Service-API\",\"SourceContext\":\"Microsoft.Hosting.Lifetime\",\"MachineName\":\"AILATUI25\",\"ThreadId\":10,\"Application\":\"SmartLunch-Backend-Service\"}');
/*!40000 ALTER TABLE `system_logs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `transactions`
--

DROP TABLE IF EXISTS `transactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `transactions` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `Date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Description` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Amount` decimal(12,2) NOT NULL,
  `Category` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Method` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ReferenceId` int DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_transactions_code` (`Code`),
  KEY `IX_transactions_date` (`Date` DESC),
  KEY `IX_transactions_category` (`Category`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Thu chi tài chính';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `transactions`
--

LOCK TABLES `transactions` WRITE;
/*!40000 ALTER TABLE `transactions` DISABLE KEYS */;
INSERT INTO `transactions` VALUES (1,'TXN00000001','2026-05-04 15:53:21','Thu tiền đơn INV-20250321-001 ABC Tech',4500000.00,'income_order','bank_transfer',NULL,'2026-05-06 15:53:21'),(2,'TXN00000002','2026-05-05 15:53:21','Thu tiền đơn khách lẻ',62000.00,'income_order','e_wallet',NULL,'2026-05-06 15:53:21'),(3,'TXN00000003','2026-05-01 15:53:21','Thanh toán NCC Thực Phẩm Sạch Việt',-45000000.00,'expense_supplier','bank_transfer',NULL,'2026-05-06 15:53:21'),(4,'TXN00000004','2026-04-29 15:53:21','Chi phí gas bếp tháng 3',-2500000.00,'expense_utility','cash',NULL,'2026-05-06 15:53:21'),(5,'TXN00000005','2026-04-29 15:53:21','Chi phí điện nước tháng 3',-4200000.00,'expense_utility','bank_transfer',NULL,'2026-05-06 15:53:21');
/*!40000 ALTER TABLE `transactions` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_transactions_code` BEFORE INSERT ON `transactions` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('transactions'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `user_organizations`
--

DROP TABLE IF EXISTS `user_organizations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_organizations` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `UserId` int NOT NULL,
  `OrganizationId` int NOT NULL,
  `Department` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Bộ phận/phòng ban trong đơn vị (dùng cho thống kê)',
  `JoinedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_user_organizations_user_org` (`UserId`,`OrganizationId`),
  UNIQUE KEY `UK_user_organizations_code` (`Code`),
  KEY `IX_user_organizations_org` (`OrganizationId`),
  CONSTRAINT `FK_user_organizations_org` FOREIGN KEY (`OrganizationId`) REFERENCES `organizations` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_user_organizations_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Thành viên đơn vị';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_organizations`
--

LOCK TABLES `user_organizations` WRITE;
/*!40000 ALTER TABLE `user_organizations` DISABLE KEYS */;
/*!40000 ALTER TABLE `user_organizations` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_user_organizations_code` BEFORE INSERT ON `user_organizations` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('user_organizations'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `user_permissions`
--

DROP TABLE IF EXISTS `user_permissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_permissions` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `UserId` int NOT NULL,
  `PermissionId` int NOT NULL,
  `AssignedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `AssignedBy` int DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_user_permissions_user_perm` (`UserId`,`PermissionId`),
  UNIQUE KEY `UK_user_permissions_code` (`Code`),
  KEY `IX_user_permissions_perm` (`PermissionId`),
  KEY `FK_user_permissions_assigned_by` (`AssignedBy`),
  CONSTRAINT `FK_user_permissions_assigned_by` FOREIGN KEY (`AssignedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_user_permissions_perm` FOREIGN KEY (`PermissionId`) REFERENCES `permissions` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_user_permissions_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=506 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Gán quyền trực tiếp cho người dùng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_permissions`
--

LOCK TABLES `user_permissions` WRITE;
/*!40000 ALTER TABLE `user_permissions` DISABLE KEYS */;
INSERT INTO `user_permissions` VALUES (1,'UPE00000001',1,1,'2026-05-06 15:53:20',1,1),(2,'UPE00000002',1,2,'2026-05-06 15:53:20',1,1),(3,'UPE00000003',1,3,'2026-05-06 15:53:20',1,1),(4,'UPE00000004',1,4,'2026-05-06 15:53:20',1,1),(5,'UPE00000005',1,5,'2026-05-06 15:53:20',1,1),(6,'UPE00000006',1,6,'2026-05-06 15:53:20',1,1),(7,'UPE00000007',1,7,'2026-05-06 15:53:20',1,1),(8,'UPE00000008',1,8,'2026-05-06 15:53:20',1,1),(9,'UPE00000009',1,9,'2026-05-06 15:53:20',1,1),(10,'UPE00000010',1,10,'2026-05-06 15:53:20',1,1),(11,'UPE00000011',1,11,'2026-05-06 15:53:20',1,1),(12,'UPE00000012',1,12,'2026-05-06 15:53:20',1,1),(13,'UPE00000013',1,13,'2026-05-06 15:53:20',1,1),(14,'UPE00000014',1,14,'2026-05-06 15:53:20',1,1),(15,'UPE00000015',1,15,'2026-05-06 15:53:20',1,1),(16,'UPE00000016',1,16,'2026-05-06 15:53:20',1,1),(17,'UPE00000017',1,17,'2026-05-06 15:53:20',1,1),(18,'UPE00000018',1,18,'2026-05-06 15:53:20',1,1),(19,'UPE00000019',1,19,'2026-05-06 15:53:20',1,1),(20,'UPE00000020',1,20,'2026-05-06 15:53:20',1,1),(21,'UPE00000021',1,21,'2026-05-06 15:53:20',1,1),(22,'UPE00000022',1,22,'2026-05-06 15:53:20',1,1),(23,'UPE00000023',1,23,'2026-05-06 15:53:20',1,1),(24,'UPE00000024',1,24,'2026-05-06 15:53:20',1,1),(25,'UPE00000025',1,25,'2026-05-06 15:53:20',1,1),(26,'UPE00000026',1,26,'2026-05-06 15:53:20',1,1),(27,'UPE00000027',1,27,'2026-05-06 15:53:20',1,1),(28,'UPE00000028',1,28,'2026-05-06 15:53:20',1,1),(29,'UPE00000029',1,29,'2026-05-06 15:53:20',1,1),(30,'UPE00000030',1,30,'2026-05-06 15:53:20',1,1),(31,'UPE00000031',1,31,'2026-05-06 15:53:20',1,1),(32,'UPE00000032',1,32,'2026-05-06 15:53:20',1,1),(33,'UPE00000033',1,33,'2026-05-06 15:53:20',1,1),(34,'UPE00000034',1,34,'2026-05-06 15:53:20',1,1),(35,'UPE00000035',1,35,'2026-05-06 15:53:20',1,1),(36,'UPE00000036',1,36,'2026-05-06 15:53:20',1,1),(37,'UPE00000037',1,37,'2026-05-06 15:53:20',1,1),(38,'UPE00000038',1,38,'2026-05-06 15:53:20',1,1),(39,'UPE00000039',1,39,'2026-05-06 15:53:20',1,1),(40,'UPE00000040',1,40,'2026-05-06 15:53:20',1,1),(41,'UPE00000041',1,41,'2026-05-06 15:53:20',1,1),(42,'UPE00000042',1,42,'2026-05-06 15:53:20',1,1),(43,'UPE00000043',1,43,'2026-05-06 15:53:20',1,1),(44,'UPE00000044',1,44,'2026-05-06 15:53:20',1,1),(45,'UPE00000045',1,45,'2026-05-06 15:53:20',1,1),(46,'UPE00000046',1,46,'2026-05-06 15:53:20',1,1),(47,'UPE00000047',1,47,'2026-05-06 15:53:20',1,1),(48,'UPE00000048',1,48,'2026-05-06 15:53:20',1,1),(49,'UPE00000049',1,49,'2026-05-06 15:53:20',1,1),(50,'UPE00000050',1,50,'2026-05-06 15:53:20',1,1),(51,'UPE00000051',1,51,'2026-05-06 15:53:20',1,1),(52,'UPE00000052',1,52,'2026-05-06 15:53:20',1,1),(53,'UPE00000053',1,53,'2026-05-06 15:53:20',1,1),(54,'UPE00000054',1,54,'2026-05-06 15:53:20',1,1),(55,'UPE00000055',1,55,'2026-05-06 15:53:20',1,1),(56,'UPE00000056',1,56,'2026-05-06 15:53:20',1,1),(57,'UPE00000057',1,57,'2026-05-06 15:53:20',1,1),(58,'UPE00000058',1,58,'2026-05-06 15:53:20',1,1),(59,'UPE00000059',1,59,'2026-05-06 15:53:20',1,1),(60,'UPE00000060',1,60,'2026-05-06 15:53:20',1,1),(61,'UPE00000061',1,61,'2026-05-06 15:53:20',1,1),(62,'UPE00000062',1,62,'2026-05-06 15:53:20',1,1),(63,'UPE00000063',1,63,'2026-05-06 15:53:20',1,1),(64,'UPE00000064',1,64,'2026-05-06 15:53:20',1,1),(65,'UPE00000065',1,65,'2026-05-06 15:53:20',1,1),(66,'UPE00000066',1,66,'2026-05-06 15:53:20',1,1),(67,'UPE00000067',1,67,'2026-05-06 15:53:20',1,1),(68,'UPE00000068',1,68,'2026-05-06 15:53:20',1,1),(69,'UPE00000069',1,69,'2026-05-06 15:53:20',1,1),(70,'UPE00000070',1,70,'2026-05-06 15:53:20',1,1),(71,'UPE00000071',1,71,'2026-05-06 15:53:20',1,1),(72,'UPE00000072',1,72,'2026-05-06 15:53:20',1,1),(73,'UPE00000073',1,73,'2026-05-06 15:53:20',1,1),(74,'UPE00000074',1,74,'2026-05-06 15:53:20',1,1),(75,'UPE00000075',1,75,'2026-05-06 15:53:20',1,1),(76,'UPE00000076',1,76,'2026-05-06 15:53:20',1,1),(77,'UPE00000077',1,77,'2026-05-06 15:53:20',1,1),(78,'UPE00000078',1,78,'2026-05-06 15:53:20',1,1),(79,'UPE00000079',1,79,'2026-05-06 15:53:20',1,1),(80,'UPE00000080',1,80,'2026-05-06 15:53:20',1,1),(81,'UPE00000081',1,81,'2026-05-06 15:53:20',1,1),(82,'UPE00000082',1,82,'2026-05-06 15:53:20',1,1),(83,'UPE00000083',1,83,'2026-05-06 15:53:20',1,1),(84,'UPE00000084',1,84,'2026-05-06 15:53:20',1,1),(85,'UPE00000085',1,85,'2026-05-06 15:53:20',1,1),(86,'UPE00000086',1,86,'2026-05-06 15:53:20',1,1),(87,'UPE00000087',1,87,'2026-05-06 15:53:20',1,1),(88,'UPE00000088',1,88,'2026-05-06 15:53:20',1,1),(89,'UPE00000089',1,89,'2026-05-06 15:53:20',1,1),(90,'UPE00000090',1,90,'2026-05-06 15:53:20',1,1),(91,'UPE00000091',1,91,'2026-05-06 15:53:20',1,1),(92,'UPE00000092',1,92,'2026-05-06 15:53:20',1,1),(93,'UPE00000093',1,93,'2026-05-06 15:53:20',1,1),(94,'UPE00000094',1,94,'2026-05-06 15:53:20',1,1),(95,'UPE00000095',1,95,'2026-05-06 15:53:20',1,1),(96,'UPE00000096',1,96,'2026-05-06 15:53:20',1,1),(97,'UPE00000097',1,97,'2026-05-06 15:53:20',1,1),(98,'UPE00000098',1,98,'2026-05-06 15:53:20',1,1),(99,'UPE00000099',1,99,'2026-05-06 15:53:20',1,1),(100,'UPE00000100',1,100,'2026-05-06 15:53:20',1,1),(101,'UPE00000101',1,101,'2026-05-06 15:53:20',1,1),(102,'UPE00000102',1,102,'2026-05-06 15:53:20',1,1),(103,'UPE00000103',1,103,'2026-05-06 15:53:20',1,1),(104,'UPE00000104',1,104,'2026-05-06 15:53:20',1,1),(105,'UPE00000105',1,105,'2026-05-06 15:53:20',1,1),(106,'UPE00000106',1,106,'2026-05-06 15:53:20',1,1),(107,'UPE00000107',1,107,'2026-05-06 15:53:20',1,1),(108,'UPE00000108',1,108,'2026-05-06 15:53:20',1,1),(109,'UPE00000109',1,109,'2026-05-06 15:53:20',1,1),(110,'UPE00000110',1,110,'2026-05-06 15:53:20',1,1),(111,'UPE00000111',1,111,'2026-05-06 15:53:20',1,1),(112,'UPE00000112',1,112,'2026-05-06 15:53:20',1,1),(113,'UPE00000113',1,113,'2026-05-06 15:53:20',1,1),(114,'UPE00000114',1,114,'2026-05-06 15:53:20',1,1),(115,'UPE00000115',1,115,'2026-05-06 15:53:20',1,1),(116,'UPE00000116',1,116,'2026-05-06 15:53:20',1,1),(117,'UPE00000117',1,117,'2026-05-06 15:53:20',1,1),(118,'UPE00000118',1,118,'2026-05-06 15:53:20',1,1),(119,'UPE00000119',1,119,'2026-05-06 15:53:20',1,1),(120,'UPE00000120',1,120,'2026-05-06 15:53:20',1,1),(121,'UPE00000121',1,121,'2026-05-06 15:53:20',1,1),(122,'UPE00000122',1,122,'2026-05-06 15:53:20',1,1),(123,'UPE00000123',1,123,'2026-05-06 15:53:20',1,1),(124,'UPE00000124',1,124,'2026-05-06 15:53:20',1,1),(125,'UPE00000125',1,125,'2026-05-06 15:53:20',1,1),(126,'UPE00000126',1,126,'2026-05-06 15:53:20',1,1),(127,'UPE00000127',1,127,'2026-05-06 15:53:20',1,1),(128,'UPE00000128',1,128,'2026-05-06 15:53:20',1,1),(129,'UPE00000129',1,129,'2026-05-06 15:53:20',1,1),(130,'UPE00000130',1,130,'2026-05-06 15:53:20',1,1),(131,'UPE00000131',1,131,'2026-05-06 15:53:20',1,1),(132,'UPE00000132',1,132,'2026-05-06 15:53:20',1,1),(133,'UPE00000133',1,133,'2026-05-06 15:53:20',1,1),(134,'UPE00000134',1,134,'2026-05-06 15:53:20',1,1),(135,'UPE00000135',1,135,'2026-05-06 15:53:20',1,1),(136,'UPE00000136',1,136,'2026-05-06 15:53:20',1,1),(137,'UPE00000137',1,137,'2026-05-06 15:53:20',1,1),(138,'UPE00000138',1,138,'2026-05-06 15:53:20',1,1),(139,'UPE00000139',1,139,'2026-05-06 15:53:20',1,1),(140,'UPE00000140',1,140,'2026-05-06 15:53:20',1,1),(141,'UPE00000141',1,141,'2026-05-06 15:53:20',1,1),(142,'UPE00000142',1,142,'2026-05-06 15:53:20',1,1),(143,'UPE00000143',1,143,'2026-05-06 15:53:20',1,1),(144,'UPE00000144',1,144,'2026-05-06 15:53:20',1,1),(145,'UPE00000145',1,145,'2026-05-06 15:53:20',1,1),(146,'UPE00000146',1,146,'2026-05-06 15:53:20',1,1),(147,'UPE00000147',1,147,'2026-05-06 15:53:20',1,1),(148,'UPE00000148',1,148,'2026-05-06 15:53:20',1,1),(149,'UPE00000149',1,149,'2026-05-06 15:53:20',1,1),(150,'UPE00000150',1,150,'2026-05-06 15:53:20',1,1),(151,'UPE00000151',1,151,'2026-05-06 15:53:20',1,1),(152,'UPE00000152',1,152,'2026-05-06 15:53:20',1,1),(153,'UPE00000153',1,153,'2026-05-06 15:53:20',1,1),(154,'UPE00000154',1,154,'2026-05-06 15:53:20',1,1),(155,'UPE00000155',1,155,'2026-05-06 15:53:20',1,1),(156,'UPE00000156',1,156,'2026-05-06 15:53:20',1,1),(157,'UPE00000157',1,157,'2026-05-06 15:53:20',1,1),(158,'UPE00000158',1,158,'2026-05-06 15:53:20',1,1),(159,'UPE00000159',1,159,'2026-05-06 15:53:20',1,1),(160,'UPE00000160',1,160,'2026-05-06 15:53:20',1,1),(161,'UPE00000161',1,161,'2026-05-06 15:53:20',1,1),(162,'UPE00000162',1,162,'2026-05-06 15:53:20',1,1),(163,'UPE00000163',1,163,'2026-05-06 15:53:20',1,1),(164,'UPE00000164',1,164,'2026-05-06 15:53:20',1,1),(165,'UPE00000165',1,165,'2026-05-06 15:53:20',1,1),(166,'UPE00000166',1,166,'2026-05-06 15:53:20',1,1),(167,'UPE00000167',1,167,'2026-05-06 15:53:20',1,1),(168,'UPE00000168',1,168,'2026-05-06 15:53:20',1,1),(169,'UPE00000169',1,169,'2026-05-06 15:53:20',1,1),(170,'UPE00000170',1,170,'2026-05-06 15:53:20',1,1),(171,'UPE00000171',1,171,'2026-05-06 15:53:20',1,1),(172,'UPE00000172',1,172,'2026-05-06 15:53:20',1,1),(173,'UPE00000173',1,173,'2026-05-06 15:53:20',1,1),(174,'UPE00000174',1,174,'2026-05-06 15:53:20',1,1),(175,'UPE00000175',1,175,'2026-05-06 15:53:20',1,1),(176,'UPE00000176',1,176,'2026-05-06 15:53:20',1,1),(177,'UPE00000177',1,177,'2026-05-06 15:53:20',1,1),(178,'UPE00000178',1,178,'2026-05-06 15:53:20',1,1),(179,'UPE00000179',1,179,'2026-05-06 15:53:20',1,1),(180,'UPE00000180',1,180,'2026-05-06 15:53:20',1,1),(181,'UPE00000181',1,181,'2026-05-06 15:53:20',1,1),(182,'UPE00000182',1,182,'2026-05-06 15:53:20',1,1),(183,'UPE00000183',1,183,'2026-05-06 15:53:20',1,1),(184,'UPE00000184',1,184,'2026-05-06 15:53:20',1,1),(185,'UPE00000185',1,185,'2026-05-06 15:53:20',1,1),(186,'UPE00000186',1,186,'2026-05-06 15:53:20',1,1),(187,'UPE00000187',1,187,'2026-05-06 15:53:20',1,1),(188,'UPE00000188',1,188,'2026-05-06 15:53:20',1,1),(189,'UPE00000189',1,189,'2026-05-06 15:53:20',1,1),(190,'UPE00000190',1,190,'2026-05-06 15:53:20',1,1),(256,'UPE00000191',2,1,'2026-05-06 15:53:20',1,1),(257,'UPE00000192',2,2,'2026-05-06 15:53:20',1,1),(258,'UPE00000193',2,3,'2026-05-06 15:53:20',1,1),(259,'UPE00000194',2,4,'2026-05-06 15:53:20',1,1),(260,'UPE00000195',2,5,'2026-05-06 15:53:20',1,1),(261,'UPE00000196',2,41,'2026-05-06 15:53:20',1,1),(262,'UPE00000197',2,42,'2026-05-06 15:53:20',1,1),(263,'UPE00000198',2,43,'2026-05-06 15:53:20',1,1),(264,'UPE00000199',2,44,'2026-05-06 15:53:20',1,1),(265,'UPE00000200',2,45,'2026-05-06 15:53:20',1,1),(266,'UPE00000201',2,46,'2026-05-06 15:53:20',1,1),(267,'UPE00000202',2,47,'2026-05-06 15:53:20',1,1),(268,'UPE00000203',2,48,'2026-05-06 15:53:20',1,1),(269,'UPE00000204',2,49,'2026-05-06 15:53:20',1,1),(270,'UPE00000205',2,50,'2026-05-06 15:53:20',1,1),(271,'UPE00000206',2,51,'2026-05-06 15:53:20',1,1),(272,'UPE00000207',2,52,'2026-05-06 15:53:20',1,1),(273,'UPE00000208',2,53,'2026-05-06 15:53:20',1,1),(274,'UPE00000209',2,54,'2026-05-06 15:53:20',1,1),(275,'UPE00000210',2,55,'2026-05-06 15:53:20',1,1),(276,'UPE00000211',2,56,'2026-05-06 15:53:20',1,1),(277,'UPE00000212',2,57,'2026-05-06 15:53:20',1,1),(278,'UPE00000213',2,58,'2026-05-06 15:53:20',1,1),(279,'UPE00000214',2,59,'2026-05-06 15:53:20',1,1),(280,'UPE00000215',2,60,'2026-05-06 15:53:20',1,1),(281,'UPE00000216',2,61,'2026-05-06 15:53:20',1,1),(282,'UPE00000217',2,62,'2026-05-06 15:53:20',1,1),(283,'UPE00000218',2,63,'2026-05-06 15:53:20',1,1),(284,'UPE00000219',2,64,'2026-05-06 15:53:20',1,1),(285,'UPE00000220',2,65,'2026-05-06 15:53:20',1,1),(286,'UPE00000221',2,66,'2026-05-06 15:53:20',1,1),(287,'UPE00000222',2,67,'2026-05-06 15:53:20',1,1),(288,'UPE00000223',2,68,'2026-05-06 15:53:20',1,1),(289,'UPE00000224',2,69,'2026-05-06 15:53:20',1,1),(290,'UPE00000225',2,70,'2026-05-06 15:53:20',1,1),(291,'UPE00000226',2,76,'2026-05-06 15:53:20',1,1),(292,'UPE00000227',2,77,'2026-05-06 15:53:20',1,1),(293,'UPE00000228',2,78,'2026-05-06 15:53:20',1,1),(294,'UPE00000229',2,79,'2026-05-06 15:53:20',1,1),(295,'UPE00000230',2,80,'2026-05-06 15:53:20',1,1),(296,'UPE00000231',2,111,'2026-05-06 15:53:20',1,1),(297,'UPE00000232',2,112,'2026-05-06 15:53:20',1,1),(298,'UPE00000233',2,113,'2026-05-06 15:53:20',1,1),(299,'UPE00000234',2,114,'2026-05-06 15:53:20',1,1),(300,'UPE00000235',2,115,'2026-05-06 15:53:20',1,1),(301,'UPE00000236',2,121,'2026-05-06 15:53:20',1,1),(302,'UPE00000237',2,122,'2026-05-06 15:53:20',1,1),(303,'UPE00000238',2,123,'2026-05-06 15:53:20',1,1),(304,'UPE00000239',2,124,'2026-05-06 15:53:20',1,1),(305,'UPE00000240',2,125,'2026-05-06 15:53:20',1,1),(306,'UPE00000241',2,126,'2026-05-06 15:53:20',1,1),(307,'UPE00000242',2,127,'2026-05-06 15:53:20',1,1),(308,'UPE00000243',2,128,'2026-05-06 15:53:20',1,1),(309,'UPE00000244',2,129,'2026-05-06 15:53:20',1,1),(310,'UPE00000245',2,130,'2026-05-06 15:53:20',1,1),(311,'UPE00000246',2,131,'2026-05-06 15:53:20',1,1),(312,'UPE00000247',2,132,'2026-05-06 15:53:20',1,1),(313,'UPE00000248',2,133,'2026-05-06 15:53:20',1,1),(314,'UPE00000249',2,134,'2026-05-06 15:53:20',1,1),(315,'UPE00000250',2,135,'2026-05-06 15:53:20',1,1),(316,'UPE00000251',2,136,'2026-05-06 15:53:20',1,1),(317,'UPE00000252',2,137,'2026-05-06 15:53:20',1,1),(318,'UPE00000253',2,138,'2026-05-06 15:53:20',1,1),(319,'UPE00000254',2,139,'2026-05-06 15:53:20',1,1),(320,'UPE00000255',2,140,'2026-05-06 15:53:20',1,1),(321,'UPE00000256',2,141,'2026-05-06 15:53:20',1,1),(322,'UPE00000257',2,142,'2026-05-06 15:53:20',1,1),(323,'UPE00000258',2,143,'2026-05-06 15:53:20',1,1),(324,'UPE00000259',2,144,'2026-05-06 15:53:20',1,1),(325,'UPE00000260',2,145,'2026-05-06 15:53:20',1,1),(326,'UPE00000261',2,146,'2026-05-06 15:53:20',1,1),(327,'UPE00000262',2,147,'2026-05-06 15:53:20',1,1),(328,'UPE00000263',2,148,'2026-05-06 15:53:20',1,1),(329,'UPE00000264',2,149,'2026-05-06 15:53:20',1,1),(330,'UPE00000265',2,150,'2026-05-06 15:53:20',1,1),(331,'UPE00000266',2,151,'2026-05-06 15:53:20',1,1),(332,'UPE00000267',2,152,'2026-05-06 15:53:20',1,1),(333,'UPE00000268',2,153,'2026-05-06 15:53:20',1,1),(334,'UPE00000269',2,154,'2026-05-06 15:53:20',1,1),(335,'UPE00000270',2,155,'2026-05-06 15:53:20',1,1),(336,'UPE00000271',2,156,'2026-05-06 15:53:20',1,1),(337,'UPE00000272',2,157,'2026-05-06 15:53:20',1,1),(338,'UPE00000273',2,158,'2026-05-06 15:53:20',1,1),(339,'UPE00000274',2,159,'2026-05-06 15:53:20',1,1),(340,'UPE00000275',2,160,'2026-05-06 15:53:20',1,1),(341,'UPE00000276',2,161,'2026-05-06 15:53:20',1,1),(342,'UPE00000277',2,162,'2026-05-06 15:53:20',1,1),(343,'UPE00000278',2,163,'2026-05-06 15:53:20',1,1),(344,'UPE00000279',2,164,'2026-05-06 15:53:20',1,1),(345,'UPE00000280',2,165,'2026-05-06 15:53:20',1,1),(346,'UPE00000281',2,166,'2026-05-06 15:53:20',1,1),(347,'UPE00000282',2,167,'2026-05-06 15:53:20',1,1),(348,'UPE00000283',2,168,'2026-05-06 15:53:20',1,1),(349,'UPE00000284',2,169,'2026-05-06 15:53:20',1,1),(350,'UPE00000285',2,170,'2026-05-06 15:53:20',1,1),(351,'UPE00000286',2,176,'2026-05-06 15:53:20',1,1),(352,'UPE00000287',2,177,'2026-05-06 15:53:20',1,1),(353,'UPE00000288',2,178,'2026-05-06 15:53:20',1,1),(354,'UPE00000289',2,179,'2026-05-06 15:53:20',1,1),(355,'UPE00000290',2,180,'2026-05-06 15:53:20',1,1),(356,'UPE00000291',2,181,'2026-05-06 15:53:20',1,1),(383,'UPE00000292',8,132,'2026-05-06 15:53:20',1,1),(384,'UPE00000293',8,133,'2026-05-06 15:53:20',1,1),(385,'UPE00000294',8,135,'2026-05-06 15:53:20',1,1),(386,'UPE00000295',8,141,'2026-05-06 15:53:20',1,1),(387,'UPE00000296',8,142,'2026-05-06 15:53:20',1,1),(388,'UPE00000297',8,143,'2026-05-06 15:53:20',1,1),(389,'UPE00000298',8,144,'2026-05-06 15:53:20',1,1),(390,'UPE00000299',8,145,'2026-05-06 15:53:20',1,1),(398,'UPE00000300',9,132,'2026-05-06 15:53:20',1,1),(399,'UPE00000301',9,133,'2026-05-06 15:53:20',1,1),(400,'UPE00000302',9,135,'2026-05-06 15:53:20',1,1),(401,'UPE00000303',9,141,'2026-05-06 15:53:20',1,1),(402,'UPE00000304',9,142,'2026-05-06 15:53:20',1,1),(403,'UPE00000305',9,143,'2026-05-06 15:53:20',1,1),(404,'UPE00000306',9,144,'2026-05-06 15:53:20',1,1),(405,'UPE00000307',9,145,'2026-05-06 15:53:20',1,1),(413,'UPE00000308',10,112,'2026-05-06 15:53:20',1,1),(414,'UPE00000309',10,115,'2026-05-06 15:53:20',1,1),(415,'UPE00000310',10,122,'2026-05-06 15:53:20',1,1),(416,'UPE00000311',10,125,'2026-05-06 15:53:20',1,1),(417,'UPE00000312',10,127,'2026-05-06 15:53:20',1,1),(418,'UPE00000313',10,130,'2026-05-06 15:53:20',1,1),(419,'UPE00000314',10,131,'2026-05-06 15:53:20',1,1),(420,'UPE00000315',10,132,'2026-05-06 15:53:20',1,1),(421,'UPE00000316',10,135,'2026-05-06 15:53:20',1,1),(422,'UPE00000317',10,136,'2026-05-06 15:53:20',1,1),(423,'UPE00000318',10,137,'2026-05-06 15:53:20',1,1),(424,'UPE00000319',10,140,'2026-05-06 15:53:20',1,1),(425,'UPE00000320',10,142,'2026-05-06 15:53:20',1,1),(426,'UPE00000321',10,156,'2026-05-06 15:53:20',1,1),(427,'UPE00000322',10,157,'2026-05-06 15:53:20',1,1),(428,'UPE00000323',10,160,'2026-05-06 15:53:20',1,1),(429,'UPE00000324',10,171,'2026-05-06 15:53:20',1,1),(430,'UPE00000325',10,172,'2026-05-06 15:53:20',1,1),(444,'UPE00000326',11,112,'2026-05-06 15:53:20',1,1),(445,'UPE00000327',11,115,'2026-05-06 15:53:20',1,1),(446,'UPE00000328',11,122,'2026-05-06 15:53:20',1,1),(447,'UPE00000329',11,125,'2026-05-06 15:53:20',1,1),(448,'UPE00000330',11,127,'2026-05-06 15:53:20',1,1),(449,'UPE00000331',11,130,'2026-05-06 15:53:20',1,1),(450,'UPE00000332',11,131,'2026-05-06 15:53:20',1,1),(451,'UPE00000333',11,132,'2026-05-06 15:53:20',1,1),(452,'UPE00000334',11,135,'2026-05-06 15:53:20',1,1),(453,'UPE00000335',11,136,'2026-05-06 15:53:20',1,1),(454,'UPE00000336',11,137,'2026-05-06 15:53:20',1,1),(455,'UPE00000337',11,140,'2026-05-06 15:53:20',1,1),(456,'UPE00000338',11,142,'2026-05-06 15:53:20',1,1),(457,'UPE00000339',11,156,'2026-05-06 15:53:20',1,1),(458,'UPE00000340',11,157,'2026-05-06 15:53:20',1,1),(459,'UPE00000341',11,160,'2026-05-06 15:53:20',1,1),(460,'UPE00000342',11,171,'2026-05-06 15:53:20',1,1),(461,'UPE00000343',11,172,'2026-05-06 15:53:20',1,1),(475,'UPE00000344',12,112,'2026-05-06 15:53:20',1,1),(476,'UPE00000345',12,115,'2026-05-06 15:53:20',1,1),(477,'UPE00000346',12,122,'2026-05-06 15:53:20',1,1),(478,'UPE00000347',12,125,'2026-05-06 15:53:20',1,1),(479,'UPE00000348',12,127,'2026-05-06 15:53:20',1,1),(480,'UPE00000349',12,130,'2026-05-06 15:53:20',1,1),(481,'UPE00000350',12,131,'2026-05-06 15:53:20',1,1),(482,'UPE00000351',12,132,'2026-05-06 15:53:20',1,1),(483,'UPE00000352',12,135,'2026-05-06 15:53:20',1,1),(484,'UPE00000353',12,136,'2026-05-06 15:53:20',1,1),(485,'UPE00000354',12,137,'2026-05-06 15:53:20',1,1),(486,'UPE00000355',12,140,'2026-05-06 15:53:20',1,1),(487,'UPE00000356',12,142,'2026-05-06 15:53:20',1,1),(488,'UPE00000357',12,156,'2026-05-06 15:53:20',1,1),(489,'UPE00000358',12,157,'2026-05-06 15:53:20',1,1),(490,'UPE00000359',12,160,'2026-05-06 15:53:20',1,1),(491,'UPE00000360',12,171,'2026-05-06 15:53:20',1,1),(492,'UPE00000361',12,172,'2026-05-06 15:53:20',1,1);
/*!40000 ALTER TABLE `user_permissions` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_user_permissions_code` BEFORE INSERT ON `user_permissions` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('user_permissions'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `user_roles`
--

DROP TABLE IF EXISTS `user_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_roles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `UserId` int NOT NULL,
  `RoleId` int NOT NULL,
  `AssignedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `AssignedBy` int DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_user_roles_user_role` (`UserId`,`RoleId`),
  UNIQUE KEY `UK_user_roles_code` (`Code`),
  KEY `IX_user_roles_role` (`RoleId`),
  KEY `FK_user_roles_assigned_by` (`AssignedBy`),
  CONSTRAINT `FK_user_roles_assigned_by` FOREIGN KEY (`AssignedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_user_roles_role` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_user_roles_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Gán vai trò cho người dùng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_roles`
--

LOCK TABLES `user_roles` WRITE;
/*!40000 ALTER TABLE `user_roles` DISABLE KEYS */;
INSERT INTO `user_roles` VALUES (1,'URO00000001',1,1,'2026-05-06 15:53:20',1,1),(2,'URO00000002',2,2,'2026-05-06 15:53:20',1,1),(3,'URO00000003',3,3,'2026-05-06 15:53:20',1,1),(4,'URO00000004',4,4,'2026-05-06 15:53:20',1,1),(5,'URO00000005',5,5,'2026-05-06 15:53:20',1,1),(6,'URO00000006',6,6,'2026-05-06 15:53:20',1,1),(7,'URO00000007',7,6,'2026-05-06 15:53:20',1,1),(8,'URO00000008',8,7,'2026-05-06 15:53:20',1,1),(9,'URO00000009',9,7,'2026-05-06 15:53:20',1,1),(10,'URO00000010',10,8,'2026-05-06 15:53:20',1,1),(11,'URO00000011',11,8,'2026-05-06 15:53:20',1,1),(12,'URO00000012',12,8,'2026-05-06 15:53:20',1,1);
/*!40000 ALTER TABLE `user_roles` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_user_roles_code` BEFORE INSERT ON `user_roles` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('user_roles'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `user_tokens`
--

DROP TABLE IF EXISTS `user_tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_tokens` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `UserId` int NOT NULL,
  `AccessToken` varchar(2000) COLLATE utf8mb4_unicode_ci NOT NULL,
  `RefreshToken` varchar(2000) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IssuedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ExpiresAt` datetime NOT NULL,
  `RevokedAt` datetime DEFAULT NULL,
  `ReplacedByToken` varchar(2000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `Jti` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'JWT ID',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_user_tokens_code` (`Code`),
  KEY `IX_user_tokens_user` (`UserId`),
  KEY `IX_user_tokens_active` (`IsActive`),
  KEY `IX_user_tokens_expires` (`ExpiresAt`),
  CONSTRAINT `FK_user_tokens_user` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Token xác thực';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_tokens`
--

LOCK TABLES `user_tokens` WRITE;
/*!40000 ALTER TABLE `user_tokens` DISABLE KEYS */;
INSERT INTO `user_tokens` VALUES (1,'UTK00000001',1,'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJhZG1pbkBzbWFydGx1bmNoLmNvbSIsIkZ1bGxOYW1lIjoiUXXhuqNuIFRy4buLIFZpw6puIiwianRpIjoiYjA2MzNiNDctMTBjMS00M2Q1LWEwZTctYTcxNWFlM2QxOTkyIiwiaWF0IjoxNzc4MDgyODMxLCJleHAiOjE3NzgwODY0MzEsImlzcyI6IlNtYXJ0THVuY2gtQVBJIiwiYXVkIjoiU21hcnRMdW5jaC1Vc2VycyJ9.CVbsK6cs8nPQzrRw6TJFEeawy8jP1HUunn5S6Edf18o','60wRdYUsHGxDVpnLgg6zlBNQohVaHPbxlJwyL0Cxc0ybhxUXCFH6Dv+4xUffUnWHjoXmm229O5HaFyP57AMj9g==','2026-05-06 15:53:51','2026-05-13 15:53:51',NULL,NULL,1,'b0633b47-10c1-43d5-a0e7-a715ae3d1992');
/*!40000 ALTER TABLE `user_tokens` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_user_tokens_code` BEFORE INSERT ON `user_tokens` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('user_tokens'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT COMMENT 'Auto-increment Primary Key',
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `Username` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Tên đăng nhập',
  `Email` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Email',
  `PasswordHash` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Mật khẩu đã hash (BCrypt/Argon2)',
  `FirstName` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Họ',
  `LastName` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Tên',
  `PhoneNumber` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Số điện thoại',
  `AvatarUrl` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Ảnh đại diện',
  `AvatarMediaFileId` int DEFAULT NULL COMMENT 'Media file id cho avatar (ưu tiên hơn AvatarUrl)',
  `Gender` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Giới tính',
  `BirthDate` date DEFAULT NULL COMMENT 'Ngày sinh',
  `Address` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Địa chỉ',
  `Provider` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'system' COMMENT 'Nhà cung cấp xác thực (system, google, facebook, firebase)',
  `IsActive` tinyint(1) NOT NULL DEFAULT '1' COMMENT 'Trạng thái hoạt động',
  `IsEmailVerified` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'Email đã xác thực',
  `EmailVerifiedAt` datetime DEFAULT NULL COMMENT 'Thời điểm xác thực email',
  `LastLoginAt` datetime DEFAULT NULL COMMENT 'Đăng nhập lần cuối',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `CreatedBy` int DEFAULT NULL COMMENT 'Người tạo',
  `UpdatedBy` int DEFAULT NULL COMMENT 'Người cập nhật',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_users_username` (`Username`),
  UNIQUE KEY `UK_users_email` (`Email`),
  UNIQUE KEY `UK_users_code` (`Code`),
  KEY `IX_users_active_created` (`IsActive`,`CreatedAt` DESC),
  KEY `IX_users_provider` (`Provider`),
  KEY `FK_users_created_by` (`CreatedBy`),
  KEY `FK_users_updated_by` (`UpdatedBy`),
  CONSTRAINT `FK_users_created_by` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_users_updated_by` FOREIGN KEY (`UpdatedBy`) REFERENCES `users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Người dùng hệ thống';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'USR00000001','admin','admin@smartlunch.com','$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u','Quản Trị','Viên','+84901000002',NULL,NULL,NULL,NULL,NULL,'system',1,1,'2026-05-06 15:53:20','2026-05-06 15:53:51','2026-05-06 15:53:20',NULL,NULL,NULL),(2,'USR00000002','manager','manager@smartlunch.com','$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u','Minh','Nguyễn','+84901000003',NULL,NULL,NULL,NULL,NULL,'system',1,1,'2026-05-06 15:53:20',NULL,'2026-05-06 15:53:20',NULL,NULL,NULL),(3,'USR00000003','warehouse_staff','warehouse@smartlunch.com','$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u','Khoân Viên','Nguyễn','+84901000004',NULL,NULL,NULL,NULL,NULL,'system',1,1,'2026-05-06 15:53:20',NULL,'2026-05-06 15:53:20',NULL,NULL,NULL),(4,'USR00000004','chef_staff','chef@smartlunch.com','$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u','Bếp','Trần','+84901000005',NULL,NULL,NULL,NULL,NULL,'system',1,1,'2026-05-06 15:53:20',NULL,'2026-05-06 15:53:20',NULL,NULL,NULL),(5,'USR00000005','sales_staff','sales@smartlunch.com','$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u','Bán Hàng','Lê','+84901000006',NULL,NULL,NULL,NULL,NULL,'system',1,1,'2026-05-06 15:53:20',NULL,'2026-05-06 15:53:20',NULL,NULL,NULL),(6,'USR00000006','abc_company','contact@abctech.vn','$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u','Đại Diện','ABC Tech','+84901000007',NULL,NULL,NULL,NULL,NULL,'system',1,1,'2026-05-06 15:53:20',NULL,'2026-05-06 15:53:20',NULL,NULL,NULL),(7,'USR00000007','xyz_school','contact@xyzschool.edu.vn','$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u','Hương','Lê','+84901000008',NULL,NULL,NULL,NULL,NULL,'system',1,1,'2026-05-06 15:53:20',NULL,'2026-05-06 15:53:20',NULL,NULL,NULL),(8,'USR00000008','shipper1','shipper1@smartlunch.com','$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u','Giao','Hàng','+84901000009',NULL,NULL,NULL,NULL,NULL,'system',1,1,'2026-05-06 15:53:20',NULL,'2026-05-06 15:53:20',NULL,NULL,NULL),(9,'USR00000009','shipper2','shipper2@smartlunch.com','$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u','Vận','Chuyển','+84901000010',NULL,NULL,NULL,NULL,NULL,'system',1,1,'2026-05-06 15:53:20',NULL,'2026-05-06 15:53:20',NULL,NULL,NULL),(10,'USR00000010','customer1','customer1@gmail.com','$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u','Anh','Phạm','+84901000011',NULL,NULL,NULL,NULL,NULL,'system',1,1,'2026-05-06 15:53:20',NULL,'2026-05-06 15:53:20',NULL,NULL,NULL),(11,'USR00000011','customer2','customer2@gmail.com','$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u','Bình','Võ','+84901000012',NULL,NULL,NULL,NULL,NULL,'system',1,1,'2026-05-06 15:53:20',NULL,'2026-05-06 15:53:20',NULL,NULL,NULL),(12,'USR00000012','customer3','customer3@gmail.com','$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u','Chi','Đỗ','+84901000013',NULL,NULL,NULL,NULL,NULL,'system',1,0,NULL,NULL,'2026-05-06 15:53:20',NULL,NULL,NULL);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_users_code` BEFORE INSERT ON `users` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('users'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Temporary view structure for view `vw_active_users_with_roles`
--

DROP TABLE IF EXISTS `vw_active_users_with_roles`;
/*!50001 DROP VIEW IF EXISTS `vw_active_users_with_roles`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_active_users_with_roles` AS SELECT 
 1 AS `Id`,
 1 AS `Code`,
 1 AS `Username`,
 1 AS `Email`,
 1 AS `FirstName`,
 1 AS `LastName`,
 1 AS `IsActive`,
 1 AS `IsEmailVerified`,
 1 AS `LastLoginAt`,
 1 AS `CreatedAt`,
 1 AS `Roles`,
 1 AS `RoleIds`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_role_permissions_summary`
--

DROP TABLE IF EXISTS `vw_role_permissions_summary`;
/*!50001 DROP VIEW IF EXISTS `vw_role_permissions_summary`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_role_permissions_summary` AS SELECT 
 1 AS `RoleId`,
 1 AS `RoleName`,
 1 AS `RoleIsActive`,
 1 AS `PermissionCount`,
 1 AS `Permissions`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_user_permissions`
--

DROP TABLE IF EXISTS `vw_user_permissions`;
/*!50001 DROP VIEW IF EXISTS `vw_user_permissions`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_user_permissions` AS SELECT 
 1 AS `UserId`,
 1 AS `Username`,
 1 AS `PermissionId`,
 1 AS `PermissionName`,
 1 AS `Resource`,
 1 AS `Action`,
 1 AS `PermissionSource`,
 1 AS `RoleId`,
 1 AS `RoleName`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `weekly_menu_images`
--

DROP TABLE IF EXISTS `weekly_menu_images`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `weekly_menu_images` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `WeeklyMenuId` int NOT NULL,
  `MediaFileId` int NOT NULL,
  `Role` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'gallery' COMMENT 'cover | gallery',
  `SortOrder` int NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_weekly_menu_images_code` (`Code`),
  KEY `IX_weekly_menu_images_menu` (`WeeklyMenuId`,`SortOrder`),
  KEY `IX_weekly_menu_images_media` (`MediaFileId`),
  CONSTRAINT `FK_weekly_menu_images_media` FOREIGN KEY (`MediaFileId`) REFERENCES `media_files` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_weekly_menu_images_menu` FOREIGN KEY (`WeeklyMenuId`) REFERENCES `weekly_menus` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Ảnh thực đơn tuần';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `weekly_menu_images`
--

LOCK TABLES `weekly_menu_images` WRITE;
/*!40000 ALTER TABLE `weekly_menu_images` DISABLE KEYS */;
/*!40000 ALTER TABLE `weekly_menu_images` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `weekly_menus`
--

DROP TABLE IF EXISTS `weekly_menus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `weekly_menus` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mã tự sinh (trigger)',
  `StartDate` date NOT NULL,
  `EndDate` date NOT NULL,
  `MenuType` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'General' COMMENT 'School | Worker | Office | Custom',
  `EducationLevel` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mẫu giáo | Tiểu học | THCS | THPT (nếu là School)',
  `Description` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Mô tả thực đơn',
  `CreatedBy` int NOT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_weekly_menus_code` (`Code`),
  UNIQUE KEY `UK_weekly_menus_dates_type` (`StartDate`,`EndDate`,`MenuType`,`EducationLevel`),
  KEY `IX_weekly_menus_created_by` (`CreatedBy`),
  CONSTRAINT `FK_weekly_menus_user` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC COMMENT='Thực đơn tuần';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `weekly_menus`
--

LOCK TABLES `weekly_menus` WRITE;
/*!40000 ALTER TABLE `weekly_menus` DISABLE KEYS */;
INSERT INTO `weekly_menus` VALUES (1,'WMN00000001','2025-03-24','2025-03-28','General',NULL,'Thực đơn tuần 4 tháng 3/2025',1,'2026-05-06 15:53:21'),(2,'WMN00000002','2025-03-31','2025-04-04','General',NULL,'Thực đơn tuần 1 tháng 4/2025',1,'2026-05-06 15:53:21');
/*!40000 ALTER TABLE `weekly_menus` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_weekly_menus_code` BEFORE INSERT ON `weekly_menus` FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('weekly_menus'); END IF; END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Dumping events for database 'SmartLunch'
--

--
-- Dumping routines for database 'SmartLunch'
--
/*!50003 DROP FUNCTION IF EXISTS `fn_next_code` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `fn_next_code`(p_table_name VARCHAR(100)) RETURNS varchar(20) CHARSET utf8mb4 COLLATE utf8mb4_unicode_ci
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
    DECLARE v_prefix VARCHAR(5);
    DECLARE v_seq INT;

    UPDATE code_prefixes
    SET LastSequence = LastSequence + 1
    WHERE TableName = p_table_name;

    SELECT Prefix, LastSequence
    INTO v_prefix, v_seq
    FROM code_prefixes
    WHERE TableName = p_table_name;

    RETURN CONCAT(v_prefix, LPAD(v_seq, 8, '0'));
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_check_user_permission` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_check_user_permission`(IN p_user_id INT, IN p_permission_name VARCHAR(200), OUT p_has_permission TINYINT(1))
BEGIN
    DECLARE v_count INT DEFAULT 0;
    SELECT COUNT(*) INTO v_count FROM vw_user_permissions WHERE UserId = p_user_id AND PermissionName = p_permission_name;
    SET p_has_permission = IF(v_count > 0, 1, 0);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_get_users_by_role` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_users_by_role`(IN p_role_name VARCHAR(100))
BEGIN
    SELECT u.Id, u.Code, u.Username, u.Email, u.FirstName, u.LastName, u.IsActive, ur.AssignedAt
    FROM users u INNER JOIN user_roles ur ON u.Id = ur.UserId INNER JOIN roles r ON ur.RoleId = r.Id
    WHERE r.Name = p_role_name AND u.IsActive = 1 AND ur.IsActive = 1 AND r.IsActive = 1 ORDER BY ur.AssignedAt DESC;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_get_user_permissions` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_user_permissions`(IN p_user_id INT)
BEGIN
    SELECT PermissionId, PermissionName, Resource, Action, PermissionSource, RoleId, RoleName
    FROM vw_user_permissions WHERE UserId = p_user_id ORDER BY Resource, Action;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_get_user_roles` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_user_roles`(IN p_user_id INT)
BEGIN
    SELECT r.Id, r.Code, r.Name, r.Description, r.IsActive, ur.AssignedAt, ur.AssignedBy
    FROM user_roles ur INNER JOIN roles r ON ur.RoleId = r.Id
    WHERE ur.UserId = p_user_id AND ur.IsActive = 1 AND r.IsActive = 1 ORDER BY ur.AssignedAt DESC;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Current Database: `SmartLunch`
--

USE `SmartLunch`;

--
-- Final view structure for view `vw_active_users_with_roles`
--

/*!50001 DROP VIEW IF EXISTS `vw_active_users_with_roles`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_active_users_with_roles` AS select `u`.`Id` AS `Id`,`u`.`Code` AS `Code`,`u`.`Username` AS `Username`,`u`.`Email` AS `Email`,`u`.`FirstName` AS `FirstName`,`u`.`LastName` AS `LastName`,`u`.`IsActive` AS `IsActive`,`u`.`IsEmailVerified` AS `IsEmailVerified`,`u`.`LastLoginAt` AS `LastLoginAt`,`u`.`CreatedAt` AS `CreatedAt`,group_concat(distinct `r`.`Name` order by `r`.`Name` ASC separator ', ') AS `Roles`,group_concat(distinct `r`.`Id` order by `r`.`Id` ASC separator ', ') AS `RoleIds` from ((`users` `u` left join `user_roles` `ur` on(((`u`.`Id` = `ur`.`UserId`) and (`ur`.`IsActive` = 1)))) left join `roles` `r` on(((`ur`.`RoleId` = `r`.`Id`) and (`r`.`IsActive` = 1)))) where (`u`.`IsActive` = 1) group by `u`.`Id`,`u`.`Code`,`u`.`Username`,`u`.`Email`,`u`.`FirstName`,`u`.`LastName`,`u`.`IsActive`,`u`.`IsEmailVerified`,`u`.`LastLoginAt`,`u`.`CreatedAt` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_role_permissions_summary`
--

/*!50001 DROP VIEW IF EXISTS `vw_role_permissions_summary`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_role_permissions_summary` AS select `r`.`Id` AS `RoleId`,`r`.`Name` AS `RoleName`,`r`.`IsActive` AS `RoleIsActive`,count(distinct `rp`.`PermissionId`) AS `PermissionCount`,group_concat(distinct `p`.`Name` order by `p`.`Name` ASC separator ', ') AS `Permissions` from ((`roles` `r` left join `role_permissions` `rp` on(((`r`.`Id` = `rp`.`RoleId`) and (`rp`.`IsActive` = 1)))) left join `permissions` `p` on(((`rp`.`PermissionId` = `p`.`Id`) and (`p`.`IsActive` = 1)))) group by `r`.`Id`,`r`.`Name`,`r`.`IsActive` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_user_permissions`
--

/*!50001 DROP VIEW IF EXISTS `vw_user_permissions`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_user_permissions` AS select distinct `u`.`Id` AS `UserId`,`u`.`Username` AS `Username`,`p`.`Id` AS `PermissionId`,`p`.`Name` AS `PermissionName`,`p`.`Resource` AS `Resource`,`p`.`Action` AS `Action`,(case when (`up`.`Id` is not null) then 'direct' when (`rp`.`Id` is not null) then 'role' else 'none' end) AS `PermissionSource`,coalesce(`ur`.`RoleId`,0) AS `RoleId`,coalesce(`r`.`Name`,'') AS `RoleName` from ((((((`users` `u` left join `user_permissions` `up` on(((`u`.`Id` = `up`.`UserId`) and (`up`.`IsActive` = 1)))) left join `permissions` `p1` on(((`up`.`PermissionId` = `p1`.`Id`) and (`p1`.`IsActive` = 1)))) left join `user_roles` `ur` on(((`u`.`Id` = `ur`.`UserId`) and (`ur`.`IsActive` = 1)))) left join `roles` `r` on(((`ur`.`RoleId` = `r`.`Id`) and (`r`.`IsActive` = 1)))) left join `role_permissions` `rp` on(((`r`.`Id` = `rp`.`RoleId`) and (`rp`.`IsActive` = 1)))) left join `permissions` `p` on((((`p1`.`Id` = `p`.`Id`) or (`rp`.`PermissionId` = `p`.`Id`)) and (`p`.`IsActive` = 1)))) where ((`u`.`IsActive` = 1) and (`p`.`Id` is not null)) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-06 15:57:19
