import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { GuideName } from './constants/user-tour-guide.constants';
import { UserGuideState, UserTourStore } from './user-tour-guide.store';

@Injectable({
  providedIn: 'root',
})
export class UserTourGuideQuery extends Query<UserGuideState> {
  constructor(protected store: UserTourStore) {
    super(store);
  }

  isFirstTimeGuide(name: GuideName): boolean {
    return this.store.getValue()[name];
  }

  setGuideStarted(name: GuideName) {
    this.store.update({
      ...this.store.getValue(),
      [name]: false,
    });
  }
}
