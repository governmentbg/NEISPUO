import { InstitutionDto } from '@domain/institution/routes/institution/institution.dto';
import { RIFlexFieldDto } from '@domain/ri-flex-field/routes/ri-flex-field/ri-flex-field.dto';
import { RIProcedureDto } from '@domain/ri-procedure/routes/ri-procedure/ri-procedure.dto';
import { IsNotEmpty, MinLength } from 'class-validator';
import { RIInstitutionDepartment } from '../../ri-institution-department.entity';

export class RIInstitutionDepartmentDTO extends RIInstitutionDepartment {
    RIInstitutionDepartmentID: number;

    @IsNotEmpty()
    Name: string;

    RIProcedure: RIProcedureDto;
}
