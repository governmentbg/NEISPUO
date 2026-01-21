import mssql from 'mssql';
import express, { Request } from 'express';
import StackTraceGPS from 'stacktrace-gps';
import fetch from 'node-fetch';
import { LogEvent } from './logger-types.js';
import morgan from 'morgan';

const env = {
  NODE_ENV: process.env.NODE_ENV || 'Development',
  PORT: process.env.PORT || 3001,
  DB_USERNAME: process.env.SB__Data__DbUser || throwMissingEnvironment('SB__Data__DbUser'),
  DB_PASS: process.env.SB__Data__DbPass || throwMissingEnvironment('SB__Data__DbPass'),
  DB_NAME: process.env.SB__Data__DbName || throwMissingEnvironment('SB__Data__DbName'),
  DB_IP: process.env.SB__Data__DbIP || throwMissingEnvironment('SB__Data__DbIP'),
  SOURCEMAPS_URL: process.env.SB__Logger__SourcemapsUrl || 'http://localhost:4200',
  APP_NAME: process.env.SB__Logger__App ?? 'frontent-logger-sink',
  POD_NAME: process.env.SB__Logger__Pod ?? null
};

const app = express();

const sqlConfig: mssql.config = {
  user: env.DB_USERNAME,
  password: env.DB_PASS,
  database: env.DB_NAME,
  server: env.DB_IP,
  options: {
    encrypt: false
  }
};

interface TypedRequestBody<T> extends Request {
  body: T;
}

type LoggerRequest = TypedRequestBody<{ events: Array<LogEvent> }>;

app.use(morgan('combined'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));

app.set('trust proxy', true);

const gps = new StackTraceGPS({
  ajax: async (location) => {
    const url = new URL(location);
    const urlPath = url.pathname;
    const response = await fetch(env.SOURCEMAPS_URL + urlPath);
    if (!response.ok) {
      throw new Error(`Failed to retrieve file. Status code: ${response.status}`);
    }
    return await response.text();
  }
});

// Route to handle incoming logs
app.post('/logger', async (req: LoggerRequest, res) => {
  const clientIP = req.ip;
  const requestPath = req.headers['referer'] ?? null;

  for (let i = 0; i < req.body.events.length; i++) {
    const event = req.body.events[i];

    const { timestamp, message, stackFrames, requestId, sysUserId, sessionId, appVersion } = event.payload;

    let stackTraceString: string | null = null;

    if (stackFrames && stackFrames.length > 0) {
      const promises = stackFrames.map((frame: StackFrame) => gps.pinpoint(frame));
      const mappedStackFrames = await Promise.allSettled(promises);
      stackTraceString = mappedStackFrames
        .map((entry, index) =>
          entry.status === 'fulfilled'
            ? `    at ${entry.value.functionName} (${entry.value.fileName}:${entry.value.lineNumber}:${entry.value.columnNumber})`
            : stackFrames[index]?.source
        )
        .join('\n');
    }

    await saveInDb(
      timestamp,
      message,
      stackTraceString,
      clientIP,
      appVersion,
      requestId,
      sysUserId,
      sessionId,
      requestPath,
      env.APP_NAME,
      env.POD_NAME
    );
  }

  // Process or store the log data as needed
  res.sendStatus(200); // Send a response to the client
});

// Start the server
const server = app.listen(env.PORT, () => {
  console.log(`Server is running on port ${env.PORT}`);
});

// Trap exit signals and exit gracefully
const shutdown = () => {
  console.log('Stopping ...');
  server.close(() => {
    console.log('Stopped');
  });
};
process.on('SIGINT', shutdown);
process.on('SIGTERM', shutdown);

async function saveInDb(
  timestamp: string,
  message: string,
  stackTrace: string | null,
  clientIP: string | undefined,
  version: string,
  requestId: string | null,
  sysUserId: number | null,
  sessionId: string | null,
  requestPath: string | null,
  appName: string,
  podName: string | null
) {
  const date = new Date(parseInt(timestamp));

  const sql = `
  INSERT INTO [logs].[SchoolBooksLog2]
           ([DateUtc]
            ,[Level]
            ,[MessageTemplate]
            ,[Exception]
            ,[App]
            ,[AppVersion]
            ,[Pod]
            ,[IpAddress]
            ,[RequestPath]
            ,[RequestId]
            ,[SysUserId]
            ,[SessionId])
     VALUES
           (@date
           ,'Error'
           ,@message
           ,@stackTrace
           ,@appName
           ,@version
           ,@podName
           ,@clientIP
           ,@requestPath
           ,@requestId
           ,@sysUserId
           ,@sessionId)
  `;

  const pool = await mssql.connect(sqlConfig);
  await pool
    .request()
    .input('date', mssql.DateTime2, date.toISOString())
    .input('message', mssql.NVarChar, message)
    .input('stackTrace', mssql.NVarChar, stackTrace)
    .input('appName', mssql.NVarChar, truncateString(appName, 100))
    .input('version', mssql.NVarChar, truncateString(version, 50))
    .input('podName', mssql.NVarChar, truncateString(podName, 100))
    .input('clientIP', mssql.NVarChar, truncateString(clientIP ?? '', 50))
    .input('requestPath', mssql.NVarChar, requestPath)
    .input('requestId', mssql.NVarChar, truncateString(requestId, 50))
    .input('sysUserId', mssql.Int, sysUserId)
    .input('sessionId', mssql.NVarChar, truncateString(sessionId, 50))
    .query(sql)
    .catch((err) => {
      console.log(err);
    });
}

function truncateString(value: string | null, maxChars: number): string | null {
  if (value == null) {
    return null;
  }

  return value.length <= maxChars ? value : value.substring(0, maxChars);
}

function throwMissingEnvironment(variableName: string): never {
  throw new Error(`Missing environment variable: ${variableName}`);
}
