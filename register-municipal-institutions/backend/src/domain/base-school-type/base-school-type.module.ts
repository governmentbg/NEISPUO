import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { BaseSchoolTypeService } from './routes/base-school-type/base-school-type.service';
import { BaseSchoolTypeController } from './routes/base-school-type/base-school-type.controller';
import { BaseSchoolType } from './base-school-type.entity';

@Module({
    imports: [TypeOrmModule.forFeature([BaseSchoolType])],
    exports: [TypeOrmModule],

    controllers: [BaseSchoolTypeController],
    providers: [BaseSchoolTypeService],
})
export class BaseSchoolTypeModule {}
