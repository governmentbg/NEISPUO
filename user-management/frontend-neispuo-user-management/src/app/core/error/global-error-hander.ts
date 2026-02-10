import { ErrorHandler, Injectable, NgZone } from '@angular/core';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
    constructor(private zone: NgZone) {}

    handleError(error: Error) {
        console.error(`[GLOBAL EXCEPTION HANDLER]: ${error?.message}`);
    }
}
