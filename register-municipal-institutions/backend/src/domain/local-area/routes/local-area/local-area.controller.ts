import { Controller, UseGuards } from '@nestjs/common';
import { Crud } from '@nestjsx/crud';
import { BasicAuthReadOnlyGuard } from '../../../../shared/guards/basic-auth-read-only.guard';
import { LocalArea } from '../../local-area.entity';
import { LocalAreaService } from './local-area.service';

@Crud({
    model: {
        type: LocalArea,
    },
    routes: {
        only: ['getManyBase'],
    },
    query: {
        join: {
            Town: { eager: true },
            'Town.Municipality': { eager: true },
            'Town.Municipality.Region': { eager: true },
        },
    },
})
@UseGuards(BasicAuthReadOnlyGuard)
@Controller('v1/local-area')
export class LocalAreaController {
    constructor(public service: LocalAreaService) {}
}
