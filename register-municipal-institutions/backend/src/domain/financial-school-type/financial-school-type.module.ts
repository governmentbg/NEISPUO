import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { FinancialSchoolTypeService } from './routes/financial-school-type/financial-school-type.service';
import { FinancialSchoolTypeController } from './routes/financial-school-type/financial-school-type.controller';
import { FinancialSchoolType } from './financial-school-type.entity';

@Module({
    imports: [TypeOrmModule.forFeature([FinancialSchoolType])],
    exports: [TypeOrmModule],

    controllers: [FinancialSchoolTypeController],
    providers: [FinancialSchoolTypeService],
})
export class FinancialSchoolTypeModule {}
