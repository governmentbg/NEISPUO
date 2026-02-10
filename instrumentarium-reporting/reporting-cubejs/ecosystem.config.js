/**
 * Read more about pm2 here: http://pm2.keymetrics.io/
 *
 * Environment is picked from the .env
 */

module.exports = {
    apps: [
        {
            name: 'reporting-cube',
            script: 'index.js',
            /** Instances and mode;  */
            instances: 1,
            instance_id_env: "NODE_APP_INSTANCE",
            exec_mode: 'cluster',
            /** Restart and watch behaviour */
            autorestart: false,
            watch: false,
            ignore_watch: ['node_modules', 'log', '.git', 'files', '.cubestore'],
            max_memory_restart: '1G',
            /** Log configurations */
            log_date_format: 'DD-MM-YYYY HH:mm Z',
            merge_logs: true,
        },
    ],
};
