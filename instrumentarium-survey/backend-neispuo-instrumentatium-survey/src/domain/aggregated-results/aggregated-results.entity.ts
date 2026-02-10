import { Column, Entity, JoinColumn, ManyToOne, PrimaryGeneratedColumn } from 'typeorm';
import { ApiModelProperty } from '@nestjs/swagger';
import { Campaign } from '@domain/campaigns/campaign.entity';
import { Questionaire } from '@domain/questionaire/questionaire.entity';
import { Indicator } from '@domain/indicator/indicator.entity';

@Entity({ schema: 'tools_assessment', name: 'aggregatedResults' })
export class AggregatedResults {
    @PrimaryGeneratedColumn({ name: 'id' })
    id: number;

    @ApiModelProperty()
    @Column({
        type: 'float'
    })
    totalScore: number;

    @ApiModelProperty()
    @ManyToOne(
        () => Campaign,
        Campaign => Campaign.aggregatedResults
    )
    @JoinColumn({ name: 'campaignId' })
    campaignId: Campaign | number;

    @ApiModelProperty()
    @ManyToOne(
        () => Questionaire,
        Questionaire => Questionaire.aggregatedResults
    )
    @JoinColumn({ name: 'questionaireId' })
    questionaireId: Questionaire | number;

    @ApiModelProperty()
    @ManyToOne(
        () => Indicator,
        Indicator => Indicator.aggregatedResults
    )
    @JoinColumn({ name: 'indicatorId' })
    indicatorId: Indicator | number;
}
