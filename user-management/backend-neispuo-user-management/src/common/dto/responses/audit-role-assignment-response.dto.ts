import { DTO } from './dto.interface';

export class AuditRoleAssignmentDTO implements DTO {
    AssignedTo: number;

    CreatedBy: number;

    SysRole: number;

    CreatedOn?: Date;

    Action: string;

    InstitutionID: number;
}
