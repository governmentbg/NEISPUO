import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';

import { QuestionaireService } from './routes/questionaire.service';
import { QuestionaireController } from './routes/questionaire.controller';
import { Questionaire } from './questionaire.entity';
// import { QuestionaireQuestion } from '@domain/questionaire-question/questionaire-question.entity';

@Module({
    imports: [TypeOrmModule.forFeature([Questionaire])],
    exports: [TypeOrmModule],

    controllers: [QuestionaireController],
    providers: [QuestionaireService]
})
export class QuestionaireModule {}