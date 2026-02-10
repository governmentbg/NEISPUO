import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { JoinProcedureState, JoinProcedureStore } from './join-procedure.store';

@Injectable()
export class JoinProcedureQuery extends Query<JoinProcedureState> {
  activeMIs$ = this.select('activeMIs');

  misToDelete$ = this.select('misToDelete');

  miToUpdate$ = this.select('miToUpdate');

  constructor(protected store: JoinProcedureStore) {
    super(store);
  }
}
