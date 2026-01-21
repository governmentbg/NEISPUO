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
        seeds: ["src/database/seeds/**/*.ts"],
        synchronize: false,
        schema: 'dbo',
        migrationsTableName: 'survey_migrations',
        migrations: ['migrations/*.ts'],
        cli: {
            migrationsDir: 'migrations'
        },
        logging: true
    },
    {
        name: 'app-seed',
        type: process.env.DB_CLIENT,
        host: process.env.DB_HOST,
        port: +process.env.DB_PORT,
        username: process.env.DB_USERNAME,
        password: process.env.DB_PASSWORD,
        database: process.env.DB,
        charset: 'utf8mb4',
        entities: ['src/**/*.entity{.ts,.js}'],
        seeds: ["src/resources/seeds/**/*.ts"],
        synchronize: false,
        migrations: ['migrations/*.ts'],
        cli: {
            migrationsDir: 'migrations'
        },
        logging: true
    }
];
