-- =====================================
-- SmartLunch Database - Initialization
-- MySQL 8.0+ Compatible
-- Full UTF-8 (utf8mb4) for Vietnamese support
-- =====================================

DROP DATABASE IF EXISTS SmartLunch;
CREATE DATABASE SmartLunch
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE SmartLunch;

-- Đảm bảo connection hỗ trợ tiếng Việt đầy đủ
SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;
SET character_set_connection = utf8mb4;
