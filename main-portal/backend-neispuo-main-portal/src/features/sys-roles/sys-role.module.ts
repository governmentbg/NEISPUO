import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { SysRole } from 'src/entities/sys-role.entity';
import { SysRoleController } from './routes/sys-role.controller';
import { SysRoleService } from './routes/sys-role.service';

@Module({
  imports: [TypeOrmModule.forFeature([SysRole])],
  controllers: [SysRoleController],
  providers: [SysRoleService],
  exports: [SysRoleService],
})
export class SysRoleModule {}
