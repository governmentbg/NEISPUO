import { Module } from '@nestjs/common';
import { SysUserSysRoleService } from './routes/sysuser-sysrole/sysuser-sysrole.service';
import { SysUserSysRoleController } from './routes/sysuser-sysrole/sysuser-sysrole.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { SysUserSysRole } from './sysuser-sysrole.entity';

@Module({
    imports: [TypeOrmModule.forFeature([SysUserSysRole])],
    exports: [TypeOrmModule],

    controllers: [SysUserSysRoleController],
    providers: [SysUserSysRoleService]
})
export class SysUserSysRoleModule {}
