import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class SchoolBookCodeAssignRequestDTO implements DTO {
    @ApiModelProperty({
        type: 'number',
        isArray: true,
    })
    @IsNumber({}, { each: true })
    personIDs: number[];
}
