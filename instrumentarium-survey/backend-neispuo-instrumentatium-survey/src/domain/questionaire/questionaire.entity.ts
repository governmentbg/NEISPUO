import { AggregatedResults } from '@domain/aggregated-results/aggregated-results.entity';
import { ApiModelProperty } from '@nestjs/swagger';
import { Column, Entity, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { QuestionaireQuestion } from '../questionaire-question/questionaire-question.entity';
import { SubmittedQuestionaire } from '../submitted-questionaire/submitted-questionaire.entity';

@Entity({ schema: 'tools_assessment', name: 'questionaire' })
export class Questionaire {
    @PrimaryGeneratedColumn({ name: 'id' })
    id: number;

    @ApiModelProperty()
    @Column({
        type: 'nvarchar'
    })
    name: string;

    @ApiModelProperty()
    @Column({
        type: 'tinyint'
    })
    SysRoleID: number;

    @ApiModelProperty()
    @OneToMany(
        () => SubmittedQuestionaire,
        submittedQuestionaire => submittedQuestionaire.questionaireId
    )
    submittedQuestionaires: SubmittedQuestionaire[];

    @ApiModelProperty()
    @OneToMany(
        () => QuestionaireQuestion,
        QuestionaireQuestion => QuestionaireQuestion.questionaireId
    )
    QuestionaireQuestion: QuestionaireQuestion[];

    @ApiModelProperty()
    @OneToMany(
        () => AggregatedResults,
        AggregatedResults => AggregatedResults.questionaireId
    )
    aggregatedResults: AggregatedResults[];
}
