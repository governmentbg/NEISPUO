import { Type } from 'class-transformer';
import { IsDate, IsString } from 'class-validator';
import { DTO } from './responses/dto.interface';

export class ClassDTO implements DTO {
    @IsString()
    classID: string;

    @IsString()
    title: string;

    @IsString()
    classCode: string;

    @IsString()
    orgID: string;

    @Type(() => Date)
    @IsDate()
    termStartDate: Date;

    @Type(() => Date)
    @IsDate()
    termEndDate: Date;
}
