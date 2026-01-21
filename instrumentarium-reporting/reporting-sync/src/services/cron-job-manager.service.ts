import { CronJob, DatabaseJob } from '../interfaces';
import { info } from './logger.service';

const jobState = {
  jobs: [] as CronJob[],
  jobTaskMappings: new Map<string, any>(),
  lastKnownJobStates: new Map<string, any>(),
};

export const getEnabledJobs = (): CronJob[] => {
  return jobState.jobs.filter((job) => job.enabled);
};

export const getAllTaskMappings = (): Map<string, any> => {
  return jobState.jobTaskMappings;
};

export const validateJobs = (dbJobs: DatabaseJob[]): DatabaseJob[] => {
  const cronJobNames = new Set(jobState.jobs.map((job) => job.name));

  return dbJobs.map((dbJob) => {
    if (!cronJobNames.has(dbJob.name)) {
      return {
        ...dbJob,
        enabled: false,
        error_message: `Job '${dbJob.name}' not found in CRON_JOBS array`,
      };
    }
    return dbJob;
  });
};

export const hasJobConfigurationChanged = (
  lastKnownState: any,
  dbJob: any,
): boolean => {
  if (!lastKnownState) return true;

  return (
    lastKnownState.enabled !== dbJob.enabled ||
    lastKnownState.schedule !== dbJob.schedule ||
    lastKnownState.description !== dbJob.description
  );
};

export const needsRestart = (lastKnownState: any, dbJob: any): boolean => {
  if (!lastKnownState) return true;

  return (
    lastKnownState.schedule !== dbJob.schedule ||
    lastKnownState.enabled !== dbJob.enabled
  );
};

export const getLastKnownState = (jobName: string): any => {
  return jobState.lastKnownJobStates.get(jobName);
};

export const registerJobs = (jobs: readonly CronJob[]): void => {
  jobState.jobs = [...jobs];
  jobs.forEach((job) => {
    info(`Registered cron job: ${job.name} with schedule: ${job.schedule}`);
  });
};

export const addTaskMapping = (jobName: string, task: any): void => {
  jobState.jobTaskMappings.set(jobName, task);
};

export const updateJobState = (
  jobName: string,
  updates: Partial<CronJob>,
): void => {
  for (let i = 0; i < jobState.jobs.length; i++) {
    if (jobState.jobs[i].name === jobName) {
      jobState.jobs[i] = { ...jobState.jobs[i], ...updates };
      break;
    }
  }
};

export const initializeLastKnownStates = (): void => {
  jobState.jobs.forEach((job) => {
    jobState.lastKnownJobStates.set(job.name, {
      enabled: job.enabled,
      schedule: job.schedule,
      description: job.description,
    });
  });
};

export const updateLastKnownState = (jobName: string, state: any): void => {
  jobState.lastKnownJobStates.set(jobName, state);
};
