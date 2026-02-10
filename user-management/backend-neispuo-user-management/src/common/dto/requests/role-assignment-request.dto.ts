import { Type } from 'class-transformer';
import { IsDate, IsNumber, IsString } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class RoleAssignmentDTO implements DTO {
    @IsNumber()
    RoleAssignmentID: number;

    @IsNumber()
    assignedTo: number;

    @IsNumber()
    createdBy: number;

    @IsNumber()
    sysRole: number;

    @Type(() => Date)
    @IsDate()
    CreatedOn: Date;

    @IsString()
    Action: string;

    @IsNumber()
    InstitutionID: number;
}
