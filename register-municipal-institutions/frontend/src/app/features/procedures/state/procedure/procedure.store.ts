import { Store, StoreConfig } from '@datorama/akita';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';

export interface ProcedureState extends Store {

  /** разделяне */
  divideMiToDelete: MunicipalInstitution;
  divideMisToCreate: MunicipalInstitution[];

  /** отделяне */
  detachMiToUpdate: MunicipalInstitution;
  detachMisToCreate: MunicipalInstitution[];

  /** вливане */
  joinMisToDelete: MunicipalInstitution[];
  joinMiToUpdate: MunicipalInstitution;

  /** сливане */
  mergeMisToDelete: MunicipalInstitution[];
  mergeMiToCreate: MunicipalInstitution;

  availableMIs: MunicipalInstitution[];

}

export function initiateState() {
  return {
    joinMisToDelete: [],
    joinMiToUpdate: {},
    divideMisToCreate: [],
    availableMIs: [],
    detachMisToCreate: [],
    mergeMiToCreate: {},

  } as ProcedureState;
}

@StoreConfig({ name: 'procedure' })
export class ProcedureStore extends Store<ProcedureState> {
  constructor() {
    super(initiateState());
  }
}
