import { Controller } from '@nestjs/common';
import { Crud } from '@nestjsx/crud';
import { Role } from '../../role.entity';
import { RoleService } from './role.service';

// @Crud({
//     model: {
//         type: Role
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('role')
export class RoleController {
    constructor(public service: RoleService) {}
}
