import {
    Controller,
    Body,
    UseGuards,
    BadRequestException,
    UseInterceptors,
    Req,
    Param,
    Put,
    NotFoundException,
    Get,
    ClassSerializerInterceptor,
    UsePipes,
    ValidationPipe
} from '@nestjs/common';
import {
    Crud,
    Override,
    ParsedRequest,
    CrudRequest,
    CrudController,
    ParsedBody
} from '@nestjsx/crud';
import { UserService } from './user.service';
import { UserGuard } from './user.guard';
import { ConfigService } from '../../../../shared/services/config/config.service';
import { UserDTO } from './user.dto';
import { TrackUpdatedByInterceptor } from '../../../../shared/interceptors/track-updated-by.interceptor';
import { AuthedRequest } from '../../../../shared/interfaces/authed-request.interface';
import { UpdateUserDto } from './update-user.dto';
import { async } from 'rxjs/internal/scheduler/async';

// @Crud({
//     model: {
//         type: UserDTO
//     },
//     params: {
//         id: {
//             type: 'uuid',
//             primary: true,
//             field: 'id'
//         }
//     },
//     routes: {
//         only: ['getOneBase', 'getManyBase']
//     },

//     query: {
//         join: {
//             submissions: { eager: true },
//             roles: { eager: true },
//             files: { eager: true }
//         }
//     }
// })
@UseGuards(UserGuard)
@Controller('v1/user')
export class UserController implements CrudController<UserDTO> {
    get base(): CrudController<UserDTO> {
        return this;
    }

    constructor(public service: UserService, private configService: ConfigService) {}

    // @Put(':id')
    // @UsePipes(new ValidationPipe({ transform: true })) // to make use of @Exclude/class-transformer.
    // async updateUser(
    //     @Req() httpRequest: AuthedRequest,
    //     @Body() body: UpdateUserDto,
    //     @Param('id') id: string
    // ) {
    //     const updatedUser = await this.service.updateUser(id, body);
    //     return { ...updatedUser };
    // }
}
