import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber, IsString } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class ParentRequestDTO implements DTO {
    @ApiModelProperty()
    @IsNumber()
    personID: number;

    @ApiModelProperty()
    @IsString()
    username: string;
}
