import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber } from 'class-validator';
import { DTO } from '../responses/dto.interface';
import { UserRequestDTO } from './user-request.dto.interface';

export class TeacherUpdateRequestDTO implements DTO, UserRequestDTO {
    @IsNumber()
    @ApiModelProperty()
    personID: number;
}
