import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { BasicAuthReadOnlyGuard } from '../../../../shared/guards/basic-auth-read-only.guard';
import { Municipality } from '../../municipality.entity';
import { MunicipalityService } from './municipality.service';

@Crud({
    model: {
        type: Municipality,
    },
    params: {
        MunicipalityID: {
            type: 'number',
            primary: true,
            field: 'MunicipalityID',
        },
    },
    routes: {
        only: ['getManyBase', 'getOneBase'],
    },
    query: {
        join: {
            Region: { eager: true },
        },
    },
})
@UseGuards(BasicAuthReadOnlyGuard)
@Controller('v1/municipality')
export class MunicipalityController implements CrudController<Municipality> {
    get base(): CrudController<Municipality> {
        return this;
    }

    constructor(public service: MunicipalityService) {}
}
