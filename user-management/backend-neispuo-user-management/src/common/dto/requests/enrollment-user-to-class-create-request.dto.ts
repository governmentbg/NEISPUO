import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber, IsOptional } from 'class-validator';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { DTO } from '../responses/dto.interface';

export class EnrollmentUserToClassCreateRequestDTO implements DTO {
    @IsNumber()
    @ApiModelProperty()
    personID: number;

    @IsNumber()
    @ApiModelProperty()
    curriculumID: number;

    @IsOptional()
    userRole?: UserRoleType;
}
