import * as fs from 'fs';
import * as http from 'http';
import { Duration } from 'luxon';
import { tmpdir } from 'os';
import { join } from 'path';
import { truncateTable } from '../databases/clickhouse/repositories/sync.repository';
import {
  executeStreamingQuery,
  getAllSynchronizationLogs,
  getSynchronizationLog,
  upsertSynchronizationLog,
} from '../databases/mssql/repositories/sync.repository';

import { ENV_CONFIG } from '../constants/env-config';
import { ensureConnection } from '../databases/mssql/mssql-client';
import { SyncStatusEnum } from '../enums/sync-status.enum';
import { ColumnMapping, SyncConfig, SyncResult } from '../interfaces';
import {
  createSyncResult,
  generateSyncId,
  registerTempFile,
  safeUnlink,
} from '../utils';
import { isHttpSuccessfulResponse } from '../utils/isHttpSuccessfulResponse';
import { error, info } from './logger.service';
import { ChunkConfig } from '../interfaces/sync/chunk-config.interface';

export const logSyncError = (scriptName: string, err?: any): void => {
  error(`❌ ${scriptName} sync failed.`, err);
};

export const logSyncResult = (result: SyncResult): void => {
  if (result.success) {
    info('✅ Sync completed successfully!');
    info(`📊 Records processed: ${result.recordsProcessed}`);
    info(`📊 Records inserted: ${result.recordsInserted}`);
    info(
      `⏱️  Execution time: ${Duration.fromMillis(
        result.executionTimeMs,
      ).toFormat('s.SS')}`,
    );
  } else {
    throw new Error(
      `Logic error: Sync result indicates failure but no exception was thrown. Error: ${
        result.error || 'Unknown error'
      }`,
    );
  }
};

export const getSyncStatus = async (
  sourceName: string,
  targetName: string,
): Promise<{
  status: string;
  lastSyncStartedAt?: Date;
  lastSyncSucceededAt?: Date;
  error?: string;
} | null> => {
  const log = await getSynchronizationLog(sourceName, targetName);

  if (!log) {
    return null;
  }

  return {
    status: log.status,
    lastSyncStartedAt: log.lastSyncStartedAt,
    lastSyncSucceededAt: log.lastSyncSucceededAt,
    error: log.error,
  };
};

export const getAllSyncLogs = async (): Promise<
  Array<{
    sourceName: string;
    targetName: string;
    status: string;
    lastSyncStartedAt?: Date;
    lastSyncSucceededAt?: Date;
    error?: string;
  }>
> => {
  return (await getAllSynchronizationLogs()).map((log) => ({
    sourceName: log.sourceName,
    targetName: log.targetName,
    status: log.status,
    lastSyncStartedAt: log.lastSyncStartedAt,
    lastSyncSucceededAt: log.lastSyncSucceededAt,
    error: log.error,
  }));
};

