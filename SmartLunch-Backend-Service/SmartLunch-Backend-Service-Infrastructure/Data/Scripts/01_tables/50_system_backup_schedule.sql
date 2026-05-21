CREATE TABLE IF NOT EXISTS system_backup_schedule (
    Id INT NOT NULL DEFAULT 1 COMMENT 'Singleton row',
    IsEnabled TINYINT(1) NOT NULL DEFAULT 0,
    ScheduleMode VARCHAR(16) NOT NULL DEFAULT 'Daily' COMMENT 'Daily | Weekly | Once',
    TimeOfDayMinutes INT NOT NULL DEFAULT 180 COMMENT 'Minutes from midnight VN time',
    DayOfWeek INT NULL COMMENT '0=Sunday .. 6=Saturday for Weekly',
    OnceScheduledAt DATETIME NULL COMMENT 'One-shot datetime VN',
    LastRunAt DATETIME NULL,
    NextRunAt DATETIME NULL,
    UpdatedAt DATETIME NOT NULL,
    UpdatedByUserId INT NULL,
    PRIMARY KEY (Id)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Auto backup schedule (singleton)';

INSERT INTO system_backup_schedule (Id, IsEnabled, ScheduleMode, TimeOfDayMinutes, DayOfWeek, UpdatedAt)
SELECT 1, 0, 'Daily', 1260, NULL, NOW()
WHERE NOT EXISTS (SELECT 1 FROM system_backup_schedule WHERE Id = 1);
