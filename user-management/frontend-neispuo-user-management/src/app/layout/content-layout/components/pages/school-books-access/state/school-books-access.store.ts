import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';

export interface SchoolBooksAccessState {
    personId: number;
    institutionId: number;
    assignedSchoolBooks: any[];
}
export function createInitialState(): SchoolBooksAccessState {
    return {} as SchoolBooksAccessState;
}

@Injectable({
    providedIn: 'root',
})
@StoreConfig({ name: 'session' })
export class SchoolBooksAccessStore extends Store<SchoolBooksAccessState> {
    constructor() {
        super(createInitialState());
    }
}
