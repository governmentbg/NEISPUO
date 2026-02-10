import { Controller, UseGuards } from '@nestjs/common';
import { Crud } from '@nestjsx/crud';
import { Country } from '../../country.entity';
import { CountryService } from './country.service';

// @Crud({
//     model: {
//         type: Country
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('v1/country')
export class CountryController {
    constructor(public service: CountryService) {}
}
