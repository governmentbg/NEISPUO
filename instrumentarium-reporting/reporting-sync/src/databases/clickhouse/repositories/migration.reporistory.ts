import { MigrationStatusEnum } from '../../../enums/migration-status.enum';
import { Migration } from '../../../interfaces';
import { clickHouseClient } from '../clickhouse-client';

function removeDuplicateMigrations(migrations: Migration[]): Migration[] {
  const uniqueMigrations: Migration[] = [];
  const seenKeys = new Set<string>();

  for (const migration of migrations) {
    const key = `${migration.id}-${migration.name}`;
    if (!seenKeys.has(key)) {
      seenKeys.add(key);
      uniqueMigrations.push(migration);
    }
  }

  return uniqueMigrations;
}


export async function getAppliedMigrations(): Promise<Migration[]> {
  try {
    const result = await clickHouseClient.query({
      query: 'SELECT * FROM migrations ORDER BY id',
    });
    
    const responseData = await result.json();
    const rawMigrations = (responseData.data || []) as Migration[];
    
    return removeDuplicateMigrations(rawMigrations);
  } catch (err) {
    // Ignore errors if migrations table doesn't exist, return empty array
    return [];
  }
}

export async function recordMigration(
  id: number,
  name: string,
  status: MigrationStatusEnum,
  executionTimeMs: number,
  errorMessage?: string,
): Promise<void> {
  const deleteQuery = `
    ALTER TABLE migrations DELETE WHERE id = ${id} AND name = '${name}'
  `;

  try {
    await executeQuery(deleteQuery);
  } catch (err) {
    // Ignore errors if no record exists to delete
  }

  const insertQuery = `
    INSERT INTO migrations (id, name, applied_at, status, error_message, execution_time_ms)
    VALUES (${id}, '${name}', now(), '${status}', '${
    errorMessage || ''
  }', ${executionTimeMs})
  `;

  await executeQuery(insertQuery);
}

export async function executeQuery(sql: string): Promise<void> {
  await clickHouseClient.exec({ query: sql });
}

export async function checkMigrationsTableExists(): Promise<void> {
  await executeQuery('SELECT 1 FROM migrations LIMIT 1');
}
