import {
    Controller,
    Delete,
    UseInterceptors,
    Query,
    UseGuards,
    Res,
    HttpStatus,
    BadRequestException,
    Get,
    Param,
    Req
} from '@nestjs/common';
import { Response } from 'express';
import { ApiBearerAuth } from '@nestjs/swagger';
import {
    Crud,
    CrudController,
    CrudRequest,
    CrudRequestInterceptor,
    Override,
    ParsedBody,
    ParsedRequest
} from '@nestjsx/crud';
import { Campaign } from '../campaign.entity';
import { CampaignGuard } from './campaign.guard';
import { CampaignService } from './campaign.service';
import { LoggedUser } from '@shared/decorators/logged-user.decorator';
import { LoggedUserModel } from '@shared/models/selected-role.model';
import moment from 'moment';
import { AuthedRequest } from '@shared/interfaces/authed-request.interface';
import { AuditLogsService } from '@domain/audit-logs/routes/audit-logs.service';
import { AuditAction } from '@domain/audit-logs/enums/audit-action.enum';
import { AuditModuleEnum } from '@domain/audit-logs/enums/audit-module.enum';
import { AuditObjectName } from '@domain/audit-logs/enums/audit-object.enum';

@Crud({
    model: {
        type: Campaign
    },
    query: {
        join: {
            createdBy: { eager: true },
            SubmittedQuestionaires: { eager: true, exclude: ['submittedQuestionaireObject', 'userId'] },
            'SubmittedQuestionaires.questionaireId': {eager: true}
        }
    },
    routes: {
        only: ['getManyBase', 'getOneBase', 'createOneBase', 'updateOneBase', 'deleteOneBase']
    }
})
@UseGuards(CampaignGuard)
@ApiBearerAuth()
@Controller('v1/campaign')
export class CampaignController implements CrudController<Campaign> {
    get base(): CrudController<Campaign> {
        return this;
    }

    constructor(
        public service: CampaignService,
        public auditService: AuditLogsService
        ) {}

    @UseInterceptors(CrudRequestInterceptor)
    @Delete('delete-many')
    async deleteManyCampaigns(@Query('campaign') campaigns: string[]) {
        return await this.service.deleteManyCampaigns(campaigns);
    }

    @Override('deleteOneBase')
    async deleteCampaign(@ParsedRequest() req: CrudRequest) {
        return await this.service.deleteCampaign(req, this.base);
    }

    @Override('updateOneBase')
    async updateCampaign(
        @Req() authedRequest: AuthedRequest, 
        @ParsedRequest() req: CrudRequest, 
        @ParsedBody() dto: Campaign, 
        @Param('id') id: number
            ) {
        if(dto.startDate) {
            if(!this.service.validateCampaignStartDateOnEdit(dto)) {
                throw new BadRequestException('Invalid update parameters');
            }
        }

        await this.auditService.insertAudit({
            authedRequest: authedRequest, 
            action: AuditAction.UPDATE, 
            objectName: AuditObjectName.SURVEY_CAMPAIGN, 
            objectID: dto.id, 
            oldValue: null, 
            newValue: dto 
        });

        return await this.service.updateCampaign(dto, id);
    }

    @Override('getManyBase')
    async getAll(@ParsedRequest() req: CrudRequest, @Query('years') years: string) {
        return await this.service.getAll(req, JSON.parse(years), this.base);
    }

    @Override('createOneBase')
    async createCampaign(
        @Req() authedRequest: AuthedRequest,
        @LoggedUser() loggedUser: LoggedUserModel, 
        @ParsedBody() dto: Campaign, @Res() res: Response
        ) {

        const newCampaign = await this.service.createCampaign(dto, loggedUser);
        // send response fot successfully created campaign and continue with sending emails
        res.status(HttpStatus.CREATED).json(newCampaign);
        
        await this.auditService.insertAudit({ 
            authedRequest: authedRequest, 
            action: AuditAction.INSERT, 
            objectName: AuditObjectName.SURVEY_CAMPAIGN, 
            objectID: newCampaign.id, 
            oldValue: null, 
            newValue: newCampaign 
        });

        const startDate = moment(newCampaign.startDate);
        const today = moment();
        
        if(startDate.isSame(today, 'day')) {
            await this.service.sendEmailsForNewlyCreatedCampaign(dto, newCampaign);
        }
    }

    @Get('aggregated')
    async getAggregatedData(@ParsedRequest() req: CrudRequest, @Query('campaignId') campaignId: number) {
        return await this.service.getAggregatedDataForCampaign(campaignId);
    }
}
