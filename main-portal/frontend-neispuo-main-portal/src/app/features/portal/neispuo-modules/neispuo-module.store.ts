import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';
import { NeispuoCategory } from './neispuo-category.interface';
import { NeispuoModule } from './neispuo-module.interface';
import { NeispuoUserGuide } from './neispuo-user-guide.interface';

export interface NeispuoModuleState {
  modules: NeispuoModule[];
  categories: NeispuoCategory[];
  userGuides: NeispuoUserGuide[];
  loading: boolean;
  error: any;
}

export function createInitialState(): NeispuoModuleState {
  return {
    modules: [],
    categories: [],
    userGuides: [],
    loading: true,
    error: {}
  };
}

@Injectable({
  providedIn: 'root'
})
@StoreConfig({ name: 'neispuo-modules' })
export class NeispuoModuleStore extends Store<NeispuoModuleState> {
  constructor() {
    super(createInitialState());
  }
}
