import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { StudentTeacherUsersEntity } from '../../../common/entities/student-teacher-users-view.entity';
import { TeacherUsersGuard } from './teacher-users.guard';
import { TeacherUsersService } from './teacher-users.service';

/**
 * General
 * The methods available and generated and exposed by the CRUD controller can be found at https://github.com/nestjsx/crud/wiki/Controllers#description
 *
 * Overriding methods
 * If you need to override any of the routes / methods you can refer to https://github.com/nestjsx/crud/wiki/Controllers#description
 */

@UseGuards(TeacherUsersGuard)
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
            'municipalityID',
            'regionID',
            'positionName',
            'personalID',
            'sysRoleID',
            // hotfix NUM-428
            // 'staffTypeID',
        ],
    },
})
@Controller('v1/teacher-users')
export class TeacherUsersController implements CrudController<StudentTeacherUsersEntity> {
    constructor(public service: TeacherUsersService) {}
}
