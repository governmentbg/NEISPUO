import { Injectable } from '@nestjs/common';
import { Campaign } from '../../../../domain/campaigns/campaign.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { getManager, Repository } from 'typeorm';


@Injectable()
export class EsuiCampaignService extends TypeOrmCrudService<Campaign> {
    constructor(
        @InjectRepository(Campaign) repo: Repository<Campaign>, 
    ) {
        super(repo);
    }

    public async getAggregatedResultsForCampaign(campaignId: number) {
        const entityManager = getManager();
        const aggregatedResults = await entityManager.query(`
            select agg.*
            from [tools_assessment].[aggregatedResults] agg
            where agg.campaignId = ${campaignId}
        `);

        return aggregatedResults;
    }
}
