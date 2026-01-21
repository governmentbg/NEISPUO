import { Store, StoreConfig } from '@datorama/akita';


export interface AvailableReportsState extends Store {

  availableReports: any[any];
}

export function initiateState() {
  return {
    availableReports: []

  } as AvailableReportsState;
}

@StoreConfig({ name: 'available-reports' })
export class AvailableReportsStore extends Store<AvailableReportsState> {
  constructor() {
    super(initiateState());
  }
}
