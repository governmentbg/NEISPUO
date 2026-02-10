import { Module } from '@nestjs/common';
import { UserService } from './routes/user/user.service';
import { UserController } from './routes/user/user.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { User } from '././user.entity';
import { LoginController } from './routes/login/login.controller';
import { LoginService } from './routes/login/login.service';
import { SharedModule } from '@shared/shared.module';
import { ChangePasswordController } from './routes/change-password/change-password.controller';
import { ChangePasswordService } from './routes/change-password/change-password.service';
import { RecaptchaService } from '@shared/services/recaptcha/recaptcha.service';
import { RegisterService } from './routes/register/register.service';
import { VerifyEmailController } from './routes/verify-email/verify-email.controller';
import { VerifyEmailService } from './routes/verify-email/verify-email.service';
import { FailedEmailDelivery } from '@domain/failed-email-delivery/failed-email-delivery.entity';

@Module({
    imports: [TypeOrmModule.forFeature([User, FailedEmailDelivery]), SharedModule],
    exports: [TypeOrmModule],

    controllers: [UserController, LoginController, ChangePasswordController, VerifyEmailController],
    providers: [
        UserService,
        LoginService,
        ChangePasswordService,
        RegisterService,
        RecaptchaService,
        VerifyEmailService
    ]
})
export class UserModule {}
