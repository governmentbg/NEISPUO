import { ApiModelProperty } from '@nestjs/swagger';
import { DTO } from './dto.interface';

export class ParentChildrenResponseDTO implements DTO {
    @ApiModelProperty()
    personID: number;

    @ApiModelProperty()
    firstName: string;

    @ApiModelProperty()
    middleName: string;

    @ApiModelProperty()
    lastName: string;

    @ApiModelProperty()
    personalID: string;

    @ApiModelProperty()
    schoolBooksCodes: string;
}
