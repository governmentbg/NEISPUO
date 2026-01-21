import * as dotenv from 'dotenv';
import { join } from 'path';

dotenv.config({ path: join(process.cwd(), 'config/.env') });

export const ENV_CONFIG = {
  CLICKHOUSE: {
    CLIENT: process.env.CLICKHOUSE_DB_CLIENT || '',
    URL: process.env.CLICKHOUSE_DB_URL || '',
    NAME: process.env.CLICKHOUSE_DB_NAME || '',
    USERNAME: process.env.CLICKHOUSE_DB_USERNAME || '',
    PASSWORD: process.env.CLICKHOUSE_DB_PASSWORD || '',
  },
  MSSQL: {
    HOST: process.env.MS_SQL_DB_HOST || '',
    PORT: process.env.MS_SQL_DB_PORT || '',
    NAME: process.env.MS_SQL_DB_NAME || '',
    USERNAME: process.env.MS_SQL_DB_USERNAME || '',
    PASSWORD: process.env.MS_SQL_DB_PASSWORD || '',
  },
} as const;
