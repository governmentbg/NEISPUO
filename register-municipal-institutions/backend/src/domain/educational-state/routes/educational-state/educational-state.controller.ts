import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { EducationalState } from '../../educational-state.entity';
import { EducationalStateService } from './educational-state.service';

// @Crud({
//     model: {
//         type: EducationalState
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('v1/educational-state')
export class EducationalStateController implements CrudController<EducationalState> {
    get base(): CrudController<EducationalState> {
        return this;
    }
    constructor(public service: EducationalStateService) {}
}
