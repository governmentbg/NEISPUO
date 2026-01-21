import {
    Controller,
    Post,
    Body,
    UnauthorizedException,
    ForbiddenException,
    Req
} from '@nestjs/common';
import { LoginService } from './login.service';
import { LoginRequestDTO, LoginResponseDTO } from './login.dto';
import { RecaptchaService } from '../../../../shared/services/recaptcha/recaptcha.service';
import { Request } from 'express';

@Controller('v1/auth/login')
export class LoginController {
    constructor(
        private readonly loginService: LoginService,
        private readonly recaptchaService: RecaptchaService
    ) {}

    // @Post()
    // async login(@Body() body: LoginRequestDTO, @Req() request: Request): Promise<LoginResponseDTO> {
    //     await this.recaptchaService.validate(body.recaptchaToken, request.ip);

    //     const validatedUser = await this.loginService.validateCredentials(body);

    //     if (!validatedUser) {
    //         throw new UnauthorizedException('Invalid email or password.');
    //     }

    //     if (!validatedUser.emailVerified) {
    //         throw new ForbiddenException('User has not been verified.');
    //     }

    //     return new LoginResponseDTO({
    //         accessToken: await this.loginService.createAccessTokenFromUser(validatedUser),
    //         user: validatedUser
    //     });
    // }
}
