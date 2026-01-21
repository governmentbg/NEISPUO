import { ConnectionPool, config as SqlConfig } from 'mssql';
import { ENV_CONFIG } from '../../constants/env-config';
import { info } from '../../services/logger.service';

export async function closeConnection(): Promise<void> {
  try {
    if (msSqlClient.connected) {
      await msSqlClient.close();
    }
  } catch (err) {
    throw new Error(`Failed to close MS SQL connection: ${err}`);
  }
}

export async function ensureConnection(): Promise<void> {
  if (!msSqlClient.connected) {
    info(`[${new Date().toISOString()}] Connecting to MS SQL...`);
    await msSqlClient.connect();
  }
}

export const msSqlClient: ConnectionPool = createMsSqlConnection();

function createMsSqlConnection(config?: Partial<SqlConfig>): ConnectionPool {
  const defaultConfig: SqlConfig = {
    server: ENV_CONFIG.MSSQL.HOST || '',
    port: parseInt(ENV_CONFIG.MSSQL.PORT || ''),
    database: ENV_CONFIG.MSSQL.NAME || '',
    user: ENV_CONFIG.MSSQL.USERNAME || '',
    password: ENV_CONFIG.MSSQL.PASSWORD || '',
    requestTimeout: 2 * 60 * 60 * 1000, // 2h just to be sure all select from views could pass. If they still get a timeout, we might need to reconsider having them.
    options: {
      encrypt: true,
      trustServerCertificate: true,
    },
    pool: {
      max: 30,
      min: 0,
      idleTimeoutMillis: 30000,
    },
  };

  const finalConfig = { ...defaultConfig, ...config };
  return new ConnectionPool(finalConfig);
}
