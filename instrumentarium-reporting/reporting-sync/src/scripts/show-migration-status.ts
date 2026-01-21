import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import { getMigrationStatus } from '../services/clickhouse-migration.service';
import { error } from '../services/logger.service';

export const showMigrationStatus: CliCommand = {
  name: 'Show Migration Status',
  description: 'Display the status of all database migrations',
  action: async () => {
    await executeCommandWithContinue(
      async () => {
        await getMigrationStatus();
      },
      (err) => error(`❌ ${showMigrationStatus.name} failed`, err),
    );
  },
};
