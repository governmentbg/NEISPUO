import { IsNumber, IsOptional, IsString } from 'class-validator';

export class TownDTO {
    @IsOptional()
    @IsNumber()
    townId: number;

    @IsString()
    townName: string;
}
