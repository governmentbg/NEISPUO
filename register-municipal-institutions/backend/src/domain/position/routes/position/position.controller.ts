import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { Position } from '../../position.entity';
import { PositionService } from './position.service';

// @Crud({
//     model: {
//         type: Position
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('v1/position')
export class PositionController implements CrudController<Position> {
    get base(): CrudController<Position> {
        return this;
    }
    constructor(public service: PositionService) {}
}
