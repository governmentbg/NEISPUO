import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import { executeShowDatabaseJobs } from '../services/cron.service';
import { error, info } from '../services/logger.service';

export const showDatabaseJobs: CliCommand = {
  name: 'Show Database Jobs',
  description: 'Display all jobs currently in the database',
  action: async () => {
    info('\n📊 Jobs in database:');
    
    await executeCommandWithContinue(
      async () => {
        await executeShowDatabaseJobs();
      },
      (err) => error('Failed to fetch jobs:', err),
    );
  }
}; 