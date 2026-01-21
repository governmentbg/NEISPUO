import { Injectable } from "@angular/core";
import { IsUserGuideClickedState, IsUserGuideClickedStore } from "./is-user-guide-clicked.store";
import { Query } from '@datorama/akita';

@Injectable({
  providedIn: 'root'
})
export class IsUserGuideClickedQuery extends Query<IsUserGuideClickedState> {
  isUserGuideClicked$ = this.select('isUserGuideClicked');

  constructor(protected store: IsUserGuideClickedStore) {
    super(store);
  }

  updateValue() {
    this.store.update({ isUserGuideClicked: true });
  }
}