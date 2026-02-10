import { Module } from '@nestjs/common';
import { CountryService } from '../country/routes/country/country.service';
import { CountryController } from '../country/routes/country/country.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { Country } from './country.entity';

@Module({
    imports: [TypeOrmModule.forFeature([Country])],
    exports: [TypeOrmModule],

    controllers: [CountryController],
    providers: [CountryService]
})
export class CountryModule {}
