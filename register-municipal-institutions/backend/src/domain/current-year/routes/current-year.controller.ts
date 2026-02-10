import { Controller } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { CurrentYearType } from '../current-year.entity';
import { CurrentYearService } from './current-year.service';

@Crud({
    model: {
        type: CurrentYearType,
    },
    routes: {
        only: ['getManyBase'],
    },
    query: {
        filter: {
            IsValid: true,
        },
    },
})
@Controller('v1/current-year')
export class CurrentYearController implements CrudController<CurrentYearType> {
    get base(): CrudController<CurrentYearType> {
        return this;
    }

    constructor(public service: CurrentYearService) {}
}
