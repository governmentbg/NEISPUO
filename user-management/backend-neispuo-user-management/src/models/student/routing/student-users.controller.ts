import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { StudentUsersGuard } from 'src/models/student/routing/student-users.guard';
import { StudentTeacherUsersEntity } from '../../../common/entities/student-teacher-users-view.entity';
import { StudentUsersService } from './students-users.service';

/**
 * General
 * The methods available and generated and exposed by the CRUD controller can be found at https://github.com/nestjsx/crud/wiki/Controllers#description
 *
 * Overriding methods
 * If you need to override any of the routes / methods you can refer to https://github.com/nestjsx/crud/wiki/Controllers#description
 */

@UseGuards(StudentUsersGuard)
@Crud({
    model: {
        type: StudentTeacherUsersEntity,
    },
    routes: {
        only: ['getManyBase', 'getOneBase'],
    },
    query: {
        alwaysPaginate: true,
        maxLimit: 99999,
        exclude: [
            'townID',
            'townName',
            'municipalityID',
            'municipalityName',
            'regionID',
            'regionName',
            'positionName',
            'personalID',
            'sysRoleID',
            // hotfix NUM-428
            // 'staffTypeName',
            // 'staffTypeID',
        ],
    },
})
@Controller('v1/student-users')
export class StudentUsersController implements CrudController<StudentTeacherUsersEntity> {
    constructor(public service: StudentUsersService) {}
}
