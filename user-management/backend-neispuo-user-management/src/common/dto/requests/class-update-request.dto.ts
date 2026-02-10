import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class ClassUpdateRequestDTO implements DTO {
    @ApiModelProperty()
    @IsNumber()
    curriculumID: number;

    @ApiModelProperty({
        type: 'number',
        isArray: true,
    })
    @IsNumber({}, { each: true })
    personIDsToDelete: number[];

    @ApiModelProperty({
        type: 'number',
        isArray: true,
    })
    @IsNumber({}, { each: true })
    personIDsToCreate: number[];

    @ApiModelProperty()
    @IsNumber()
    institutionID: number;
}
