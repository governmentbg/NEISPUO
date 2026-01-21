import { Controller } from '@nestjs/common';
import { CrudController } from '@nestjsx/crud';
import { RIFlexFieldValue } from '../../ri-flex-field-value.entity';
import { RIFlexFieldValueService } from './ri-flex-field-value.service';

// @Crud({
//     model: {
//         type: RIFlexFieldValue
//     },
//     routes: {
//         only: ['getManyBase']
//     },
//     query: {
//         join: {
//             RIInstitution: { eager: true }
//         }
//     }
// })
@Controller('v1/ri-flex-field-value')
export class RIFlexFieldValueController implements CrudController<RIFlexFieldValue> {
    get base(): CrudController<RIFlexFieldValue> {
        return this;
    }

    constructor(public service: RIFlexFieldValueService) {}
}
