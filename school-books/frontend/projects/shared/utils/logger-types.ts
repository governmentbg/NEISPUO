import type { StackFrame } from 'stacktrace-js';

export type Payload = {
  message: string;
  stackFrames: StackFrame[] | null;
  appVersion: string;
  requestId: string | null;
  sysUserId: number | null;
  sessionId: string | null;
};
