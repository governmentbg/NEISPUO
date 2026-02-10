import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';
import { NeispuoCategory } from './neispuo-category.interface';
import { NeispuoModule } from './neispuo-module.interface';

export interface NeispuoModuleState {
  modules: NeispuoModule[];
  categories: NeispuoCategory[];
}

export function createInitialState(): NeispuoModuleState {
  return {
    modules: [],
    categories: [],
  };
}

@Injectable({
  providedIn: 'root',
})
@StoreConfig({ name: 'neispuo-modules' })
export class NeispuoModuleStore extends Store<NeispuoModuleState> {
  constructor() {
    super(createInitialState());
  }
}
