import { Controller } from '@nestjs/common';
import { Crud, CrudController, CrudOptions } from '@nestjsx/crud';
import { RIInstitutionLatestActive } from '../../ri-institution-latest-active.entity';
import { RIInstitutionPublicService } from './ri-institution-public.service';

const crudOptions: CrudOptions = {
    model: {
        type: RIInstitutionLatestActive,
    },
    params: {
        RIInstitutionID: {
            type: 'number',
            primary: true,
            field: 'RIInstitutionID',
        },
    },
    routes: {
        only: ['getManyBase', 'getOneBase'],
    },
    query: {
        alwaysPaginate: true,
        join: {
            Town: { eager: true },
            'Town.Municipality': { eager: true, alias: 'town_municipality' },
            'Town.Municipality.Region': { eager: true, alias: 'town_municipality_region' },
            Country: { eager: true },
            LocalArea: { eager: true },
            FinancialSchoolType: { eager: true },
            DetailedSchoolType: { eager: true },
            BudgetingInstitution: { eager: true },
            BaseSchoolType: { eager: true },
            RIFlexFieldValues: { eager: true },
            'RIFlexFieldValues.RIFlexField': { eager: true },
        },
        filter: [
            {
                field: 'FinancialSchoolType.FinancialSchoolTypeID',
                operator: '$eq',
                value: 2,
            },
            {
                field: 'BudgetingInstitution.BudgetingInstitutionID',
                operator: '$eq',
                value: 5,
            },
            // TODO: add filter for municipalityID depends on logged-in user
        ],
        sort: [
            {
                field: 'InstitutionID',
                order: 'ASC',
            },
        ],
    },
};
@Crud(crudOptions)
@Controller('v1/ri-institution-public')
export class RIInstitutionPublicController implements CrudController<RIInstitutionLatestActive> {
    get base(): CrudController<RIInstitutionLatestActive> {
        return this;
    }

    constructor(public service: RIInstitutionPublicService) {}
}
