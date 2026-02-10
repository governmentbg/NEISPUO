import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { BudgetingInstitutionsRepository } from '../budgeting-institution.repository';

@Injectable()
export class BudgetingInstitutionService {
    constructor(
        @InjectRepository(BudgetingInstitutionsRepository)
        private budgetingInstitutionsRepository: BudgetingInstitutionsRepository,
    ) {}

    getAllBudgetingInstitutions() {
        return this.budgetingInstitutionsRepository.getAllBudgetingInstitutions();
    }
}
