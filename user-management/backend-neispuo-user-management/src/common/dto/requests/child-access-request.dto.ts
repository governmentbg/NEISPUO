import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber, IsString } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class ChildAccessRequestDTO implements DTO {
    @ApiModelProperty()
    @IsNumber()
    parentID: number;

    @ApiModelProperty()
    @IsString()
    childPersonalID: string;

    @ApiModelProperty()
    @IsString()
    childSchoolBookCode: string;
}
