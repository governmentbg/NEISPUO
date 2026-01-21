import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';

export interface IsUserGuideClickedState {
  isUserGuideClicked: boolean;
}

export function createInitialState(): IsUserGuideClickedState {
  return {
    isUserGuideClicked: false
  };
}

@Injectable({
  providedIn: 'root'
})
@StoreConfig({ name: 'is-user-guide-clicked' })
export class IsUserGuideClickedStore extends Store<IsUserGuideClickedState> {
  constructor() {
    super(createInitialState());
  }
}
