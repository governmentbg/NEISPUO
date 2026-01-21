import { InstitutionService } from '@domain/institution/routes/institution/institution.service';
import { Body, Controller, HttpCode, Param, Post, Req, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { RICPLRArea } from '../../ri-cplr-area.entity';
import { RICPLRAreaDTO } from './ri-cplr-area.dto';
import { RICPLRAreaService } from './ri-cplr-area.service';

@Crud({
    model: {
        type: RICPLRArea
    },
    routes: {
        only: ['getManyBase']
    },
    query: {
        join: {}
    }
})
@Controller('v1/ri-cplr-area')
export class RICPLRAreaController implements CrudController<RICPLRArea> {
    constructor(public service: RICPLRAreaService) {}
}
