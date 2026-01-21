import { ViewEntity } from 'typeorm';
import { RIInstitution } from './ri-institution.entity';

@ViewEntity({ schema: 'reginst_basic', name: 'RIInstitutionLatestActive' })
export class RIInstitutionLatestActive extends RIInstitution {}
