import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber, IsOptional } from 'class-validator';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { DTO } from '../responses/dto.interface';

export class EnrollmentUserToSchoolDeleteRequestDTO implements DTO {
    @ApiModelProperty()
    @IsNumber()
    personID: number;

    @ApiModelProperty()
    @IsNumber()
    institutionID: number;

    @IsOptional()
    userRole: UserRoleType;
}
