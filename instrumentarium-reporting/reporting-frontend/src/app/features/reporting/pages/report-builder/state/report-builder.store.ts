import { Store, StoreConfig } from '@datorama/akita';

export interface ReportBuilderState extends Store {
  report: any;
}

export function initiateState() {
  return {
    report: {}
  } as ReportBuilderState;
}

@StoreConfig({ name: 'report' })
export class ReportBuilderStore extends Store<ReportBuilderState> {
  constructor() {
    super(initiateState());
  }
}
