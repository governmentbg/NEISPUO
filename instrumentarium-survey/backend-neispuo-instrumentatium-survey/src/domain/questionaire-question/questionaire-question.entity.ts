import { Entity, JoinColumn, ManyToOne, PrimaryGeneratedColumn } from 'typeorm';
import { Question } from '../question/question.entity';
import { Questionaire } from '../questionaire/questionaire.entity';
import { ApiModelProperty } from '@nestjs/swagger';

@Entity({ schema: 'tools_assessment', name: 'questionaire_question' })
export class QuestionaireQuestion {
    @PrimaryGeneratedColumn({ name: 'QuestionaireQuestionId' })
    QuestionaireQuestionId: number;

    @ApiModelProperty()
    @ManyToOne(
        type => Questionaire,
        Questionaire => Questionaire.QuestionaireQuestion
    )
    @JoinColumn({ name: 'questionaireId', referencedColumnName: 'id' })
    questionaireId: Questionaire;

    @ApiModelProperty()
    @ManyToOne(
        type => Question,
        Question => Question.QuestionaireQuestion
    )
    @JoinColumn({ name: 'questionId', referencedColumnName: 'id' })
    questionId: Question;
}
