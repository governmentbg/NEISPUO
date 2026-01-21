import { Controller } from '@nestjs/common';
import { CrudController } from '@nestjsx/crud';
import { RIPremInstitution } from '../../ri-prem-institution.entity';
import { RIPremInstitutionService } from './ri-prem-institution.service';

// @Crud({
//     model: {
//         type: RIPremInstitution
//     }
// })
@Controller('v1/ri-prem-institution')
export class RIPremInstitutionController implements CrudController<RIPremInstitution> {
    get base(): CrudController<RIPremInstitution> {
        return this;
    }

    constructor(public service: RIPremInstitutionService) {}
}
