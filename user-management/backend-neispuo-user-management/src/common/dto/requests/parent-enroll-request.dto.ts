import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class ParentEnrollRequestDTO implements DTO {
    @ApiModelProperty()
    @IsNumber()
    parentID: number;
}
