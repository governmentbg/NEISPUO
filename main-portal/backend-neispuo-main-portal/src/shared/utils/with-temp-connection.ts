import { Connection, createConnection } from 'typeorm';
import { SqlServerConnectionOptions } from 'typeorm/driver/sqlserver/SqlServerConnectionOptions';

/**
 * Creates a temporary database connection with a specified request timeout.
 * This is useful for long-running operations that require a dedicated connection.
 *
 * @param base - The base connection to use for creating the temporary connection.
 * @param requestTimeout - The request timeout in milliseconds for the temporary connection.
 * @param run - A function that takes the temporary connection and returns a promise of type T.
 * @returns A promise that resolves to the result of the run function.
 */
export async function withTempConnection<T>(
  base: Connection,
  requestTimeout: number,
  run: (c: Connection) => Promise<T>,
) {
  const tmpName = `longRunning-${Date.now()}`;

  const opts: SqlServerConnectionOptions = {
    ...(base.options as SqlServerConnectionOptions),
    name: tmpName,
    requestTimeout,
    pool: { max: 1, min: 0 },
  };

  const tmp = await createConnection(opts);
  try {
    return await run(tmp);
  } finally {
    await tmp.close();
  }
}
