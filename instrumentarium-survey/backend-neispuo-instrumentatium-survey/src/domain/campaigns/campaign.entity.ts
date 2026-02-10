import { Column, Entity, JoinColumn, ManyToOne, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { SubmittedQuestionaire } from '../submitted-questionaire/submitted-questionaire.entity';
import { SysUser } from '../sys-user/sys-user.entity';
import { ApiModelProperty } from '@nestjs/swagger';
import { AggregatedResults } from '@domain/aggregated-results/aggregated-results.entity';

@Entity({ schema: 'tools_assessment', name: 'campaigns' })
export class Campaign {
    @PrimaryGeneratedColumn({ name: 'id' })
    id: number;

    @ApiModelProperty()
    @Column({
        type: 'nvarchar'
    })
    name: string;

    @ApiModelProperty()
    @Column({
        type: 'smallint'
    })
    type: number;

    @ApiModelProperty()
    @Column({
        type: 'datetime'
    })
    startDate: Date;

    @ApiModelProperty()
    @Column({
        type: 'datetime'
    })
    endDate: Date;

    @ApiModelProperty()
    @Column({
        type: 'smallint',
        nullable: true
    })
    isActive: number;

    @ApiModelProperty()
    @Column({
        type: 'smallint',
        nullable: true
    })
    isLocked: number;

    @ApiModelProperty()
    @ManyToOne(
        type => SysUser,
        sysUser => sysUser.campaignCreatedBy,
        { nullable: true }
    )
    @JoinColumn({ name: 'createdBy', referencedColumnName: 'SysUserID' })
    createdBy: SysUser|number;

    @ApiModelProperty()
    @Column({
        type: 'int'
    })
    institutionId: number;

    @ApiModelProperty()
    @Column({
        type: 'datetime'
    })
    createdAt: Date;

    @ApiModelProperty()
    @Column({
        type: 'datetime',
        nullable: true
    })
    updatedAt: Date;

    @ApiModelProperty()
    @Column({
        type: 'int',
        nullable: true
    })
    updatedBy: number;

    @ApiModelProperty()
    @Column({
        type: 'ntext',
        nullable: true
    })
    aggregate_results: string;

    @ApiModelProperty()
    @OneToMany(
        () => SubmittedQuestionaire,
        SubmitedQuestionaires => SubmitedQuestionaires.campaignsId
    )
    SubmittedQuestionaires: SubmittedQuestionaire[];

    @ApiModelProperty()
    @OneToMany(
        () => AggregatedResults,
        AggregatedResults => AggregatedResults.campaignId
    )
    aggregatedResults: AggregatedResults[];
}