async function exportViewToTempFile(
  tmpPath: string,
  view: string,
  syncId: string,
  columnMappings?: ColumnMapping[],
  chunkConfigs?: [] | [ChunkConfig] | [ChunkConfig, ChunkConfig],
): Promise<{ tmpPath: string; rowsWritten: number }> {
  let rowsWritten = 0;
  const writeStream = fs.createWriteStream(tmpPath, { encoding: 'utf8' });

  const streamPromise = new Promise<void>((resolve, reject) => {
    writeStream.on('finish', () => {
      resolve();
    });
    writeStream.on('error', (err) => {
      reject(err);
    });
  });

  if (!chunkConfigs || chunkConfigs.length === 0) {
    await executeStreamingQuery(
      `SELECT * FROM ${view}`,
      (row, request) => {
        const transformedRow = transformRowData(row, columnMappings);
        const ok = writeStream.write(JSON.stringify(transformedRow) + '\n');

        if (!ok) {
          request.pause();
          writeStream.once('drain', () => {
            request.resume();
          });
        }
        rowsWritten++;
      },
      (err) => {
        error(`[${syncId}] Error exporting view ${view}:`, err);
        writeStream.end();
      },
      () => {
        writeStream.end();
      },
    );
  } else if (chunkConfigs.length === 1) {
    const chunkConfig = chunkConfigs[0];
    const values = await chunkConfig.values();

    for (let i = 0; i < values.length; i++) {
      const value = values[i];
      const isLastIteration = i === values.length - 1;
      const query = `SELECT * FROM ${view} WHERE ${chunkConfig.columnName} ${chunkConfig.operator} '${value}'`;

      await executeStreamingQuery(
        query,
        (row, request) => {
          const transformedRow = transformRowData(row, columnMappings);
          const ok = writeStream.write(JSON.stringify(transformedRow) + '\n');

          if (!ok) {
            request.pause();
            writeStream.once('drain', () => {
              request.resume();
            });
          }
          rowsWritten++;
        },
        (err) => {
          error(
            `[${syncId}] Error exporting view ${view} for value ${value}:`,
            err,
          );
          writeStream.end();
        },
        () => {
          if (isLastIteration) {
            writeStream.end();
          }
        },
      );
    }
  } else if (chunkConfigs.length === 2) {
    const [firstChunk, secondChunk] = chunkConfigs;
    const firstValues = await firstChunk.values();
    const secondValues = await secondChunk.values();

    for (let i = 0; i < firstValues.length; i++) {
      const firstValue = firstValues[i];

      for (let j = 0; j < secondValues.length; j++) {
        const secondValue = secondValues[j];
        const isLastIteration =
          i === firstValues.length - 1 && j === secondValues.length - 1;

        const query = `SELECT * FROM ${view} WHERE ${firstChunk.columnName} ${firstChunk.operator} '${firstValue}' AND ${secondChunk.columnName} ${secondChunk.operator} '${secondValue}'`;

        await executeStreamingQuery(
          query,
          (row, request) => {
            const transformedRow = transformRowData(row, columnMappings);
            const ok = writeStream.write(JSON.stringify(transformedRow) + '\n');

            if (!ok) {
              request.pause();
              writeStream.once('drain', () => {
                request.resume();
              });
            }
            rowsWritten++;
          },
          (err) => {
            error(
              `[${syncId}] Error exporting view ${view} for values ${firstValue}, ${secondValue}:`,
              err,
            );
            writeStream.end();
          },
          () => {
            if (isLastIteration) {
              writeStream.end();
            }
          },
        );
      }
    }
  }

  await streamPromise;

  return { tmpPath, rowsWritten };
}

async function importFileToTableOverHttp(
  tmpPath: string,
  target: string,
  database: string,
  clickHouseUrl: string,
  username: string,
  password: string,
): Promise<void> {
  const query = `INSERT INTO ${database}.${target} FORMAT JSONEachRow`;
  const parsedUrl = new URL(clickHouseUrl);

  const auth =
    username && password
      ? Buffer.from(`${username}:${password}`).toString('base64')
      : '';

  const options = {
    hostname: parsedUrl.hostname,
    path: `/?query=${encodeURIComponent(query)}`,
    port: parsedUrl.port,
    method: 'POST',
    headers: {
      'Content-Type': 'application/x-ndjson',
      'Transfer-Encoding': 'chunked',
      ...(auth && { Authorization: `Basic ${auth}` }),
    },
  };

  return new Promise<void>((resolve, reject) => {
    let errorMessage: string | undefined;
    const req = http.request(options, (res) => {
      res.on('data', (chunk) => {
        if (!isHttpSuccessfulResponse(res.statusCode)) errorMessage = chunk;
      });

      res.on('end', () => {
        if (!isHttpSuccessfulResponse(res.statusCode)) {
          reject(new Error(`HTTP response error: ${errorMessage}`));
        } else {
          resolve();
        }
      });
    });

    req.on('error', (error) => {
      reject(new Error(`HTTP request error: ${error?.message}`));
    });

    const fileStream = fs.createReadStream(tmpPath);

    fileStream.on('error', (error) => {
      req.destroy();
      reject(new Error(`File read error: ${error.message}`));
    });

    fileStream.on('end', () => {
      req.end();
    });

    fileStream.pipe(req);
  });
}

