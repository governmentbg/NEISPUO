import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber, IsOptional } from 'class-validator';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { DTO } from '../responses/dto.interface';

export class EnrollmentStudentToClassCreateRequestDTO implements DTO {
    @ApiModelProperty()
    @IsNumber()
    personID: number;

    @ApiModelProperty({
        type: 'number',
        isArray: true,
    })
    @IsNumber({}, { each: true })
    curriculumIDs: number[];

    @IsOptional()
    userRole? = UserRoleType.STUDENT;

    @IsOptional()
    basicClassID: number;
}
