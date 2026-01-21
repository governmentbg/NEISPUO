import { Controller, UseGuards } from '@nestjs/common';
import { Crud } from '@nestjsx/crud';
import { BasicAuthReadOnlyGuard } from '../../../../shared/guards/basic-auth-read-only.guard';
import { Region } from '../../region.entity';
import { RegionService } from './region.service';

@Crud({
    model: {
        type: Region,
    },
    routes: {
        only: ['getManyBase'],
    },
})
@UseGuards(BasicAuthReadOnlyGuard)
@Controller('v1/region')
export class RegionController {
    constructor(public service: RegionService) {}
}
