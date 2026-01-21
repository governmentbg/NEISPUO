import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { DetailedSchoolTypeService } from './routes/detailed-school-type/detailed-school-type.service';
import { DetailedSchoolTypeController } from './routes/detailed-school-type/detailed-school-type.controller';
import { DetailedSchoolType } from './detailed-school-type.entity';

@Module({
    imports: [TypeOrmModule.forFeature([DetailedSchoolType])],
    exports: [TypeOrmModule],

    controllers: [DetailedSchoolTypeController],
    providers: [DetailedSchoolTypeService],
})
export class DetailedSchoolTypeModule {}
