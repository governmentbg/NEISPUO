import { migrate } from '../services/clickhouse-migration.service';
import { error } from '../services/logger.service';

async function runMigrations(): Promise<void> {
  try {
    console.log('Starting migration process...');
    await migrate();
    console.log('Migration process completed successfully!');
    process.exit(0);
  } catch (err) {
    console.error('Migration failed:', err);
    error('❌ Run Migrations failed', err);
    process.exit(1);
  }
}

if (require.main === module) {
  runMigrations();
}

export { runMigrations };
