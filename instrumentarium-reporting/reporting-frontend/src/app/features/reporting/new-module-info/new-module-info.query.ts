import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { NewModuleInfoState, NewModuleInfoStore } from './new-module-info.store';

@Injectable({
  providedIn: 'root'
})
export class NewModuleInfoQuery extends Query<NewModuleInfoState> {
  showNewModuleInfoModal$ = this.select('showNewModuleInfoModal');

  constructor(protected override store: NewModuleInfoStore) {
    super(store);
  }

  updateValue(value: boolean) {
    this.store.update({ showNewModuleInfoModal: value });
  }
}
