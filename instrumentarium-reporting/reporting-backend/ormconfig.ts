// This file is consumed by typeOrm CLI
require('dotenv').config({ path: './config/.env' });
import { DataSource } from 'typeorm';

const AppDataSource = new DataSource({
  name: 'default',
  type: 'mssql',
  host: process.env.DB_HOST,
  port: +process.env.DB_PORT,
  username: process.env.DB_USERNAME,
  password: process.env.DB_PASSWORD,
  database: process.env.DB,
  requestTimeout: 20 * 60 * 1000,
  entities: ['dist/src/domain/**/*.entity.{js,ts}'],
  synchronize: false,
  schema: 'dbo',
  migrationsTableName: 'reporting_migrations',
  migrations: ['dist/migrations/*.{js,ts}'],
  logging: ['error'],
  extra: {
    trustServerCertificate: true,
  },
});

export default AppDataSource;
