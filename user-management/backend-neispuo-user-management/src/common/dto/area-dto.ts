import { IsNumber, IsOptional, IsString } from 'class-validator';

export class AreaDTO {
    @IsOptional()
    @IsNumber()
    areaId: number;

    @IsString()
    areaName: string;
}
