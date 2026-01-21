import { Controller, Post, Body, Req } from '@nestjs/common';
import { ResetPasswordDTO } from './reset-password.dto';
import { InjectRepository } from '@nestjs/typeorm';
import { User } from 'src/domain/user/user.entity';
import { Repository } from 'typeorm';
import { ForgotPassword } from 'src/domain/forgot-password/forgot-password.entity';
import { ResetPasswordService } from './reset-password.service';
import { BcryptService } from '@shared/services/bcrypt/bcrypt.service';
import { RecaptchaService } from 'src/shared/services/recaptcha/recaptcha.service';
import { Request } from 'express';
@Controller('v1/auth/reset-password')
export class ResetPasswordController {
    constructor(
        private resetPasswordService: ResetPasswordService,
        private bcryptService: BcryptService,
        private recaptchaService: RecaptchaService,
        @InjectRepository(User) private readonly userRepo: Repository<User>,
        @InjectRepository(ForgotPassword)
        private readonly forgotPasswordRepo: Repository<ForgotPassword>
    ) {}
    // @Post()
    // async resetPassword(
    //     @Req() req: Request,
    //     @Body() resetPasswordDTO: ResetPasswordDTO
    // ): Promise<{ success: true }> {
    //     await this.recaptchaService.validate(resetPasswordDTO.recaptchaToken, req.ip);

    //     const user = await this.resetPasswordService.validateInput(resetPasswordDTO);

    //     const password = await this.bcryptService.hash(resetPasswordDTO.newPassword);
    //     await this.forgotPasswordRepo.update({ user }, { used: true });
    //     await this.userRepo.update(resetPasswordDTO.userUuid, { password });

    //     return { success: true };
    // }
}
