import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';

export interface UserTourState {
  startUserTour: boolean;
}

export function createInitialState(): UserTourState {
  return {
    startUserTour: false
  };
}

@Injectable({
  providedIn: 'root'
})
@StoreConfig({ name: 'start-user-tour' })
export class UserTourStore extends Store<UserTourState> {
  constructor() {
    super(createInitialState());
  }
}