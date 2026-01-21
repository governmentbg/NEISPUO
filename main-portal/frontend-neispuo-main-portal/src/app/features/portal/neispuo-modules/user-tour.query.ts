
import { Injectable } from "@angular/core";
import { Query } from "@datorama/akita";
import { UserTourState, UserTourStore } from "./user-tour.store";


@Injectable({
  providedIn: 'root'
})
export class UserTourQuery extends Query<UserTourState> {
  startUserTour$ = this.select('startUserTour');

  constructor(protected store: UserTourStore) {
    super(store);
  }

  updateValue(value: boolean) {
    this.store.update({ startUserTour: value });
  }
}