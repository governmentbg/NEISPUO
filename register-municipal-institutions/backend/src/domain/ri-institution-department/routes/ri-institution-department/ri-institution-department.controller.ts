import { InstitutionService } from '@domain/institution/routes/institution/institution.service';
import { Body, Controller, HttpCode, Param, Post, Req, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { RIInstitutionDepartment } from '../../ri-institution-department.entity';
import { RIInstitutionDepartmentDTO } from './ri-institution-department.dto';
import { RIInstitutionDepartmentService } from './ri-institution-department.service';

@Crud({
    model: {
        type: RIInstitutionDepartment
    },
    routes: {
        only: ['getManyBase']
    },
    query: {
        join: {}
    }
})
@Controller('v1/ri-institution-department')
export class RIInstitutionDepartmentController implements CrudController<RIInstitutionDepartment> {
    constructor(public service: RIInstitutionDepartmentService) {}
}
