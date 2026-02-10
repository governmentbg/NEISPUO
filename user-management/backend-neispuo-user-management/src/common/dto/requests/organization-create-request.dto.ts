import { ApiModelProperty } from '@nestjs/swagger';
import { IsNumber } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class OrganizationCreateRequestDTO implements DTO {
    @ApiModelProperty()
    @IsNumber()
    institutionID: number;
}
