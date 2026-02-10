import { Module } from '@nestjs/common';
import { SIEMLoggerModule } from '../siem-logger/siem-logger.module';
import { UserController } from './routing/user.controller';
import { UserService } from './routing/user.service';
import { UserRepository } from './user.repository';

@Module({
    imports: [SIEMLoggerModule.forFeature()],
    providers: [UserService, UserRepository],
    exports: [UserService],
    controllers: [UserController],
})
export class UserModule {}
