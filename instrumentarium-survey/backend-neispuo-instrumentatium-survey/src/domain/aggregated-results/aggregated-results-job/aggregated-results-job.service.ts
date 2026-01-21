import { Campaign } from '@domain/campaigns/campaign.entity';
import { SubmittedQuestionaireStates } from '@domain/submitted-questionaire/submitted-questionaire-states.enum';
import { Injectable } from '@nestjs/common';
import { LoggerService } from '@shared/services/logger/logger.service';
import { Connection, EntityManager } from 'typeorm';
import { AggregatedResults } from '../aggregated-results.entity';

@Injectable()
export class AgreggatedResultsJobService {
    constructor(
        private connection: Connection,
        public loggerService: LoggerService
    ) {}

    public async generateAggregatedDataForExpiredCampaigns() {
        const expiredCampaigns = await this.connection.getRepository(Campaign)
            .createQueryBuilder("c")
            .where("c.endDate < GETDATE()")
            .andWhere("c.isActive = 1")
            .getMany();

        if (expiredCampaigns.length === 0) return;

        for (let campaign of expiredCampaigns) {

            try {
                await this.connection.transaction(async (em: EntityManager) => {
                    const aggregatedResultsRepo = em.getRepository(AggregatedResults);

                    let aggregatedResultsForCampaign = await em.query(`
                        select SUM(weight) as 'weightSum', count(weight) as 'choicesCount', 
                            ROUND(SUM(weight) / count(weight), 2) as 'avg', indicatorId, sq.questionaireId 
                        from tools_assessment.submittedQuestionaire sq 
                        cross apply openjson(sq.submittedQuestionaireObject)
                        with (indicatorId int '$.indicatorId',
                        weight float '$.weight'
                        ) as jsonValue
                        where sq.campaignId = ${campaign.id}
                        and sq.submittedQuestionaireObject is not null
                        and sq.submittedQuestionaireObject != ''
                        and sq.state = ${SubmittedQuestionaireStates.FINISHED}
                        and weight != -1
                        and weight != -2
                        group by sq.questionaireId, indicatorId
                    `);
                    
                    for (let data of aggregatedResultsForCampaign) {
                        await aggregatedResultsRepo.save({
                            totalScore: data.avg,
                            campaignId: campaign.id,
                            questionaireId: data.questionaireId,
                            indicatorId: data.indicatorId
                        });
                    }

                    await em.query(`
                        UPDATE tools_assessment.campaigns 
                        SET isActive = 0
                        WHERE endDate < GETDATE()
                        AND isActive = 1
                    `);
                });
            } catch (e) {
                this.loggerService.jobsLogger.error(`[AggregateDataForExpiredCampaign] Error: ${e}`)

            }
        }
    }
}