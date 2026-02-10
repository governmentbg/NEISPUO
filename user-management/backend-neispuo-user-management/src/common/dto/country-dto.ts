import { IsNumber, IsOptional, IsString } from 'class-validator';

export class CountryDTO {
    @IsOptional()
    @IsNumber()
    countryId: number;

    @IsString()
    countryName: string;
}
