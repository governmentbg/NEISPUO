import { ApiModelProperty } from '@nestjs/swagger';
import { Type } from 'class-transformer';
import { IsBoolean, IsNumber } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class RoleAssignmentRequestDTO implements DTO {
    @ApiModelProperty()
    @IsNumber()
    @Type(() => Number)
    sysUserID: number;

    @ApiModelProperty()
    @IsNumber()
    @Type(() => Number)
    sysRoleID: number;

    @ApiModelProperty()
    @IsNumber()
    @Type(() => Number)
    institutionID: number;

    @ApiModelProperty()
    @IsBoolean()
    isDeleted: boolean;
}
