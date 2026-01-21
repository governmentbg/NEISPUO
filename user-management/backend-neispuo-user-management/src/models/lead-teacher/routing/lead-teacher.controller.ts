import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { LeadTeacherEntity } from '../../../common/entities/lead-teacher.entity';
import { LeadTeacherGuard } from './lead-teacher.guard';
import { LeadTeacherService } from './lead-teacher.service';

/**
 * General
 * The methods available and generated and exposed by the CRUD controller can be found at https://github.com/nestjsx/crud/wiki/Controllers#description
 *
 * Overriding methods
 * If you need to override any of the routes / methods you can refer to https://github.com/nestjsx/crud/wiki/Controllers#description
 */

@UseGuards(LeadTeacherGuard)
@Crud({
    model: {
        type: LeadTeacherEntity,
    },
    params: {
        id: {
            field: 'PersonID',
            primary: true,
        },
    },
    routes: {
        only: ['getManyBase', 'getOneBase'],
    },
    query: {
        alwaysPaginate: true,
        maxLimit: 99999,
    },
})
@Controller('v1/lead-teacher-students')
export class LeadTeacherController implements CrudController<LeadTeacherEntity> {
    constructor(public service: LeadTeacherService) {}
}
