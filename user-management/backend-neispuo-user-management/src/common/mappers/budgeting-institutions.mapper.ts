// mappers are used to convert one object to another

import { BudgetingInstitutionResponseDTO } from '../dto/responses/budgeting-institution-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class BudgetingInstitutionMapper {
    static transform(budgetingInstitutionObjects: any[]) {
        const result: BudgetingInstitutionResponseDTO[] = [];
        for (const budgetingInstitutionObject of budgetingInstitutionObjects) {
            const elementToBeInserted: BudgetingInstitutionResponseDTO = {
                sysUserID: budgetingInstitutionObject.sysUserID,
                isAzureUser: budgetingInstitutionObject.isAzureUser,
                username: budgetingInstitutionObject.username,
                name: budgetingInstitutionObject.name,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
