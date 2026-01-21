import { Module } from '@nestjs/common';
import { SystemUserService } from './routing/system-user.service';
import { SystemUserRepository } from './system-user.repository';

@Module({
    providers: [SystemUserService, SystemUserRepository],
    controllers: [],
    exports: [SystemUserService],
})
export class SystemUserModule {}
