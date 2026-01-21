-- Migration: Create jobs table for cron job configurations
CREATE TABLE IF NOT EXISTS jobs (
    name String,
    description String,
    schedule String,
    enabled Boolean DEFAULT true,
    error_message String DEFAULT '',
    created_at DateTime DEFAULT now(),
    updated_at DateTime DEFAULT now()
) ENGINE = MergeTree()
ORDER BY (name); 