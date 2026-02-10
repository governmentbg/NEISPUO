// This file is consumed by typeOrm CLI
require('dotenv').config({ path: './config/.env' });

module.exports = [
    {
        name: 'default',
        type: process.env.DB_CLIENT,
        host: process.env.DB_HOST,
        port: +process.env.DB_PORT,
        username: process.env.DB_USERNAME,
        password: process.env.DB_PASSWORD,
        database: process.env.DB,
        charset: 'utf8mb4',
        entities: ['dist/**/*.entity{.ts,.js}'],
        synchronize: false,
        schema: 'dbo',
        migrationsTableName: 'rmi_migrations',
        migrations: ['migrations/*.ts'],
        cli: {
            migrationsDir: 'migrations',
        },
        logging: ['error'],
    },
    {
        name: process.env.BLOBS_DB_CONNECTION,
        type: process.env.BLOBS_DB_CLIENT,
        host: process.env.BLOBS_DB_HOST,
        port: +process.env.BLOBS_DB_PORT,
        username: process.env.BLOBS_DB_USERNAME,
        password: process.env.BLOBS_DB_PASSWORD,
        database: process.env.BLOBS_DB,
        charset: 'utf8mb4',
        entities: ['dist/**/blob*.entity{.ts,.js}'],
        synchronize: false,
        schema: 'dbo',
        migrationsTableName: 'rmi_migrations',
        // migrations: ['migrations/*.ts'],
        // cli: {
        //     migrationsDir: 'migrations'
        // },
        logging: ['error'],
    },

    {
        name: 'app-seed',
        type: process.env.DB_CLIENT,
        host: process.env.DB_HOST,
        port: process.env.DB_PORT,
        username: process.env.DB_USERNAME,
        password: process.env.DB_PASSWORD,
        database: process.env.DB,
        entities: ['dist/**/*.entity{.ts,.js}'],
        synchronize: false,
        migrations: ['seeds/*.ts'],
        cli: {
            migrationsDir: 'seeds',
        },
    },
];
