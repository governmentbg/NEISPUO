import { Component, OnDestroy } from '@angular/core';

import { Subscription } from 'rxjs';
import { SpinnerService } from '../../services/spinner-service';

@Component({
    selector: 'app-spinner',
    templateUrl: './spinner.component.html',
    styleUrls: ['./spinner.component.scss'],
})
export class SpinnerComponent implements OnDestroy {
    subscription: Subscription;

    display: boolean = false;

    constructor(private spinnerService: SpinnerService) {
        this.subscription = this.spinnerService.displaySpinnerEventEmitter.subscribe((item) =>
            this.displaySpinner(item),
        );
    }

    displaySpinner(display: boolean) {
        this.display = display;
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }
}
