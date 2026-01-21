import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { RIFlexField } from '../../ri-flex-field.entity';
import { RIFlexFieldGuard } from './ri-flex-field.guard';
import { RIFlexFieldService } from './ri-flex-field.service';

@Crud({
    model: {
        type: RIFlexField,
    },
    params: {
        RIFlexFieldID: {
            type: 'number',
            primary: true,
            field: 'RIFlexFieldID',
        },
    },
    routes: {
        only: ['getManyBase', 'getOneBase', 'createOneBase', 'replaceOneBase', 'deleteOneBase'],
    },
    query: {
        join: {
            RIFlexFieldValues: { eager: true, allow: ['RIFlexFieldValueID'] },
            'RIFlexFieldValues.RIInstitution': {
                eager: true,
                allow: ['RIInstitutionID', 'Name', 'Abbreviation'],
            },
            Municipality: { eager: true },
            SysUser: { eager: true, exclude: ['Password'] },
        },
    },
})
@UseGuards(RIFlexFieldGuard)
@Controller('v1/ri-flex-field')
export class RIFlexFieldController implements CrudController<RIFlexField> {
    get base(): CrudController<RIFlexField> {
        return this;
    }

    constructor(public service: RIFlexFieldService) {}
}
