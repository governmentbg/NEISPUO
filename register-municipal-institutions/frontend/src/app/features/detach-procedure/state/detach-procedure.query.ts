import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { DetachProcedureState, DetachProcedureStore } from './detach-procedure.store';

@Injectable()
export class DetachProcedureQuery extends Query<DetachProcedureState> {
  activeMIs$ = this.select('activeMIs');

  miToUpdate$ = this.select('miToUpdate');

  misToCreate$ = this.select('misToCreate');

  constructor(protected store: DetachProcedureStore) {
    super(store);
  }
}
