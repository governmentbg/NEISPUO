import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber, IsOptional } from 'class-validator';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { DTO } from '../responses/dto.interface';

export class EnrollmentStudentToSchoolDeleteRequestDTO implements DTO {
    @IsNumber()
    @ApiModelProperty()
    personID: number;

    @IsNumber()
    @ApiModelProperty()
    institutionID: number;

    @IsOptional()
    userRole = UserRoleType.STUDENT;
}
