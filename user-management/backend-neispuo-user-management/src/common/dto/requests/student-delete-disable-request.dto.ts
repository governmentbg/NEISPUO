import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class StudentDeleteDisableRequestDTO implements DTO {
    @IsNumber()
    @ApiModelProperty()
    personID: number;

    @IsNumber()
    @ApiModelProperty()
    positionID: number;
}
