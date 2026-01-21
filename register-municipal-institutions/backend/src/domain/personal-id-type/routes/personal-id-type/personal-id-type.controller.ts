import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { PersonalIDType } from '../../personal-id-type.entity';
import { PersonalIDTypeService } from './personal-id-type.service';

// @Crud({
//     model: {
//         type: PersonalIDType
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('v1/personal-id-type')
export class PersonalIDTypeController implements CrudController<PersonalIDType> {
    get base(): CrudController<PersonalIDType> {
        return this;
    }
    constructor(public service: PersonalIDTypeService) {}
}
