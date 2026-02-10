import { AggregatedResults } from '@domain/aggregated-results/aggregated-results.entity';
import { ApiModelProperty } from '@nestjs/swagger';
import { Column, Entity, JoinColumn, ManyToOne, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { Criterion } from '../criteria/criteria.entity';
import { Question } from '../question/question.entity';
import { Subindicator } from '../subindicator/subindicator.entity';

@Entity({ schema: 'tools_assessment', name: 'indicator' })
export class Indicator {
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
        type => Criterion,
        Criteria => Criteria.Indicator
    )
    @JoinColumn({ name: 'criteriaId', referencedColumnName: 'id' })
    criteriaId: Criterion;

    @ApiModelProperty()
    @Column({
        type: 'int',
        nullable: true
    })
    weight: number;

    @ApiModelProperty()
    @OneToMany(
        () => Question,
        Question => Question.indicatorId
    )
    Question: Question[];

    @ApiModelProperty()
    @OneToMany(
        () => Subindicator,
        Subindicator => Subindicator.indicatorId
    )
    Subindicator: Question[];

    @ApiModelProperty()
    @OneToMany(
        () => AggregatedResults,
        AggregatedResults => AggregatedResults.indicatorId
    )
    aggregatedResults: AggregatedResults[];
}
