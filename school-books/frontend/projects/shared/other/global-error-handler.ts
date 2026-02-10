import { HttpErrorResponse } from '@angular/common/http';
import { ErrorHandler } from '@angular/core';
import type * as BeaverLogger from 'beaver-logger';
import type { AuthService } from 'projects/shared/services/auth.service';
import type { EventService } from 'projects/shared/services/event.service';
import { getRequestId, getUnexpectedErrorMessage } from 'projects/shared/utils/error';
import { environment } from 'src/environments/environment';
import type * as StackTrace from 'stacktrace-js';
import { Payload } from '../utils/logger-types';

type BeaverLogger = typeof BeaverLogger;
type StackTrace = typeof StackTrace;

export class GlobalErrorHandler implements ErrorHandler {
  static instance = new GlobalErrorHandler();

  private initializedPromise: Promise<[typeof StackTrace, (event: string, payload: Payload) => void]>;
  private eventService?: EventService;
  private authService?: AuthService;

  constructor() {
    this.initializedPromise = Promise.all([import('stacktrace-js'), import('beaver-logger')]).then(
      ([StackTrace, BeaverLogger]: [StackTrace, BeaverLogger]) => [
        StackTrace,
        BeaverLogger.Logger({
          url: '/logger',
          logLevel: 'error',
          flushInterval: 60 * 1000,
          enableSendBeacon: true
        }).error
      ]
    );
  }

  handleError(error: unknown): void;
  handleError(error: unknown, presented: boolean): void;
  handleError(error: unknown, presented?: boolean): void {
    try {
      if (!presented) {
        const message = getUnexpectedErrorMessage(error);

        // not importing the EventType to skip module dependency
        this.eventService?.dispatch({ type: 3 /* SnackbarError */, args: { message } });
      }

      const requestId = getRequestId(error);
      const sysUserId = this.authService?.sysUserId ?? null;
      const sessionId = this.authService?.sessionId ?? null;
      const appVersion = environment.appVersion;

      this.initializedPromise.then(([StackTrace, logger]) => {
        if (error instanceof Error) {
          StackTrace.fromError(error, { offline: true }).then((stackframes) => {
            const splicedStackframes = stackframes.splice(0, 20);
            logger('Error', {
              message: error.message,
              stackFrames: splicedStackframes,
              appVersion,
              requestId,
              sysUserId,
              sessionId
            });
          });
        } else if (error instanceof HttpErrorResponse) {
          logger('HttpErrorResponse', {
            message: error.message,
            stackFrames: null,
            appVersion,
            requestId,
            sysUserId,
            sessionId
          });
        } else if (error instanceof Response) {
          logger('ResponseError', {
            message: error.statusText,
            stackFrames: null,
            appVersion,
            requestId,
            sysUserId,
            sessionId
          });
        } else {
          logger('UnhandledError', {
            message: 'Unhandled error',
            stackFrames: null,
            appVersion,
            requestId,
            sysUserId,
            sessionId
          });
        }
      });
    } catch (err) {
      // this method should never throw or we can end up in an infinite loop!!!
      console.log(err);
    }

    console.error(error);
  }

  registerServices(eventService: EventService, authService: AuthService) {
    this.eventService = eventService;
    this.authService = authService;
  }
}
