import { ApiModelProperty } from '@nestjs/swagger';
import { Column, Entity, JoinColumn, ManyToOne, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { Choice } from '../choices/choice.entity';
import { Criterion } from '../criteria/criteria.entity';
import { Indicator } from '../indicator/indicator.entity';
import { QuestionArea } from '../question-area/question-area.entity';
import { QuestionaireQuestion } from '../questionaire-question/questionaire-question.entity';
import { Subindicator } from '../subindicator/subindicator.entity';

@Entity({ schema: 'tools_assessment', name: 'Question' })
export class Question {
    @PrimaryGeneratedColumn({ name: 'id' })
    id: number;

    @ApiModelProperty()
    @Column({
        type: 'smallint'
    })
    orderNumber: number;

    @ApiModelProperty()
    @Column({
        type: 'ntext'
    })
    title: string;

    @ApiModelProperty()
    @Column({
        type: 'nvarchar'
    })
    type: string;

    @ApiModelProperty()
    @ManyToOne(
        type => QuestionArea,
        QuestionArea => QuestionArea.Question
    )
    @JoinColumn({ name: 'questionAreaId', referencedColumnName: 'id' })
    questionAreaId: QuestionArea;

    @ApiModelProperty()
    @ManyToOne(
        type => Criterion,
        Criteria => Criteria.Question
    )
    @JoinColumn({ name: 'criteriaId', referencedColumnName: 'id' })
    criteriaId: Criterion;

    @ApiModelProperty()
    @ManyToOne(
        type => Indicator,
        Indicator => Indicator.Question
    )
    @JoinColumn({ name: 'indicatorId', referencedColumnName: 'id' })
    indicatorId: Indicator;

    @ApiModelProperty()
    @ManyToOne(
        type => Subindicator,
        Subindicator => Subindicator.Question
    )
    @JoinColumn({ name: 'subindicatorId', referencedColumnName: 'id' })
    subindicatorId: Subindicator;

    @ApiModelProperty()
    @OneToMany(
        () => Choice,
        Choice => Choice.questionId,
        { eager: true }
    )
    choices?: Choice[];

    @ApiModelProperty()
    @OneToMany(
        () => QuestionaireQuestion,
        QuestionaireQuestion => QuestionaireQuestion.questionId
    )
    QuestionaireQuestion: QuestionaireQuestion[];
}
