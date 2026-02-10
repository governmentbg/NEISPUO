import { Controller, UseGuards } from '@nestjs/common';
import { Crud } from '@nestjsx/crud';
import { BasicAuthReadOnlyGuard } from '../../../../shared/guards/basic-auth-read-only.guard';
import { Town } from '../../town.entity';
import { TownService } from './town.service';

@Crud({
    model: {
        type: Town,
    },
    routes: {
        only: ['getManyBase'],
    },
    query: {
        join: {
            Municipality: { eager: true },
            'Municipality.Region': { eager: true },
        },
    },
})
@UseGuards(BasicAuthReadOnlyGuard)
@Controller('v1/town')
export class TownController {
    constructor(public service: TownService) {}
}
