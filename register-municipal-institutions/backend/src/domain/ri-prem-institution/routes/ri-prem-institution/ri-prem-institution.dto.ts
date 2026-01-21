import { RIProcedure } from '@domain/ri-procedure/ri-procedure.entity';
import { RIPremInstitution } from '../../ri-prem-institution.entity';

export class RIPremInstitutionDTO extends RIPremInstitution {
    PremInstitutionID: number;

    PremDocs: string;

    PremStudents: string;

    PremInventory: string;

    RIProcedure: RIProcedure;
}
