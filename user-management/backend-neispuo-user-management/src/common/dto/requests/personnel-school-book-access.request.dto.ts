import { ApiModelProperty } from '@nestjs/swagger';
import { IsBoolean, IsNumber } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class PersonnelSchoolBookAccessRequestDTO implements DTO {
    @ApiModelProperty()
    @IsNumber()
    rowID!: number;

    @ApiModelProperty()
    @IsNumber()
    schoolYear!: number;

    @ApiModelProperty()
    @IsNumber()
    classBookID!: number;

    @ApiModelProperty()
    @IsNumber()
    personID!: number;

    @ApiModelProperty()
    @IsBoolean()
    hasAdminAccess!: boolean;
}
