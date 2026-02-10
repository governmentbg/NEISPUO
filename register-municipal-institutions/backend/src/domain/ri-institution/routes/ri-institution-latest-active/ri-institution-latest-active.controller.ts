import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController, CrudOptions } from '@nestjsx/crud';
import { cloneDeep } from 'lodash';
import { RIInstitutionLatestActive } from '../../ri-institution-latest-active.entity';
import { riInstitutionCrudOptions } from '../ri-institution/ri-institution.controller';
import { RIInstitutionLatestActiveGuard } from './ri-institution-latest-active.guard';
import { RIInstitutionLatestActiveService } from './ri-institution-latest-active.service';

const crudOptions: CrudOptions = cloneDeep(riInstitutionCrudOptions);
crudOptions.routes = { only: ['getManyBase'] };
crudOptions.model = {
    type: RIInstitutionLatestActive,
};

@UseGuards(RIInstitutionLatestActiveGuard)
@Crud(crudOptions)
/** This route inherits crud options (joins, etc) and guard from RIInstitutions */
@Controller('v1/ri-institution-latest-active')
export class RIInstitutionLatestActiveController
    implements CrudController<RIInstitutionLatestActive> {
    get base(): CrudController<RIInstitutionLatestActive> {
        return this;
    }

    constructor(public service: RIInstitutionLatestActiveService) {}
}
