export const CONSTANTS = {
  JOB_CRON_CUSTOM_EMAIL_NOTIFICATION: '0 0 8 * * 1-5', // Every weekday at 8:00 AM
  JOB_NAME_CUSTOM_EMAIL_NOTIFICATION: 'custom-email-notification',

  JOB_INTERVAL_NAME_UPDATE_JOB: 'update-job',
  JOB_INTERVAL_JOB_UPDATE: 1000 * 60 * 10, // 10 minutes
} as const;
