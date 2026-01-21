import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { JwksGuard } from 'src/shared/guards/jwks.guard';
import { NeispuoModule } from '../neispuo-module.entity';
import { NesipuoModuleService } from './neispuo-module.service';

@UseGuards(JwksGuard)
@Crud({
    model: {
        type: NeispuoModule
    },
    routes: {
        only: ['getManyBase', 'getOneBase'],
    },
    query: {
        join: {
            category: { eager: true },
            roles: { eager: true }
        }
    }

})
@Controller('v1/neispuo-module')
export class NesipuoModuleController implements CrudController<NeispuoModule>{
    constructor(public service: NesipuoModuleService) { }
}
