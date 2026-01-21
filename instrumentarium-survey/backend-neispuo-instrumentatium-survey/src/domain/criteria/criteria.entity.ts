import { ApiModelProperty } from '@nestjs/swagger';
import { Column, Entity, JoinColumn, ManyToOne, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { Indicator } from '../indicator/indicator.entity';
import { QuestionArea } from '../question-area/question-area.entity';
import { Question } from '../question/question.entity';

@Entity({ schema: 'tools_assessment', name: 'Criteria' })
export class Criterion {
    @PrimaryGeneratedColumn({ name: 'id' })
    id: number;

    @ApiModelProperty()
    @Column({
        type: 'smallint'
    })
    orderNumber: number;

    @ApiModelProperty()
    @Column({
        type: 'nvarchar'
    })
    name: string;

    @ApiModelProperty()
    @ManyToOne(
        type => QuestionArea,
        QuestionArea => QuestionArea.Criterias
    )
    @JoinColumn({ name: 'questionAreaID', referencedColumnName: 'id' })
    questionAreaID: QuestionArea;

    @ApiModelProperty()
    @OneToMany(
        () => Question,
        Question => Question.criteriaId
    )
    Question: Question[];

    @ApiModelProperty()
    @OneToMany(
        () => Indicator,
        Indicator => Indicator.criteriaId
    )
    Indicator: Indicator[];
}
