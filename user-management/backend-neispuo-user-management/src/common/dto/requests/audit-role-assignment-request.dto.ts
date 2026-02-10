import { Type } from 'class-transformer';
import { IsDate, IsString } from 'class-validator';
import { InstitutionEntity } from 'src/common/entities/institution.entity';
import { SysRoleEntity } from 'src/common/entities/sys-role.entity';
import { SysUserEntity } from 'src/common/entities/sys-user.entity';
import { QueryPartialEntity } from 'typeorm/query-builder/QueryPartialEntity';
import { DTO } from '../responses/dto.interface';

export class AuditRoleAssignmentDTO implements DTO {
    // DTO is used for create, where we do not have ID
    // @IsNumber()
    // RoleAssignmentID?: number;

    AssignedTo: QueryPartialEntity<SysUserEntity>;

    CreatedBy: QueryPartialEntity<SysUserEntity>;

    SysRole: QueryPartialEntity<SysRoleEntity>;

    @Type(() => Date)
    @IsDate()
    CreatedOn: Date;

    @IsString()
    Action: string;

    InstitutionID: QueryPartialEntity<InstitutionEntity>;
}
