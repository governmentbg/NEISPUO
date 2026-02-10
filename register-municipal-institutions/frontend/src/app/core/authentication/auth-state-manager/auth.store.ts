import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';
import { AuthState } from './auth-state.interface';

export function createInitialState(): AuthState {
  return { authReady: false } as AuthState;
}

@Injectable({
  providedIn: 'root',
})
@StoreConfig({ name: 'session' })
export class AuthStore extends Store<AuthState> {
  constructor() { super(createInitialState()); }
}
