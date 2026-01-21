import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import { migrate } from '../services/clickhouse-migration.service';
import { error } from '../services/logger.service';

export const runMigrations: CliCommand = {
  name: 'Run Migrations',
  description: 'Apply pending database migrations',
  action: async () => {
    await executeCommandWithContinue(
      async () => {
        await migrate();
      },
      (err) => error(`❌ ${runMigrations.name} failed`, err),
    );
  },
};
