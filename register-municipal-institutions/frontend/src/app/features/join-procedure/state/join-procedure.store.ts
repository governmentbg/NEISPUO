import { Store, StoreConfig } from '@datorama/akita';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';

export interface JoinProcedureState extends Store {

  /** вливане */
  activeMIs: MunicipalInstitution[];
  misToDelete: MunicipalInstitution[];
  miToUpdate: MunicipalInstitution;

}

export function initiateState() {
  return {
    activeMIs: [],
    misToDelete: [],
    miToUpdate: null,
  } as JoinProcedureState;
}

@StoreConfig({ name: 'join-procedure' })
export class JoinProcedureStore extends Store<JoinProcedureState> {
  constructor() {
    super(initiateState());
  }
}
