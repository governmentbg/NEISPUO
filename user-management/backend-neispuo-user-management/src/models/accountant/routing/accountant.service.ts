import { Injectable } from '@nestjs/common';
import { AccountantRepository } from '../accountant.repository';

@Injectable()
export class AccountantService {
    constructor(private accountantRepository: AccountantRepository) {}

    async getAccountantsByInstitutionID(institutionID: number) {
        return this.accountantRepository.getAccountantsByInstitutionID(institutionID);
    }
}
