import { ApiModelProperty } from '@nestjs/swagger';
import { Column, Entity, JoinColumn, ManyToOne, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { Campaign } from '../campaigns/campaign.entity';
import { Questionaire } from '../questionaire/questionaire.entity';

@Entity({ schema: 'tools_assessment', name: 'submittedQuestionaire' })
export class SubmittedQuestionaire {
    @PrimaryGeneratedColumn({ name: 'id' })
    id: number;

    @ApiModelProperty()
    @ManyToOne(
        type => Questionaire,
        questionaire => questionaire.submittedQuestionaires
    )
    @JoinColumn({ name: 'questionaireId', referencedColumnName: 'id' })
    questionaireId: Questionaire;

    @ApiModelProperty()
    @Column({
        type: 'smallint'
    })
    state: number;

    @ApiModelProperty()
    @ManyToOne(
        type => Campaign,
        Campaign => Campaign.SubmittedQuestionaires
    )
    @JoinColumn({ name: 'campaignId', referencedColumnName: 'id' })
    campaignsId: Campaign;

    @ApiModelProperty()
    @Column({
        type: 'int'
    })
    userId: number;

    @ApiModelProperty()
    @Column({
        type: 'text'
    })
    submittedQuestionaireObject: JSON|string;

}
