import { ProcedureType } from '@domain/procedure-type/procedure-type.entity';
import { RICPLRArea } from '@domain/ri-cplr-area/ri-cplr-area.entity';
import { RIDocument } from '@domain/ri-document/ri-document.entity';
import { RIInstitutionDepartment } from '@domain/ri-institution-department/ri-institution-department.entity';
import { StatusType } from '@domain/status-type/status-type.entity';
import { TransformType } from '@domain/transform-type/transform-type.entity';
import { IsDate, IsNotEmpty } from 'class-validator';
import { RIProcedure } from '../../ri-procedure.entity';

export class RIProcedureDto extends RIProcedure {
    @IsNotEmpty()
    ProcedureType: ProcedureType;

    @IsNotEmpty()
    StatusType: StatusType;

    @IsNotEmpty()
    TransformType: TransformType;

    @IsNotEmpty()
    YearDue: number;

    @IsNotEmpty()
    @IsDate()
    ProcedureDate: Date;

    InstitutionID: number;

    TransformDetails: string;

    IsActive: boolean;

    RICPLRArea: RICPLRArea;

    RIInstitutionDepartments: RIInstitutionDepartment[];

    RIDocument?: RIDocument;

    Ord: number;
}
