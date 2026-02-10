import { ApiModelProperty } from '@nestjs/swagger';
import { Column, Entity, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { Criterion } from '../criteria/criteria.entity';
import { Question } from '../question/question.entity';

@Entity({ schema: 'tools_assessment', name: 'QuestionArea' })
export class QuestionArea {
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
    title: string;

    @ApiModelProperty()
    @Column({
        type: 'ntext'
    })
    description: string;

    @ApiModelProperty()
    @OneToMany(
        () => Question,
        Question => Question.questionAreaId
    )
    Question: Question[];

    @ApiModelProperty()
    @OneToMany(
        () => Criterion,
        Criterion => Criterion.questionAreaID
    )
    Criterias: Question[];
}
