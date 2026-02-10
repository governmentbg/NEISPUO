import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { DivideProcedureState, DivideProcedureStore } from './divide-procedure.store';

@Injectable()
export class DivideProcedureQuery extends Query<DivideProcedureState> {
  activeMIs$ = this.select('activeMIs');

  miToDelete$ = this.select('miToDelete');

  misToCreate$ = this.select('misToCreate');

  constructor(protected store: DivideProcedureStore) {
    super(store);
  }
}
