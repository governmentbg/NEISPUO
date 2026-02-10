import * as bcrypt from 'bcryptjs';
import { AuthedRequest } from '@shared/interfaces/authed-request.interface';
import { BadRequestException, Body, Controller, Post, Req, UseGuards } from '@nestjs/common';
import { ChangePasswordDTO } from './change-password.dto';
import { ChangePasswordGuard } from './change-password.guard';
import { ChangePasswordService } from './change-password.service';
import { ConfigService } from '@shared/services/config/config.service';
import { InjectRepository } from '@nestjs/typeorm';
import { PasswordValidator } from '@shared/services/password-validator/password-validator.service';
import { RecaptchaService } from '@shared/services/recaptcha/recaptcha.service';
import { Repository } from 'typeorm';
import { User } from '@domain/user/user.entity';
@UseGuards(ChangePasswordGuard)
@Controller('v1/auth/change-password')
export class ChangePasswordController {
    constructor(
        private readonly changePasswordService: ChangePasswordService,
        private configService: ConfigService,
        @InjectRepository(User) private readonly userRepo: Repository<User>,
        private readonly recaptchaService: RecaptchaService,
        private passwordValidator: PasswordValidator
    ) {}

    // @Post()
    // async changePassword(@Req() authedRequest: AuthedRequest, @Body() dto: ChangePasswordDTO) {
    //     await this.recaptchaService.validate(dto.recaptchaToken, authedRequest.ip);
    //     const currentUser = authedRequest._authObject.user;
    //     const isCurrentPassCorrect = bcrypt.compareSync(dto.currentPassword, currentUser.Password);

    //     if (!isCurrentPassCorrect) {
    //         throw new BadRequestException('Current password is wrong!');
    //     }

    //     if (dto.newPassword !== dto.passwordConfirmation) {
    //         throw new BadRequestException('Password confirmation does not match');
    //     }

    //     const { passwordValid, message } = this.passwordValidator.validate(dto.newPassword);
    //     if (!passwordValid) {
    //         throw new BadRequestException(message);
    //     }

    //     const password = await bcrypt.hash(
    //         dto.newPassword,
    //         +this.configService.get('BCRYPT_ROUNDS')
    //     );

    //     await this.userRepo.update(currentUser.SysUserID, { password });

    //     return { succes: true };
    // }
}
