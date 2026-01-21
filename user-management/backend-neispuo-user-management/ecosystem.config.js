/**
 * Read more about pm2 here: http://pm2.keymetrics.io/
 *
 * Environment is picked from the .env
 */
 require('dotenv').config({ path: `./.env` });
module.exports = {
  apps: [
    {
      name: `usermng-backend-${process.env.APP_GROUP}`,
      script: 'dist/src/main.js',
      /** Instances and mode;  */
      instances: 1,
      instance_id_env: "NODE_APP_INSTANCE",
      exec_mode: 'cluster',
      /** Restart and watch behaviour */
      autorestart: true,
      watch: false,
      ignore_watch: ['node_modules', 'log', '.git', 'files'],
      max_memory_restart: '1G',
      /**Log configurations */
      out_file: `log/usermng-backend-${process.env.APP_GROUP}.out.log`,
      error_file: `log/usermng-backend-${process.env.APP_GROUP}.err.log`,
      log_date_format: 'DD-MM-YYYY HH:mm Z',
      merge_logs: true,
    },
  ],
};
