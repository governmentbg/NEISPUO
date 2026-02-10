import { ApiModelProperty } from '@nestjs/swagger';
import { DTO } from './dto.interface';

export class RelativesAccessResponseDTO implements DTO {
    @ApiModelProperty()
    parentChildSchoolBookAccessID?: number;

    @ApiModelProperty()
    fullName: string;

    @ApiModelProperty()
    personID: string;

    @ApiModelProperty()
    username: string;

    @ApiModelProperty()
    hasAccess: boolean;
}
