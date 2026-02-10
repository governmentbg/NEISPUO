import { ApiModelProperty } from '@nestjs/swagger';
import { IsOptional } from 'class-validator';

export class Filter {
    @ApiModelProperty()
    @IsOptional()
    value: string;

    @ApiModelProperty()
    @IsOptional()
    matchMode: string;

    constructor(value: string, matchMode: string) {
        this.value = value;
        this.matchMode = matchMode;
    }
}
