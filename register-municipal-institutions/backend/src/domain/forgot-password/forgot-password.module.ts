import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ForgotPassword } from './forgot-password.entity';
import { SharedModule } from 'src/shared/shared.module';
import { ForgotPasswordController } from './routes/forgot-password/forgot-password.controller';
import { ForgotPasswordService } from './routes/forgot-password/forgot-password.service';
import { User } from '../user/user.entity';
import { ResetPasswordController } from './routes/reset-password/reset-password.controller';
import { ResetPasswordService } from './routes/reset-password/reset-password.service';

@Module({
    imports: [TypeOrmModule.forFeature([ForgotPassword, User]), SharedModule],
    exports: [TypeOrmModule],

    controllers: [ForgotPasswordController, ResetPasswordController],
    providers: [ForgotPasswordService, ResetPasswordService]
})
export class ForgotPasswordModule {}
