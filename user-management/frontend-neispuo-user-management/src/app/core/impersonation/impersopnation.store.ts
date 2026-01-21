import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';

export interface ImpersonationState {}
export function createInitialState(): ImpersonationState {
    return {} as ImpersonationState;
}

@Injectable({
    providedIn: 'root',
})
@StoreConfig({ name: 'session' })
export class ImpersonationStore extends Store<ImpersonationState> {
    constructor() {
        super(createInitialState());
    }
}
