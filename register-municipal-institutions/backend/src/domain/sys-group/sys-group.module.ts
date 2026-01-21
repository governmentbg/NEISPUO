import { Module } from '@nestjs/common';
import { SysGroupService } from './routes/sys-group/sys-group.service';
import { SysGroupController } from './routes/sys-group/sys-group.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { SysGroup } from './sys-group.entity';

@Module({
    imports: [TypeOrmModule.forFeature([SysGroup])],
    exports: [TypeOrmModule],

    controllers: [SysGroupController],
    providers: [SysGroupService]
})
export class SysGroupModule {}
