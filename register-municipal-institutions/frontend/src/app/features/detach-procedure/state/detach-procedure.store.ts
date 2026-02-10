import { Store, StoreConfig } from '@datorama/akita';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';

export interface DetachProcedureState extends Store {

  /** разделяне */
  activeMIs: MunicipalInstitution[];
  miToUpdate: MunicipalInstitution;
  misToCreate: MunicipalInstitution[];

}

export function initiateState() {
  return {
    activeMIs: [],
    miToUpdate: null,
    misToCreate: [],
  } as DetachProcedureState;
}

@StoreConfig({ name: 'detach-procedure' })
export class DetachProcedureStore extends Store<DetachProcedureState> {
  constructor() {
    super(initiateState());
  }
}
