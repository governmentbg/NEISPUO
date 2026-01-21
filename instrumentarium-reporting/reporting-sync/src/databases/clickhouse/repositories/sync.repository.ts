import { clickHouseClient } from '../clickhouse-client';

export async function truncateTable(target: string): Promise<void> {
  await clickHouseClient.exec({ query: `TRUNCATE TABLE ${target}` });
}
