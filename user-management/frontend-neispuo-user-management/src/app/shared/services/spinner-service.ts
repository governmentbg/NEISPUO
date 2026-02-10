import { EventEmitter, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class SpinnerService {
    public displaySpinnerEventEmitter: EventEmitter<boolean>;

    constructor() {
        this.displaySpinnerEventEmitter = new EventEmitter();
    }

    public displaySpinner(): void {
        this.displaySpinnerEventEmitter.emit(true);
    }

    public hideSpinner(): void {
        this.displaySpinnerEventEmitter.emit(false);
    }
}
