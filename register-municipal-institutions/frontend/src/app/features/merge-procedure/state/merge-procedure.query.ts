import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { MergeProcedureState, MergeProcedureStore } from './merge-procedure.store';

@Injectable()
export class MergeProcedureQuery extends Query<MergeProcedureState> {
  activeMIs$ = this.select('activeMIs');

  misToDelete$ = this.select('misToDelete');

  miToCreate$ = this.select('miToCreate');

  constructor(protected store: MergeProcedureStore) {
    super(store);
  }
}
