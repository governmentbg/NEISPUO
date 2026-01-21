-- Migration: Create migrations tracking table
CREATE TABLE IF NOT EXISTS migrations (
    id Int32,
    name String,
    applied_at DateTime,
    status Enum8('COMPLETED' = 1, 'IN_PROGRESS' = 2, 'FAILED' = 3),
    error_message String,
    execution_time_ms Int32
) ENGINE = MergeTree()
ORDER BY (id, applied_at); 