export type Payload = {
  message: string;
  stackFrames: StackFrame[] | null;
  appVersion: string;
  requestId: string | null;
  sysUserId: number | null;
  sessionId: string | null;
};

export enum LOG_LEVEL {
  DEBUG = 'debug',
  INFO = 'info',
  WARN = 'warn',
  ERROR = 'error'
}

export type LogEvent = {
  level: LOG_LEVEL;
  event: string;
  payload: Payload & {
    timestamp: string;
  };
};
