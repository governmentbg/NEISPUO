import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { RouterQuery } from '@datorama/akita-ng-router-store';
import {
  switchMap, map, filter,
} from 'rxjs/operators';
import { NeispuoModuleState, NeispuoModuleStore } from './neispuo-module.store';

@Injectable({
  providedIn: 'root',
})
export class NeispuoModuleQuery extends Query<NeispuoModuleState> {
  selectedCategory$ = this.routerQuery.selectParams('categoryId')
    .pipe(
      filter((categoryId) => !!categoryId),
      switchMap((categoryId) => this.select('categories')
        .pipe(
          filter((categories) => categories.length > 0),

          map((categories) => categories.find((c) => +c.id === +categoryId)),
        )),
    );

  categories$ = this.select('categories');

  modules$ = this.routerQuery.selectParams('modules')
    .pipe(() => this.select('modules'));

  constructor(protected store: NeispuoModuleStore, private routerQuery: RouterQuery) {
    super(store);
  }
}
