import { Module } from '@nestjs/common';
import { SysRoleService } from './routes/sys-role/sys-role.service';
import { SysRoleController } from './routes/sys-role/sys-role.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { SysRole } from './sys-role.entity';

@Module({
    imports: [TypeOrmModule.forFeature([SysRole])],
    exports: [TypeOrmModule],

    controllers: [SysRoleController],
    providers: [SysRoleService]
})
export class SysRoleModule {}
