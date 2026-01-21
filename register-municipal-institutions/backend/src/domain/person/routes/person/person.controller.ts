import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { Person } from '../../person.entity';
import { PersonService } from './person.service';

// @Crud({
//     model: {
//         type: Person
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('v1/person')
export class PersonController implements CrudController<Person> {
    get base(): CrudController<Person> {
        return this;
    }
    constructor(public service: PersonService) {}
}
