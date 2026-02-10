import { createClient } from '@clickhouse/client';
import { ENV_CONFIG } from '../../constants/env-config';
import { NodeClickHouseClientConfigOptions } from '@clickhouse/client/dist/config';

export const clickHouseClient = createClickHouseConnection();

function createClickHouseConnection(config?: NodeClickHouseClientConfigOptions) {
  const defaultConfig: NodeClickHouseClientConfigOptions = {
    url: ENV_CONFIG.CLICKHOUSE.URL || '',
    username: ENV_CONFIG.CLICKHOUSE.USERNAME || '',
    password: ENV_CONFIG.CLICKHOUSE.PASSWORD || '',
    database: ENV_CONFIG.CLICKHOUSE.NAME || '',
  };

  const finalConfig = { ...defaultConfig, ...config };

  return createClient(finalConfig);
}
