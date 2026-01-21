import { Request } from 'mssql';
import { SynchronizationLog } from 'src/interfaces/synchronization-log.interface';
import { SyncStatusEnum } from '../../../enums/sync-status.enum';
import { msSqlClient } from '../mssql-client';

export const upsertSynchronizationLog = async (
  entry: Partial<SynchronizationLog>,
): Promise<void> => {
  const request = new Request(msSqlClient);

  const query = `
    MERGE reporting.SynchronizationLog AS target
    USING (SELECT @SourceName, @TargetName) AS source (SourceName, TargetName)
    ON target.SourceName = source.SourceName AND target.TargetName = source.TargetName
    WHEN MATCHED THEN
      UPDATE SET 
        Status = @Status,
        LastSyncStartedAt = CASE 
          WHEN @Status = '${SyncStatusEnum.IN_PROGRESS}' THEN @LastSyncStartedAt 
          ELSE target.LastSyncStartedAt 
        END,
        LastSyncSucceededAt = CASE 
          WHEN @Status = '${SyncStatusEnum.SYNCHRONIZED}' THEN @LastSyncSucceededAt 
          ELSE target.LastSyncSucceededAt 
        END,
        Error = @Error
    WHEN NOT MATCHED THEN
      INSERT (SourceName, TargetName, Status, LastSyncStartedAt, LastSyncSucceededAt, Error)
      VALUES (@SourceName, @TargetName, @Status, 
        CASE 
          WHEN @Status = '${SyncStatusEnum.IN_PROGRESS}' THEN @LastSyncStartedAt 
          ELSE NULL 
        END,
        CASE 
          WHEN @Status = '${SyncStatusEnum.SYNCHRONIZED}' THEN @LastSyncSucceededAt 
          ELSE NULL 
        END, @Error);
  `;

  request.input('SourceName', entry.sourceName);
  request.input('TargetName', entry.targetName);
  request.input('Status', entry.status);
  request.input('LastSyncStartedAt', entry.lastSyncStartedAt || new Date());
  request.input('LastSyncSucceededAt', entry.lastSyncSucceededAt || new Date());
  request.input('Error', entry.error || null);

  await request.query(query);
};

export const getSynchronizationLog = async (
  sourceName: string,
  targetName: string,
): Promise<SynchronizationLog | null> => {
  const request = new Request(msSqlClient);

  const query = `
    SELECT 
      SynchronizationLogID,
      SourceName as sourceName,
      TargetName as targetName,
      Status as status,
      LastSyncStartedAt as lastSyncStartedAt,
      LastSyncSucceededAt as lastSyncSucceededAt,
      Error as error
    FROM reporting.SynchronizationLog
    WHERE SourceName = @SourceName AND TargetName = @TargetName
  `;

  request.input('SourceName', sourceName);
  request.input('TargetName', targetName);

  const result = await request.query(query);

  if (result.recordset.length === 0) {
    return null;
  }

  return result.recordset[0] as SynchronizationLog;
};

export const getAllSynchronizationLogs = async (): Promise<
  SynchronizationLog[]
> => {
  const request = new Request(msSqlClient);

  const query = `
    SELECT 
      SynchronizationLogID,
      SourceName as sourceName,
      TargetName as targetName,
      Status as status,
      LastSyncStartedAt as lastSyncStartedAt,
      LastSyncSucceededAt as lastSyncSucceededAt,
      Error as error
    FROM reporting.SynchronizationLog
    ORDER BY LastSyncStartedAt DESC
  `;

  const result = await request.query(query);
  return result.recordset as SynchronizationLog[];
};

export async function executeStreamingQuery(
  query: string,
  onRow: (row: any, request: Request) => void,
  onError: (error: any) => void,
  onDone: () => void,
): Promise<void> {
  const request = msSqlClient.request();
  request.stream = true;

  return new Promise<void>((resolve, reject) => {
    request.on('row', (row) => {
      try {
        onRow(row, request);
      } catch (err) {
        onError(err);
        try {
          // Attempt to cancel the request to stop further events
          (request as any).cancel?.();
        } catch {}
        reject(err);
      }
    });

    request.on('error', (err) => {
      onError(err);
      reject(err);
    });

    request.on('done', () => {
      onDone();
      resolve();
    });

    request.query(query);
  });
}

export async function getInstitutionIds(): Promise<string[]> {
  const request = msSqlClient.request();

  const queryResult = await request.query(
    `SELECT InstitutionID FROM [core].[Institution]`,
  );

  return queryResult.recordset.map(
    (row: { InstitutionID: string }) => row.InstitutionID,
  );
}
