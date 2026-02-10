import {
 Controller, UseGuards,
} from '@nestjs/common';
import {
    Crud,
    CrudController,
    CrudOptions,
    CrudRequest,
    Override,
    ParsedRequest,
} from '@nestjsx/crud';
import { cloneDeep } from 'lodash';
import { CondOperator } from '@nestjsx/crud-request';
import { ProcedureTypeEnum } from '../../../procedure-type/enums/procedure-type.enum';
import { RIInstitution } from '../../ri-institution.entity';
import { riInstitutionCrudOptions } from '../ri-institution/ri-institution.controller';
import { RIInstitutionClosedGuard } from './ri-institution-closed.guard';
import { RIInstitutionClosedService } from './ri-institution-closed.service';

const crudOptions: CrudOptions = cloneDeep(riInstitutionCrudOptions);
crudOptions.routes = { only: ['getManyBase'] };

@UseGuards(RIInstitutionClosedGuard)
@Crud(crudOptions)
/** This route inherits crud options (joins, etc) and guard from RIInstitutions */
@Controller('v1/ri-institution-closed')
export class RIInstitutionClosedController implements CrudController<RIInstitution> {
    get base(): CrudController<RIInstitution> {
        return this;
    }

    constructor(public service: RIInstitutionClosedService) {}

    @Override('getManyBase')
    getClosedInstitutions(@ParsedRequest() req: CrudRequest) {
        const { parsed } = req;

        // select only where procedure type === closed and also apply whatever other filters
        parsed.search = {
            $and: [
                {
                    'RIProcedure.ProcedureType.ProcedureTypeID': {
                        [CondOperator.EQUALS]: ProcedureTypeEnum.DELETE,
                    },
                },
                cloneDeep(parsed.search),
            ],
        };

        return this.base.getManyBase(req);
    }
}
