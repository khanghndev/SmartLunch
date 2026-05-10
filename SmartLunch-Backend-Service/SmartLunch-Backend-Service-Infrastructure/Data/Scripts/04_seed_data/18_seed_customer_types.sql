-- =====================================================
-- Seed: Customer Types (aligned with AI rules.json profiles)
-- =====================================================

INSERT INTO customer_types (ProfileKey, Name, Description) VALUES
('default', 'Mặc định', 'Profile mặc định (không phân loại)'),
('industrial', 'Công nghiệp', 'Profile tối ưu suất ăn công nghiệp'),
('vegetarian', 'Ăn chay', 'Ưu tiên món chay / hạn chế thịt động vật'),
('org_company', 'Công ty', 'Suất ăn văn phòng / doanh nghiệp'),
('org_elementary', 'Mẫu giáo', 'Suất ăn cho mầm non (ưu tiên kid_friendly, low_spice)'),
('org_primary_school', 'Tiểu học', 'Suất ăn cho tiểu học'),
('org_secondary_school', 'THCS', 'Suất ăn cho trung học cơ sở'),
('org_high_school', 'THPT', 'Suất ăn cho trung học phổ thông');

