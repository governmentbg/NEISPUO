import { Injectable } from '@angular/core';
import { EntityState, EntityStore, StoreConfig } from '@datorama/akita';

export interface UserState extends EntityState<number[]> {}

@Injectable({
  providedIn: 'root'
})
@StoreConfig({ name: 'users' })
export class UserStore extends EntityStore<UserState, number[]> {
  constructor() {
    super();
  }
}
