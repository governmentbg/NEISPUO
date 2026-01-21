import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CONSTANTS } from '@shared/constants';

@Component({
    selector: 'app-export-button',
    templateUrl: './export-button.component.html',
    styleUrls: ['./export-button.component.scss'],
})
export class ExportButtonComponent {
    CONSTANTS = CONSTANTS;

    @Input()
    loading: boolean = false;

    @Output()
    // eslint-disable-next-line @angular-eslint/no-output-on-prefix
    onClick = new EventEmitter<any>();

    onClickButton(event: any) {
        this.onClick.emit(event);
    }
}
