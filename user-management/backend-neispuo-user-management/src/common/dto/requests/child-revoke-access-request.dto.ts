import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class ChildRevokeAccessRequestDTO implements DTO {
    @ApiModelProperty()
    @IsNumber()
    personID: number;
}
