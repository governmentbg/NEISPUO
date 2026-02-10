import { Type } from 'class-transformer';
import { IsString, ValidateNested } from 'class-validator';
import { AreaDTO } from './area-dto';
import { CountryDTO } from './country-dto';
import { TownDTO } from './town-dto';

export class AddressDTO {
    @ValidateNested()
    @Type(() => TownDTO)
    town: TownDTO;

    @ValidateNested()
    @Type(() => AreaDTO)
    area: AreaDTO;

    @ValidateNested()
    @Type(() => CountryDTO)
    country: CountryDTO;

    @IsString()
    postalCode: string;

    @IsString()
    street: string;
}
