-- =====================================================
-- Table: customer_types
-- Description: Customer profile types aligned with AI rules (org_*)
-- =====================================================
CREATE TABLE customer_types (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    ProfileKey VARCHAR(100) NOT NULL COMMENT 'Khớp rules.json profiles key (org_*, default, ...)',
    Name VARCHAR(255) NOT NULL,
    Description VARCHAR(1000) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_customer_types_code (Code),
    UNIQUE KEY UK_customer_types_profile_key (ProfileKey)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Customer profile types';

