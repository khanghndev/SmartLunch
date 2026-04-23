CREATE TABLE sentiments (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    ReviewId INT NOT NULL,
    SentimentLabel VARCHAR(20) NOT NULL,
    Confidence DECIMAL(4,2) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_sentiments_code (Code),
    UNIQUE KEY UK_sentiments_review (ReviewId),
    CONSTRAINT FK_sentiments_review FOREIGN KEY (ReviewId) REFERENCES reviews (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Phân tích cảm xúc';
