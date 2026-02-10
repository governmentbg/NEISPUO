import { Component, Input } from '@angular/core';
import { CONSTANTS } from '@shared/constants';

@Component({
    selector: 'app-json-view-card',
    templateUrl: './json-view-card.component.html',
    styleUrls: ['./json-view-card.component.scss'],
})
export class JsonViewCardComponent {
    CONSTANTS = CONSTANTS;

    @Input() data: unknown;

    @Input() loading = false;

    @Input() error: string | null = null;

    @Input() empty = false;
}
