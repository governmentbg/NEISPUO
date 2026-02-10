import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { SysUserService } from './routes/sys-user/sys-user.service';
import { SysUserController } from './routes/sys-user/sys-user.controller';
import { SysUser } from './sys-user.entity';

@Module({
    imports: [TypeOrmModule.forFeature([SysUser])],
    exports: [TypeOrmModule],

    controllers: [SysUserController],
    providers: [SysUserService],
})
export class SysUserModule {}
