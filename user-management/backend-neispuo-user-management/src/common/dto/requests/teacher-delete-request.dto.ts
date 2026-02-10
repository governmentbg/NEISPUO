import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber } from 'class-validator';

export class DeleteTeacherDtoRequest {
    @IsNumber()
    @ApiModelProperty()
    personID: number;
}