function transformRowData(row: any, columnMappings?: ColumnMapping[]): any {
  if (!columnMappings || columnMappings.length === 0) {
    return row;
  }

  const transformedRow = { ...row };

  for (const mapping of columnMappings) {
    const { columnName, transformFunction } = mapping;

    if (transformedRow.hasOwnProperty(columnName)) {
      transformedRow[columnName] = transformFunction(
        transformedRow[columnName],
      );
    }
  }

  return transformedRow;
}

export async function syncViewToTable(config: SyncConfig): Promise<SyncResult> {
  const startTime = new Date();
  const syncId = generateSyncId();

  info(`[${syncId}] Starting sync: ${config.source} -> ${config.target}`);

  let tempFilePath = join(
    tmpdir(),
    `${syncId}_${config.source.replace(/[^a-zA-Z0-9_]/g, '_')}.json`,
  );
  registerTempFile(tempFilePath);

  try {
    await ensureConnection();

    await upsertSynchronizationLog({
      sourceName: config.source,
      targetName: config.target,
      status: SyncStatusEnum.IN_PROGRESS,
      lastSyncStartedAt: new Date(),
    });

    // const tmpPath =
    //   '/tmp/sync_1760954255447_reporting_R_PG_Absences_Per_Month.json';
    // const rowsWritten = 696969;

    const { tmpPath, rowsWritten } = await exportViewToTempFile(
      tempFilePath,
      config.source,
      syncId,
      config.columnMappings,
      config.chunkConfigs,
    );

    await truncateTable(config.target);

    await importFileToTableOverHttp(
      tmpPath,
      config.target,
      ENV_CONFIG.CLICKHOUSE.NAME,
      ENV_CONFIG.CLICKHOUSE.URL,
      ENV_CONFIG.CLICKHOUSE.USERNAME,
      ENV_CONFIG.CLICKHOUSE.PASSWORD,
    );

    await upsertSynchronizationLog({
      sourceName: config.source,
      targetName: config.target,
      status: SyncStatusEnum.SYNCHRONIZED,
      lastSyncSucceededAt: new Date(),
    });

    return createSyncResult(true, rowsWritten, rowsWritten, startTime);
  } catch (error) {
    await upsertSynchronizationLog({
      sourceName: config.source,
      targetName: config.target,
      status: SyncStatusEnum.FAILED,
      error: `${error?.message}${
        error?.stack ? `\nStack: ${error.stack}` : ''
      }`,
    });

    throw error;
  } finally {
    await safeUnlink(tempFilePath);
  }
}

export async function showAllSyncLogs(): Promise<void> {
  try {
    info('📊 Fetching all synchronization logs...');
    const logs = await getAllSyncLogs();

    if (logs.length === 0) {
      info('No synchronization logs found.');
      return;
    }

    info(`Found ${logs.length} synchronization log(s):`);
    logs.forEach((log, index) => {
      info(`\n${index + 1}. ${log.sourceName} -> ${log.targetName}`);
      info(`   Status: ${log.status}`);
      info(
        `   Last Sync Started: ${
          log.lastSyncStartedAt ? log.lastSyncStartedAt.toISOString() : 'Never'
        }`,
      );
      info(
        `   Last Sync Succeeded: ${
          log.lastSyncSucceededAt
            ? log.lastSyncSucceededAt.toISOString()
            : 'Never'
        }`,
      );
      if (log.error) {
        info(`   Error: ${log.error}`);
      }
    });
  } catch (error) {
    info('❌ Error fetching sync logs:', error);
  }
}
