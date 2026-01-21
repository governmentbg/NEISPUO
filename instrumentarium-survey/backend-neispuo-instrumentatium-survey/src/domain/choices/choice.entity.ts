import { Column, Entity, JoinColumn, ManyToOne, OneToMany, PrimaryGeneratedColumn } from 'typeorm';

import { Question } from '../question/question.entity';
import { ApiModelProperty } from '@nestjs/swagger';

@Entity({ schema: 'tools_assessment', name: 'Choices' })
export class Choice {
    @PrimaryGeneratedColumn({ name: 'id' })
    id: number;

    @ApiModelProperty()
    @Column({
        type: 'ntext'
    })
    title: string;

    @ApiModelProperty()
    @Column({
        type: 'ntext'
    })
    description: string;

    @ApiModelProperty()
    @Column({
        type: 'nvarchar'
    })
    value: string;

    @ApiModelProperty()
    @ManyToOne(
        () => Question,
        Question => Question.choices
    )
    @JoinColumn({ name: 'questionID' })
    questionId: Question;

    @ApiModelProperty()
    @Column({
        type: 'int'
    })
    weight: number;
}
