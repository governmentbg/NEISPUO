import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';
import { GuideName } from './constants/user-tour-guide.constants';

export type UserGuideState = Record<GuideName.DASHBOARD_MENU, boolean>;

export function initialState(): UserGuideState {
  return {
    [GuideName.DASHBOARD_MENU]: true,
  };
}

@Injectable({
  providedIn: 'root',
})
@StoreConfig({ name: 'user-tour-guide' })
export class UserTourStore extends Store<UserGuideState> {
  constructor() {
    super(initialState());
  }
}
