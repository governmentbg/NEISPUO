import { Controller, UseGuards } from '@nestjs/common';
import { ApiBearerAuth } from '@nestjs/swagger';
import { Crud, CrudController, CrudRequest, Override, ParsedRequest } from '@nestjsx/crud';
import { AggregatedResults } from '../aggregated-results.entity';
import { AggregatedResultsForChartsModel } from '../models/aggregated-results-for-charts.model';
import { AggregatedResultsGuard } from './aggregated-results.guard';
import { AggregatedResultsService } from './aggregated-results.service';

@Crud({
    model: {
        type: AggregatedResults
    },
    query: {
        join: {
            campaignId: { eager: true },
            questionaireId: { eager: true },
            indicatorId: { eager: true },
        }
    },
    routes: {
        only: ['getManyBase']
    }
})

@ApiBearerAuth()
@UseGuards(AggregatedResultsGuard)
@Controller('v1/aggregated-results')
export class AggregatedResultsController implements CrudController<AggregatedResults> {
    get base(): CrudController<AggregatedResults> {
        return this;
    }
    constructor(public service: AggregatedResultsService) { }
    
    @Override('getManyBase')
    async getMany(@ParsedRequest() req: CrudRequest) {
        let aggregatedDataByIndicators = await this.base.getManyBase(req);
        let campaignId : any = req.parsed.search.$and[2];
        campaignId = campaignId["campaignId.ID"];

        let aggregatedData : AggregatedResultsForChartsModel = 
            await this.service.addAggregatedDataForCharts(campaignId, aggregatedDataByIndicators);

        return aggregatedData;
    }
}
