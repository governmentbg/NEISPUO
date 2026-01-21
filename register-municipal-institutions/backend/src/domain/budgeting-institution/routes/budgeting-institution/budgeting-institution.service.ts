import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { BudgetingInstitution } from '../../budgeting-institution.entity';

@Injectable()
export class BudgetingInstitutionService extends TypeOrmCrudService<BudgetingInstitution> {
    constructor(@InjectRepository(BudgetingInstitution) repo: Repository<BudgetingInstitution>) {
        super(repo);
    }
}
