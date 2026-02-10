module.exports = {
  apps: [
    {
      name: 'reporting-sync',
      script: 'dist/entrypoints/cron-job.entrypoints.js',
      instances: 1,
      instance_id_env: 'NODE_APP_INSTANCE',
      exec_mode: 'cluster',
      autorestart: false,
      watch: false,
      ignore_watch: ['node_modules', 'log', '.git', 'files'],
      log_date_format: 'DD-MM-YYYY HH:mm Z',
      merge_logs: true,
      // Note: Kubernetes memory limits may kill the container before PM2's 1.5G restart triggers.
      max_memory_restart: '1500M',
      node_args: '--max-old-space-size=512',
    },
  ],
};
