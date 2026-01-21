import { ApiModelProperty } from '@nestjs/swagger';
import { IsBoolean, IsNumber } from 'class-validator';

export class UpdatePersonnelSchoolBookAccessRequestDTO {
    @ApiModelProperty()
    @IsNumber()
    rowID!: number;

    @ApiModelProperty()
    @IsBoolean()
    hasAdminAccess!: boolean;
}
