import { RIFlexFieldValueDto } from '@domain/ri-flex-field-value/routes/ri-flex-field-value/ri-flex-field-value.dto';
import { RIProcedureDto } from '@domain/ri-procedure/routes/ri-procedure/ri-procedure.dto';
import { IsNotEmpty } from 'class-validator';
import { RIInstitution } from '../../ri-institution.entity';

export class RIInstitutionDTO extends RIInstitution {
    RIInstitutionID: number;

    @IsNotEmpty()
    Abbreviation: string;

    @IsNotEmpty()
    Name: string;

    // Institution: InstitutionDto;

    InstitutionID: number;

    Bulstat: string;

    @IsNotEmpty()
    BaseSchoolType: any;

    RIProcedure: RIProcedureDto;

    TRAddress: string;

    TRPostCode: number;

    ReligInstDetails: string;

    HeadFirstName: string;

    HeadMiddleName: string;

    HeadLastName: string;

    IsNational: boolean;

    PersonnelCount: number;

    AuthProgram: string;

    IsDataDue: boolean;

    RIFlexFieldValues: RIFlexFieldValueDto[];
}
