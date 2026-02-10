import { Controller, UseGuards } from '@nestjs/common';
import { ApiBearerAuth } from '@nestjs/swagger';
import { Crud, CrudController } from '@nestjsx/crud';
import { Questionaire } from '../questionaire.entity';
import { QuestionaireGuard } from './questionaire.guard';
import { QuestionaireService } from './questionaire.service';

@Crud({
    model: {
        type: Questionaire
    },
    routes: {
        only: ['getManyBase', 'getOneBase', 'createOneBase', 'updateOneBase', 'deleteOneBase']
    }
})

@ApiBearerAuth()
@UseGuards(QuestionaireGuard)
@Controller('v1/questionaire')
export class QuestionaireController implements CrudController<Questionaire> {
    get base(): CrudController<Questionaire> {
        return this;
    }
    constructor(public service: QuestionaireService) {}
}
