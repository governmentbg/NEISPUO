import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';

export interface NewModuleInfoState {
  showNewModuleInfoModal: boolean;
}

export function createInitialState(): NewModuleInfoState {
  return {
    showNewModuleInfoModal: false
  };
}

@Injectable({
  providedIn: 'root'
})
@StoreConfig({ name: 'new-module-info' })
export class NewModuleInfoStore extends Store<NewModuleInfoState> {
  constructor() {
    super(createInitialState());
  }
}
