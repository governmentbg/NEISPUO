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
    schema: 'dbo',
    migrationsTableName: 'main_portal_migrations',
    migrations: ['dist/migrations/*.js'],
    cli: {
      migrationsDir: 'migrations',
    },
    synchronize: false,
    logging: ['error'],
  },
];
