import { Controller } from '@nestjs/common';
import { CrudController } from '@nestjsx/crud';
import { Institution } from '../../institution.entity';
import { InstitutionService } from './institution.service';

// @Crud({
//     model: {
//         type: Institution
//     },
//     routes: {
//         only: ['getManyBase', 'getOneBase']
//     },

//     query: {
//         alwaysPaginate: true,
//         join: {
//             Town: { eager: true },
//             'Town.Municipality': { eager: true, alias: 'town_municipality' },
//             'Town.Municipality.Region': { eager: true, alias: 'town_municipality_region' }
//             // LocalArea: { eager: true },
//             // FinancialSchoolType: { eager: true },
//             // BaseSchoolType: { eager: true },
//             // BudgetingInstitution: { eager: true },
//             // DetailedSchoolType: { eager: true }
//         }
//     }
// })
@Controller('v1/institution')
export class InstitutionController implements CrudController<Institution> {
    get base(): CrudController<Institution> {
        return this;
    }

    constructor(public service: InstitutionService) {}
}
