import { ApiModelProperty } from '@nestjs/swagger';
import { IsNotEmpty } from 'class-validator';

export class Paging {
    @ApiModelProperty()
    @IsNotEmpty()
    from: number;

    @ApiModelProperty()
    @IsNotEmpty()
    numberOfElements: number;

    constructor(from: number, numberOfElements: number) {
        this.from = from;
        this.numberOfElements = numberOfElements;
    }
}
