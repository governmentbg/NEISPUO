import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { ProcedureState, ProcedureStore } from './procedure.store';

@Injectable()
export class ProcedureQuery extends Query<ProcedureState> {
  divideMiToDelete$ = this.select('divideMiToDelete');

  divideMisToCreate$ = this.select('divideMisToCreate');

  mergeMiToCreate$ = this.select('mergeMiToCreate');

  availableMis$ = this.select('availableMIs');

  constructor(protected store: ProcedureStore) {
    super(store);
  }
}
