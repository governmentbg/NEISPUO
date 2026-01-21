import {
    Controller,
    Body,
    UseGuards,
    BadRequestException,
    UseInterceptors,
    Req,
    Param,
    Post
} from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { RegisterService } from './register.service';
import * as bcrypt from 'bcryptjs';
import { RegisterGuard } from './register.guard';
import { ConfigService } from 'src/shared/services/config/config.service';
import { UserDTO } from '../user/user.dto';
import { AuthedRequest } from 'src/shared/interfaces/authed-request.interface';
import { Request } from 'express';

import { TrackUpdatedByInterceptor } from 'src/shared/interceptors/track-updated-by.interceptor';
import { RecaptchaService } from '@shared/services/recaptcha/recaptcha.service';
import { UserResponseDTO } from '../user/user.dto';

// Commented because it won't be used for now.

// @Crud({
//     model: {
//         type: UserDTO
//     },
//     routes: {
//         exclude: [
//             'getManyBase',
//             'getOneBase',
//             'createOneBase',
//             'createManyBase',
//             'updateOneBase',
//             'replaceOneBase',
//             'deleteOneBase',
//         ]
//     },
//     query: {}
// })
// @UseGuards(RegisterGuard)
// @UseInterceptors(TrackUpdatedByInterceptor)
// @Controller('v1/auth/register')
// export class RegisterController implements CrudController<UserDTO> {
//     get base(): CrudController<UserDTO> {
//         return this;
//     }

//     constructor(
//         public service: RegisterService,
//         private configService: ConfigService,
//         private recaptchaService: RecaptchaService
//     ) {}

//     @Post()
//     async register(@Req() request: Request, @Body() body: UserDTO): Promise<UserResponseDTO> {
//         await this.recaptchaService.validate(body.recaptchaToken, request.ip);

//         await this.service.register(body);

//         return { success: true } as UserResponseDTO;
//     }
// }
