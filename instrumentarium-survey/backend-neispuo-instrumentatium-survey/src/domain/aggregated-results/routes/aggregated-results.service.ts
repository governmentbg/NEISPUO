import { Injectable } from '@nestjs/common';
import { AggregatedResults } from '../aggregated-results.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { getManager, Repository } from 'typeorm';
import { SubmittedQuestionaireStates } from '@domain/submitted-questionaire/submitted-questionaire-states.enum';
import { AggregatedResultsForChartsModel } from '../models/aggregated-results-for-charts.model';

@Injectable()
export class AggregatedResultsService extends TypeOrmCrudService<AggregatedResults> {
    constructor(@InjectRepository(AggregatedResults) repo: Repository<AggregatedResults>) {
        super(repo);
    }

    public async addAggregatedDataForCharts(campaignId: number, aggregatedData: any) {
        const entityManager = getManager();

        const scoreByRoles = await entityManager.query(`
            select ROUND(AVG(ar.totalScore), 2) as 'score', sr.Name as 'role' 
            from tools_assessment.aggregatedResults ar 
            join tools_assessment.questionaire q on ar.questionaireId = q.id
            join core.SysRole sr on q.SysRoleID = sr.SysRoleID
            where ar.campaignId = ${campaignId}
            group by q.SysRoleID, sr.Name
        `);

        const countSubmitted = await entityManager.query(`
            select COUNT(sq.id) as 'count', sr.Name as 'role'
            from tools_assessment.submittedQuestionaire sq 
            join tools_assessment.questionaire q on sq.questionaireId = q.id
            join core.SysRole sr on q.SysRoleID = sr.SysRoleID 
            where sq.campaignId = ${campaignId}
            and sq.submittedQuestionaireObject is not null
            and sq.submittedQuestionaireObject != ''
            and sq.state = ${SubmittedQuestionaireStates.FINISHED}
            group by q.SysRoleID, sr.Name
        `);

        let aggregated: AggregatedResultsForChartsModel = {
            scoreByRoles: scoreByRoles,
            countSubmitted: countSubmitted,
            scoreByIndicators: aggregatedData
        };

        return aggregated;
    }
}
