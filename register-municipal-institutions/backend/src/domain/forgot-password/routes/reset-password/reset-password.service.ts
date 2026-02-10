import { Injectable, UnauthorizedException, BadRequestException } from '@nestjs/common';
import { ResetPasswordDTO } from './reset-password.dto';
import { InjectRepository } from '@nestjs/typeorm';
import { User } from 'src/domain/user/user.entity';
import { Repository } from 'typeorm';
import { ForgotPassword } from 'src/domain/forgot-password/forgot-password.entity';
import { ConfigService } from 'src/shared/services/config/config.service';
import { PasswordValidator } from '@shared/services/password-validator/password-validator.service';

@Injectable()
export class ResetPasswordService {
    constructor(
        private configService: ConfigService,
        @InjectRepository(User) private readonly userRepo: Repository<User>,
        @InjectRepository(ForgotPassword)
        private readonly forgotPasswordRepo: Repository<ForgotPassword>,
        private readonly passwordValidator: PasswordValidator
    ) {}

    async validateInput(resetPasswordDTO: ResetPasswordDTO): Promise<User> {
        // validate user
        const user = await this.userRepo.findOne(resetPasswordDTO.userUuid);
        if (!user) {
            throw new UnauthorizedException('Invalid Request!');
        }

        // validate token
        const forgotPassRecord = await this.forgotPasswordRepo.findOne({
            user,
            token: resetPasswordDTO.token,
            used: false
        });
        if (!forgotPassRecord) {
            throw new UnauthorizedException('Invalid Request!');
        }

        // validate token expiration
        if (
            forgotPassRecord.createdAt <
            new Date(
                Date.now() -
                    3600 * 1000 * +this.configService.get('FORGOT_PASS_LINK_EXPIRATION_HOURS')
            )
        ) {
            throw new BadRequestException('Reset period expired!');
        }

        // validate password
        if (resetPasswordDTO.newPassword !== resetPasswordDTO.passwordConfirmation) {
            throw new BadRequestException('Passwords do not match!');
        }
        const { passwordValid, message } = this.passwordValidator.validate(
            resetPasswordDTO.newPassword
        );
        if (!passwordValid) {
            throw new BadRequestException(message);
        }

        return user;
    }
}
