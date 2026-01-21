import * as fs from 'fs';
import { join } from 'path';
import { MIGRATIONS_PATH } from '../constants/migrations-path';
import {
  checkMigrationsTableExists,
  executeQuery,
  getAppliedMigrations,
  recordMigration,
} from '../databases/clickhouse/repositories/migration.reporistory';
import { MigrationStatusEnum } from '../enums/migration-status.enum';
import { info } from './logger.service';

function extractMigrationId(filename: string): number {
  const match = filename.match(/^(\d+)_/);
  return match ? parseInt(match[1], 10) : 0;
}

async function getMigrationFiles(
  migrationsPath: string = MIGRATIONS_PATH,
): Promise<string[]> {
  const files = await fs.promises.readdir(migrationsPath);
  return files.filter((file) => file.endsWith('.sql'));
}

async function ensureMigrationsTable(
  migrationsPath: string = MIGRATIONS_PATH,
): Promise<void> {
  const migrationFile = '000_create_migrations_table.sql';
  const filePath = join(migrationsPath, migrationFile);

  try {
    await checkMigrationsTableExists();
    return;
  } catch (err) {
    const sql = await fs.promises.readFile(filePath, 'utf8');
    await executeQuery(sql);
  }
}

async function applyMigration(
  filename: string,
  migrationsPath: string = MIGRATIONS_PATH,
): Promise<void> {
  const migrationId = extractMigrationId(filename);
  const startTime = Date.now();

  info(`Applying migration: ${filename}`);

  try {
    const filePath = join(migrationsPath, filename);
    const sql = await fs.promises.readFile(filePath, 'utf8');

    await recordMigration(
      migrationId,
      filename,
      MigrationStatusEnum.IN_PROGRESS,
      0,
    );

    await executeQuery(sql);

    const executionTime = Date.now() - startTime;

    await recordMigration(
      migrationId,
      filename,
      MigrationStatusEnum.COMPLETED,
      executionTime,
    );

    info(`✓ Migration ${filename} applied successfully (${executionTime}ms)`);
  } catch (err) {
    const executionTime = Date.now() - startTime;
    const errorMessage = err instanceof Error ? err.message : String(err);

    await recordMigration(
      migrationId,
      filename,
      MigrationStatusEnum.FAILED,
      executionTime,
      errorMessage,
    );

    throw err;
  }
}

export async function migrate(migrationsPath?: string): Promise<void> {
  const finalMigrationsPath = migrationsPath || MIGRATIONS_PATH;

  info('Starting migration process...');

  await ensureMigrationsTable(finalMigrationsPath);

  const migrationFiles = await getMigrationFiles(finalMigrationsPath);
  const appliedMigrations = await getAppliedMigrations();

  const appliedNames = new Set(appliedMigrations.map((m) => m.name));

  const pendingMigrations = migrationFiles.filter(
    (file) => !appliedNames.has(file),
  );

  if (pendingMigrations.length === 0) {
    info('No pending migrations found.');
    return;
  }

  info(`Found ${pendingMigrations.length} pending migrations:`);
  pendingMigrations.forEach((file) => info(`  - ${file}`));

  for (const migrationFile of pendingMigrations) {
    await applyMigration(migrationFile, finalMigrationsPath);
  }

  info('Migration process completed successfully!');
}

export async function getMigrationStatus(
  migrationsPath?: string,
): Promise<void> {
  const finalMigrationsPath = migrationsPath || MIGRATIONS_PATH;

  const appliedMigrations = await getAppliedMigrations();
  const migrationFiles = await getMigrationFiles(finalMigrationsPath);

  info('\nMigration Status:');
  info('================');

  for (const file of migrationFiles) {
    const applied = appliedMigrations.find((m) => m.name === file);

    if (applied) {
      const statusIcon =
        applied.status === 'COMPLETED'
          ? '✓'
          : applied.status === 'FAILED'
          ? '✗'
          : '⏳';
      info(`${statusIcon} ${file} (${applied.status})`);

      if (applied.status === 'FAILED' && applied.error_message) {
        info(`    Error: ${applied.error_message}`);
      }

      if (applied.execution_time_ms > 0) {
        info(`    Duration: ${applied.execution_time_ms}ms`);
      }
    } else {
      info(`⏸️  ${file} (PENDING)`);
    }
  }

  const stats = {
    total: migrationFiles.length,
    applied: appliedMigrations.filter((m) => m.status === 'COMPLETED').length,
    failed: appliedMigrations.filter((m) => m.status === 'FAILED').length,
    pending: migrationFiles.length - appliedMigrations.length,
  };

  info('\nSummary:');
  info(`  Total migrations: ${stats.total}`);
  info(`  Applied: ${stats.applied}`);
  info(`  Failed: ${stats.failed}`);
  info(`  Pending: ${stats.pending}`);
}
