import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';
import { AuthState } from '@shared/business-object-model/interfaces/auth-state.interface';

// this stores the logged in user in the angular project
export function createInitialState(): AuthState {
    return { authReady: false } as AuthState;
}

@Injectable({
    providedIn: 'root',
})
@StoreConfig({ name: 'session' })
export class AuthStore extends Store<AuthState> {
    constructor() {
        super(createInitialState());
    }
}
