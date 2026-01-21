import { Module } from '@nestjs/common';
import { RoleService } from './routes/role/role.service';
import { RoleController } from './routes/role/role.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { Role } from './role.entity';

@Module({
    imports: [TypeOrmModule.forFeature([Role])],
    exports: [TypeOrmModule],

    controllers: [RoleController],
    providers: [RoleService]
})
export class RoleModule {}
