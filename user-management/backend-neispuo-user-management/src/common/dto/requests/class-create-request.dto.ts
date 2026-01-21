import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class ClassCreateRequestDTO implements DTO {
    @ApiModelProperty()
    @IsNumber()
    curriculumID: number;

    @ApiModelProperty({
        type: 'number',
        isArray: true,
    })
    @IsNumber({}, { each: true })
    personIDs: number[];

    @ApiModelProperty()
    @IsNumber()
    institutionID: number;
}
