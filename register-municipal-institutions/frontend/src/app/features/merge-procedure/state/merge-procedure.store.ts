import { Store, StoreConfig } from '@datorama/akita';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';

export interface MergeProcedureState extends Store {

  /** сливане */
  activeMIs: MunicipalInstitution[];
  misToDelete: MunicipalInstitution[];
  miToCreate: MunicipalInstitution;

}

export function initiateState() {
  return {
    activeMIs: [],
    misToDelete: [],
    miToCreate: null,
  } as MergeProcedureState;
}

@StoreConfig({ name: 'merge-procedure' })
export class MergeProcedureStore extends Store<MergeProcedureState> {
  constructor() {
    super(initiateState());
  }
}
