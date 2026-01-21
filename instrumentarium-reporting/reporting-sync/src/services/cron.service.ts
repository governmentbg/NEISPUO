import * as cron from 'node-cron';
import { CRON_JOBS } from '../constants/cron-job-registry';
import {
  fetchJobByName,
  fetchJobs,
  insertJob,
  updateJobScheduleAndStatus,
  updateJobStatus,
} from '../databases/clickhouse/repositories/cron-job.repository';
import {
  closeConnection,
  ensureConnection,
} from '../databases/mssql/mssql-client';
import { CronJob, DatabaseJob } from '../interfaces';
import { cleanupTempFiles, setupGracefulShutdown } from '../utils';

import { confirm, input, select } from '@inquirer/prompts';
import {
  addTaskMapping,
  getAllTaskMappings,
  getEnabledJobs,
  getLastKnownState,
  hasJobConfigurationChanged,
  initializeLastKnownStates,
  needsRestart,
  registerJobs,
  updateJobState,
  updateLastKnownState,
  validateJobs,
} from './cron-job-manager.service';
import { error, info } from './logger.service';

export const isValidCron = (expression: string): boolean => {
  return cron.validate(expression);
};

export const executeSyncJob = async (
  action: () => Promise<void>,
  onError: (err: any) => void,
): Promise<void> => {
  try {
    await action();
  } catch (err) {
    onError(err);
  }
};

export const stopAllJobs = (): void => {
  const taskMappings = getAllTaskMappings();
  taskMappings.forEach((task, jobName) => {
    task.stop();
    info(`Stopped job: ${jobName}`);
  });
  info('All cron jobs stopped');
};

export const startJob = (job: CronJob): void => {
  info(`Starting job: ${job.name} (${job.schedule})`);

  const task = cron.schedule(job.schedule, async () => {
    try {
      info(`[${new Date().toISOString()}] Executing job: ${job.name}`);
      await job.task();
      info(`[${new Date().toISOString()}] Completed job: ${job.name}`);
    } catch (err) {
      error(`[${new Date().toISOString()}] Error in job ${job.name}:`, err);
    }
  });

  addTaskMapping(job.name, task);
};

const startAllJobs = (): void => {
  info('Starting cron job manager...');

  const enabledJobs = getEnabledJobs();
  enabledJobs.forEach(startJob);

  info(`Started ${enabledJobs.length} cron jobs`);
};

async function syncJobsWithDatabase(): Promise<void> {
  try {
    info('Syncing jobs with database...');
    
    await ensureConnection();

    const dbJobs = await fetchJobs();

    const validatedJobs = validateJobs(dbJobs);

    for (const job of validatedJobs) {
      await updateJobStatus(job.name, job.enabled, job.error_message);
    }

    const dbJobNames = new Set(dbJobs.map((job) => job.name));
    const enabledJobs = getEnabledJobs();
    for (const cronJob of enabledJobs) {
      if (!dbJobNames.has(cronJob.name)) {
        const newDbJob: DatabaseJob = {
          name: cronJob.name,
          description: cronJob.description || '',
          schedule: cronJob.schedule,
          enabled: cronJob.enabled,
          error_message: '',
          created_at: new Date(),
          updated_at: new Date(),
        };
        await insertJob(newDbJob);
      }
    }

    const dbJobMap = new Map(dbJobs.map((job) => [job.name, job]));

    for (const fileJob of enabledJobs) {
      const dbJob = dbJobMap.get(fileJob.name);

      if (dbJob) {
        info(`Syncing job '${fileJob.name}' with database configuration`);

        updateJobState(fileJob.name, {
          enabled: dbJob.enabled,
          description: dbJob.description,
          schedule: dbJob.schedule,
          error_message: dbJob.error_message,
        });
      }
    }

    info(`Synced ${enabledJobs.length} jobs with database`);
  } catch (err) {
    error('Failed to sync with database:', err);
    info('Continuing with file-based jobs only...');
  }
}

export const startCronService = async (): Promise<void> => {
  await setupGracefulShutdown(async () => {
    stopAllJobs();
    await cleanupTempFiles();
    await closeConnection();
    info('Goodbye! 👋');
  });

  info('🚀 Starting reporting-sync cron service...');

  registerJobs(CRON_JOBS);

  await syncJobsWithDatabase();

  initializeLastKnownStates();

  startAllJobs();
};

export const checkForRuntimeChanges = async (): Promise<void> => {
  const dbJobs = await fetchJobs();
  const dbJobMap = new Map(dbJobs.map((job) => [job.name, job]));

  let changesDetected = 0;

  const enabledJobs = getEnabledJobs();
  for (const localJob of enabledJobs) {
    const dbJob = dbJobMap.get(localJob.name);

    if (dbJob) {
      const lastKnownState = getLastKnownState(localJob.name);
      const hasChanged = hasJobConfigurationChanged(lastKnownState, dbJob);

      if (hasChanged) {
        changesDetected++;
        info(`Job configuration changed for '${localJob.name}'`);

        if (needsRestart(lastKnownState, dbJob)) {
          await restartJobWithNewConfig(localJob.name, dbJob);
        }

        updateLastKnownState(localJob.name, {
          enabled: dbJob.enabled,
          schedule: dbJob.schedule,
          description: dbJob.description,
        });
      }
    }
  }

  if (changesDetected > 0) {
    info(`Detected ${changesDetected} job configuration changes`);
  }
};

