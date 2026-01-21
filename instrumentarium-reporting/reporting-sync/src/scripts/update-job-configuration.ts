import { CliCommand } from '../interfaces';
import { executeCommandWithContinue } from '../services/cli.service';
import { executeJobConfigurationUpdate } from '../services/cron.service';
import { error, info } from '../services/logger.service';

export const updateJobConfigurationScript: CliCommand = {
  name: 'Update Job Configuration',
  description: 'Update schedule and enabled status of a job',
  action: async () => {
    await executeCommandWithContinue(
      async () => {
        await executeJobConfigurationUpdate();
        info('✅ Job configuration updated successfully!');
      },
      (err) => error('Failed to update job configuration:', err),
    );
  },
};
