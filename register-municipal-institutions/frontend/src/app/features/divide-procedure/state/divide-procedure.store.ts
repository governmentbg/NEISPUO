import { Store, StoreConfig } from '@datorama/akita';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';

export interface DivideProcedureState extends Store {

  /** разделяне */
  activeMIs: MunicipalInstitution[];
  miToDelete: MunicipalInstitution;
  misToCreate: MunicipalInstitution[];

}

export function initiateState() {
  return {
    activeMIs: [],
    miToDelete: null,
    misToCreate: [],
  } as DivideProcedureState;
}

@StoreConfig({ name: 'divide-procedure' })
export class DivideProcedureStore extends Store<DivideProcedureState> {
  constructor() {
    super(initiateState());
  }
}
