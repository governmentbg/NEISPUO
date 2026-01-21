/**
 * Read more about pm2 here: http://pm2.keymetrics.io/
 *
 * Environment is picked from the .env
 */

module.exports = {
    apps: [
        {
            name: 'rmi-backend',
            script: 'dist/src/main.js',
            /** Instances and mode;  */
            instances: 4,
            exec_mode: 'cluster',

            /** Restart and watch behaviour */
            autorestart: false,
            watch: false,
            ignore_watch: ['node_modules', 'log', '.git', 'files'],
            max_memory_restart: '1G',
            /** Log configurations */
            out_file: 'log/rmi-backend.out.log',
            error_file: 'log/rmi-backend.err.log',
            log_date_format: 'DD-MM-YYYY HH:mm Z',
            merge_logs: true,
        },
    ],
};
