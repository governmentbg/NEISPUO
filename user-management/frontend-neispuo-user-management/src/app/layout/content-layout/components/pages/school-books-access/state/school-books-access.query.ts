import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { SchoolBooksAccessState, SchoolBooksAccessStore } from './school-books-access.store';

@Injectable({
    providedIn: 'root',
})
export class SchoolBooksAccessQuery extends Query<SchoolBooksAccessState> {
    public readonly personalSchoolBooks$ = this.select('assignedSchoolBooks');

    public readonly selectedUser$ = this.select((state) => state.personId);

    constructor(protected store: SchoolBooksAccessStore) {
        super(store);
    }
}
