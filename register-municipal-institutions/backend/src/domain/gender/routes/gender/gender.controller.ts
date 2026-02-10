import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { Gender } from '../../gender.entity';
import { GenderService } from './gender.service';

// @Crud({
//     model: {
//         type: Gender
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('v1/gender')
export class GenderController implements CrudController<Gender> {
    get base(): CrudController<Gender> {
        return this;
    }
    constructor(public service: GenderService) {}
}
