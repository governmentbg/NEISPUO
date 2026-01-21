import { ApiModelProperty } from '@nestjs/swagger';
import { Column, Entity, JoinColumn, ManyToOne, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { Indicator } from '../indicator/indicator.entity';
import { Question } from '../question/question.entity';

@Entity({ schema: 'tools_assessment', name: 'Subindicator' })
export class Subindicator {
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
        type => Indicator,
        Indicator => Indicator.Subindicator
    )
    @JoinColumn({ name: 'indicatorId', referencedColumnName: 'id' })
    indicatorId: Indicator;

    @ApiModelProperty()
    @Column({
        type: 'int',
        nullable: true
    })
    weight: number;

    @ApiModelProperty()
    @OneToMany(
        () => Question,
        Question => Question.subindicatorId
    )
    Question: Question[];
}
