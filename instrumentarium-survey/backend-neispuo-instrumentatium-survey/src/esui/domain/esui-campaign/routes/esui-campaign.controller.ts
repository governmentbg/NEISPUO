import { Campaign } from '../../../../domain/campaigns/campaign.entity';
import { CampaignService } from '../../../../domain/campaigns/routes/campaign.service';
import { Controller, Get, Param, Req, UseGuards, UseInterceptors } from '@nestjs/common';
import { ApiBearerAuth } from '@nestjs/swagger';
import { Crud, CrudController, CrudRequest, CrudRequestInterceptor, Override, ParsedBody, ParsedRequest } from '@nestjsx/crud';
import { EsuiGuard } from '../../esui.guard';
import { EsuiCampaignService } from './esui-campaign.service';
import { AgreggatedResultsJobService } from '@domain/aggregated-results/aggregated-results-job/aggregated-results-job.service';
import { AuditLogsService } from '@domain/audit-logs/routes/audit-logs.service';
import { AuditAction } from '@domain/audit-logs/enums/audit-action.enum';
import { AuditObjectName } from '@domain/audit-logs/enums/audit-object.enum';
import { AuthedRequest } from '@shared/interfaces/authed-request.interface';

@Crud({
    model: {
        type: Campaign
    },
    routes: {
        only: ['getManyBase', 'getOneBase', 'createOneBase', 'replaceOneBase', 'updateOneBase']
    }
})

@UseGuards(EsuiGuard)
@ApiBearerAuth()
@Controller('esui/campaign')
export class EsuiCampaignController implements CrudController<Campaign> {
    get base(): CrudController<Campaign> {
        return this;
    }
    constructor(
        public service: EsuiCampaignService, 
        public campaignService: CampaignService,
        public aggrService: AgreggatedResultsJobService,
        public auditService: AuditLogsService
        ) { }

    @Override('createOneBase')
    async createOne(@Req() authedRequest: AuthedRequest, @ParsedRequest() req: CrudRequest, @ParsedBody() dto: Campaign) {
        dto = this.campaignService.hardcodePropertiesForESUICampaignCreation(dto);
        const newCampaign = await this.campaignService.createCampaign(dto, {});
        
        await this.auditService.insertAudit({
            authedRequest: authedRequest, 
            action: AuditAction.INSERT, 
            objectName: AuditObjectName.ESUI_CAMPAIGN, 
            objectID: newCampaign.id, 
            oldValue: null, 
            newValue: newCampaign 
        });

        return newCampaign;
    }

    @Override('replaceOneBase')
    async replaceCampaign(@Req() authedRequest: AuthedRequest, @ParsedRequest() req: CrudRequest, @ParsedBody() dto: Campaign) {
        // ESUI wants to make PUT Request, but send only part of the parameters (DB columns)
        // that's why we need to override replaceOneBase and then use the updateOneBase functionality
        // in fact we are faking a PATCH request for the CRUD package

        let newDto = await this.base.updateOneBase(req, dto);
        
        await this.auditService.insertAudit({
            authedRequest: authedRequest, 
            action: AuditAction.UPDATE, 
            objectName: AuditObjectName.ESUI_CAMPAIGN, 
            objectID: newDto.id, 
            oldValue: null, 
            newValue: newDto 
        });

        return newDto;
    }

    @UseInterceptors(CrudRequestInterceptor)
    @Get('aggregated/:id')
    async getAggregatedDataForSubmittedQuestionaires(
        @ParsedRequest() req: CrudRequest,
        @Param('id') campaignId: number
    ) {
        await this.aggrService.generateAggregatedDataForExpiredCampaigns();
             
        return await this.service.getAggregatedResultsForCampaign(campaignId);
    }
}
