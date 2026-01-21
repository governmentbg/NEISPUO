import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { FinancialSchoolType } from '../../financial-school-type.entity';

@Injectable()
export class FinancialSchoolTypeService extends TypeOrmCrudService<FinancialSchoolType> {
    constructor(@InjectRepository(FinancialSchoolType) repo: Repository<FinancialSchoolType>) {
        super(repo);
    }
}
