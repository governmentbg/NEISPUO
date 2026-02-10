import { Crud, CrudController, Override, ParsedRequest, CrudRequest } from '@nestjsx/crud';
import { ForgotPassword } from '../../forgot-password.entity';
import {
    Controller,
    UseGuards,
    Body,
    BadRequestException,
    Req,
    Inject,
    Logger
} from '@nestjs/common';
import { ForgotPasswordService } from './forgot-password.service';
import { CreateForgotPasswordDTO } from './forgot-password.dto';
import { InjectRepository } from '@nestjs/typeorm';
import { User } from '../../../../domain/user/user.entity';
import { Repository } from 'typeorm';
import { ConfigService } from '../../../../shared/services/config/config.service';
import { ForgotPasswordGuard } from './forgot-password.guard';
import * as randomString from 'randomstring';
import { RecaptchaService } from '../../../../shared/services/recaptcha/recaptcha.service';
import { Request } from 'express';

// @Crud({
//     model: {
//         type: ForgotPassword
//     },
//     routes: {
//         only: ['createOneBase']
//     },
//     query: {
//         join: {
//             user: {
//                 eager: true
//             }
//         }
//     }
// })
@UseGuards(ForgotPasswordGuard)
@Controller('v1/auth/forgot-password')
export class ForgotPasswordController {
    get base(): CrudController<ForgotPassword> {
        return this;
    }
    constructor(
        public service: ForgotPasswordService,
        private readonly configService: ConfigService,
        private readonly recaptchaService: RecaptchaService,
        @Inject('winston') private readonly logger: Logger,
        @InjectRepository(User) private readonly userRepo: Repository<User>
    ) {}

    // @Override()
    // async createOne(
    //     @Req() request: Request,
    //     @ParsedRequest() req: CrudRequest,
    //     @Body() body: CreateForgotPasswordDTO
    // ): Promise<{ success: true }> {
    //     await this.recaptchaService.validate(body.recaptchaToken, request.ip);

    //     if (!body?.email) {
    //         throw new BadRequestException();
    //     }

    //     const user = await this.userRepo.findOne({ email: body.email });
    //     if (!user) {
    //         this.logger.warn(
    //             `Attempted forgot-password for invalid email: ${body.email}. User does not exist!`
    //         );
    //         return;
    //     }

    //     const token = randomString.generate({
    //         length: 32,
    //         charset: 'alphanumeric'
    //     });
    //     const forgotPasswordDto: ForgotPassword = {
    //         user,
    //         token,
    //         used: false
    //     };

    //     await this.base.createOneBase(req, forgotPasswordDto);

    //     const uiEndpoint =
    //         this.configService.get('UI_BASE_URL') +
    //         this.configService.get('UI_RESET_PASSWORD_ENDPOINT');
    //     const url = `${uiEndpoint}?userUuid=${user.id}&token=${token}`;

    //     return { success: true };
    // }
}