const restartJobWithNewConfig = async (
  jobName: string,
  dbJob: any,
): Promise<void> => {
  const taskMappings = getAllTaskMappings();
  const existingTask = taskMappings.get(jobName);

  if (existingTask) {
    existingTask.stop();
    info(`   - Stopped existing job '${jobName}'`);
  }

  updateJobState(jobName, {
    enabled: dbJob.enabled,
    schedule: dbJob.schedule,
    description: dbJob.description,
  });

  if (dbJob.enabled) {
    const enabledJobs = getEnabledJobs();
    const job = enabledJobs.find((j) => j.name === jobName);
    if (job) {
      startJob(job);
      info(
        `   - Started job '${jobName}' with new schedule: ${dbJob.schedule}`,
      );
    }
  } else {
    info(`   - Job '${jobName}' is disabled, not starting`);
  }
};

export const updateJobConfiguration = async (
  jobName: string,
  schedule: string,
  enabled: boolean,
): Promise<void> => {
  const existingJob = await fetchJobByName(jobName);
  if (!existingJob) {
    throw new Error(`Job '${jobName}' not found in database`);
  }

  if (!isValidCron(schedule)) {
    throw new Error(`Invalid cron schedule format: ${schedule}`);
  }

  info(`Updating job '${jobName}' configuration...`);
  info(`  - Schedule: ${existingJob.schedule} → ${schedule}`);
  info(`  - Enabled: ${existingJob.enabled} → ${enabled}`);

  await updateJobScheduleAndStatus(jobName, schedule, enabled);

  updateJobState(jobName, {
    schedule,
    enabled,
  });

  const taskMappings = getAllTaskMappings();
  const existingTask = taskMappings.get(jobName);

  if (existingTask) {
    existingTask.stop();
    info(`   - Stopped existing job '${jobName}'`);
  }

  if (enabled) {
    const enabledJobs = getEnabledJobs();
    const job = enabledJobs.find((j) => j.name === jobName);
    if (job) {
      startJob(job);
      info(`   - Started job '${jobName}' with new configuration`);
    }
  } else {
    info(`   - Job '${jobName}' is disabled, not starting`);
  }

  updateLastKnownState(jobName, {
    enabled,
    schedule,
    description: existingJob.description,
  });

  info(`✅ Successfully updated job '${jobName}' configuration`);
};

export const executeJobConfigurationUpdate = async (): Promise<void> => {
  const jobs = await fetchJobs();

  if (jobs.length === 0) {
    info('   No jobs found in database');
    return;
  }

  info('\n📋 Available jobs:');
  jobs.forEach((job, index) => {
    const status = job.enabled ? '✅ Enabled' : '❌ Disabled';
    info(`   ${index + 1}. ${job.name}: ${status}`);
    info(`      Schedule: ${job.schedule}`);
    info(`      Description: ${job.description}`);
    info('');
  });

  const selectedJobName = await select({
    message: 'Select a job to update:',
    choices: jobs.map((job) => ({
      name: `${job.name} (${job.enabled ? 'Enabled' : 'Disabled'})`,
      value: job.name,
    })),
  });

  const selectedJob = jobs.find((job) => job.name === selectedJobName);
  if (!selectedJob) {
    throw new Error('Job not found');
  }

  info(`\n🔧 Updating job: ${selectedJob.name}`);
  info(`Current configuration:`);
  info(`  - Schedule: ${selectedJob.schedule}`);
  info(`  - Enabled: ${selectedJob.enabled ? 'Yes' : 'No'}`);
  info(`  - Description: ${selectedJob.description}`);

  const newSchedule = await input({
    message:
      'Enter new cron schedule (e.g., "0 0 * * *" for daily at midnight):',
    default: selectedJob.schedule,
    validate: (input: string) => {
      if (!isValidCron(input)) {
        return 'Invalid cron schedule format. Please use standard cron syntax.';
      }
      return true;
    },
  });

  const newEnabled = await confirm({
    message: 'Should this job be enabled?',
    default: selectedJob.enabled,
  });

  info(`\n📝 Summary of changes:`);
  info(`  - Schedule: ${selectedJob.schedule} → ${newSchedule}`);
  info(
    `  - Enabled: ${selectedJob.enabled ? 'Yes' : 'No'} → ${
      newEnabled ? 'Yes' : 'No'
    }`,
  );

  const confirmed = await confirm({
    message: 'Do you want to apply these changes?',
    default: false,
  });

  if (!confirmed) {
    throw new Error('Operation cancelled by user');
  }

  await updateJobConfiguration(selectedJob.name, newSchedule, newEnabled);
};

export const executeShowDatabaseJobs = async (): Promise<void> => {
  const jobs = await fetchJobs();

  if (jobs.length === 0) {
    info('   No jobs found in database');
    return;
  }

  jobs.forEach((job) => {
    const status = job.enabled ? '✅ Enabled' : '❌ Disabled';
    const error = job.error_message ? ` (Error: ${job.error_message})` : '';
    info(`   - ${job.name}: ${status}${error}`);
    info(`     Schedule: ${job.schedule}`);
    info(`     Description: ${job.description}`);
    info('');
  });
};
