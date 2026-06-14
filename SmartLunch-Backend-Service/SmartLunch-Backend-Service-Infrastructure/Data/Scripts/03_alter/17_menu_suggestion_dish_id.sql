-- Migration: Add DishId to MenuSuggestionPlanItems
-- Date: 2026-06-14

-- 1. Thêm cột DishId
ALTER TABLE `menu_suggestion_plan_items` 
ADD COLUMN `DishId` int NULL;

-- 2. Thêm khóa ngoại cho DishId (tùy chọn)
-- ALTER TABLE `MenuSuggestionPlanItems`
-- ADD CONSTRAINT `FK_MenuSuggestionPlanItems_Dishes_DishId`
-- FOREIGN KEY (`DishId`) REFERENCES `Dishes` (`Id`)
-- ON DELETE SET NULL;
