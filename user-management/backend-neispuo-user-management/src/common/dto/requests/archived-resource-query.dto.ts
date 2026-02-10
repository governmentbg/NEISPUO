import { ApiModelProperty } from '@nestjs/swagger';
import { IsNotEmpty, IsOptional, IsString } from 'class-validator';
import { IsValidSchoolYear } from 'src/common/decorators/class-validator/is-valid-school-year.decorator';
import { DTO } from '../responses/dto.interface';

export class ArchivedResourceQueryDTO implements DTO {
    @ApiModelProperty({
        description:
            'Mandatory identifier; userPersonID for enrollment, OrganizationID for institutions, PersonID for users)',
        example: '12345',
    })
    @IsNotEmpty()
    @IsString()
    identifier: string;

    @ApiModelProperty({
        description: 'Optional school year (e.g., 2022 or 2022/2023). If omitted, fetches data across all partitions.',
        required: false,
        example: '2022',
    })
    @IsOptional()
    @IsString()
    @IsValidSchoolYear()
    schoolYear?: string;
}
