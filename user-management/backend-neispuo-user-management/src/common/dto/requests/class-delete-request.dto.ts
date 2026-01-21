import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class ClassDeleteRequestDTO implements DTO {
    @ApiModelProperty()
    @IsNumber()
    curriculumID: number;
}
