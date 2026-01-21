import { Type } from 'class-transformer';
import { IsDate } from 'class-validator';

export class TermDTO {
    @Type(() => Date)
    @IsDate()
    termStartDate: Date;

    @Type(() => Date)
    @IsDate()
    termEndDate: Date;
}
