require('dotenv').config({ path: `./.env` });
module.exports = [
    {
        name: 'default',
        type: process.env.DB_CLIENT,
        host: process.env.DB_HOST,
        port: +process.env.DB_PORT,
        username: process.env.DB_USERNAME,
        password: process.env.DB_PASSWORD,
        database: process.env.DB,
        connectionTimeout: 2 * 60 * 1000,
        requestTimeout: 2 * 60 * 1000,
        charset: 'utf8mb4',
        entities: ['dist/**/**/**/*.entity.js'], // enable for normal app start
        // entities: ['src/**/*.entity.ts'], // enable for running cli-scripts
        schema: 'dbo',
        migrationsTableName: 'usermng_migrations',
        migrations: ['dist/migrations/*.js'],
        cli: {
            migrationsDir: 'migrations',
        },
        logging: ['error'],
        pool: {
            min: 10,
            max: 100,
        },
        options: {
            encrypt: false,
            enableArithAbort: true,
        },
        synchronize: false,
    },
];
