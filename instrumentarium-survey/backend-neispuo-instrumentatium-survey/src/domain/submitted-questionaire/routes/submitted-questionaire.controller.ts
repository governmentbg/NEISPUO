import { AuditAction } from '@domain/audit-logs/enums/audit-action.enum';
import { AuditObjectName } from '@domain/audit-logs/enums/audit-object.enum';
import { AuditLogsService } from '@domain/audit-logs/routes/audit-logs.service';
import { Controller, Param, UseInterceptors, Get, UseGuards, Query, Req } from '@nestjs/common';
import { ApiBearerAuth } from '@nestjs/swagger';
import {
    Crud,
    CrudController,
    ParsedRequest,
    CrudRequest,
    CrudRequestInterceptor,
    ParsedBody,
    Override
} from '@nestjsx/crud';
import { AuthedRequest } from '@shared/interfaces/authed-request.interface';
import { SubmittedQuestionaire } from '../submitted-questionaire.entity';
import { SubmittedQuestionaireGuard } from './submitted-questionaire.guard';
import { SubmittedQuestionaireService } from './submitted-questionaire.service';

@Crud({
    model: {
        type: SubmittedQuestionaire
    },
    query: {
        join: {
            campaignsId: { eager: true },
            questionaireId: { eager: true }
        }
    },
    routes: {
        only: ['getManyBase', 'getOneBase', 'createOneBase', 'updateOneBase', 'deleteOneBase']
    }
})
@ApiBearerAuth()
@UseGuards(SubmittedQuestionaireGuard)
@Controller('v1/submitted-questionaire')
export class SubmittedQuestionaireController implements CrudController<SubmittedQuestionaire> {
    get base(): CrudController<SubmittedQuestionaire> {
        return this;
    }
    constructor(
        public service: SubmittedQuestionaireService,
        public auditService: AuditLogsService
        ) {}

    @UseInterceptors(CrudRequestInterceptor)
    @Get('aggregated/:id')
    getAggregatedDataForSubmittedQuestionaires(
        @Param('id') questionaireId: number,
        @ParsedRequest() req: CrudRequest
    ) {
        return this.service.generateAggregatedDataForQuestionnaires(questionaireId);
    }

    @Override('updateOneBase')
    async updateSubmmittedQuestionaire(
        @Req() authedRequest: AuthedRequest,
        @ParsedRequest() req: CrudRequest,
        @ParsedBody() dto: SubmittedQuestionaire
    ) {

        await this.auditService.insertAudit({
            authedRequest: authedRequest, 
            action: AuditAction.UPDATE, 
            objectName: AuditObjectName.SURVEY_SUBMITTED_QUESTIONNAIRE, 
            objectID: dto.id, 
            oldValue: null,
            newValue: dto 
        });

        // if (dto.state == SubmittedQuestionaireStates.FINISHED) {
        // dto = await this.service.addCalculatedScoreToDTO(req, dto);

        // return this.base.updateOneBase(req, dto);
        // } else {
        return this.base.updateOneBase(req, dto);
        // }
    }

    @Get('whole-questionaire/:id')
    async getFullQuestionaireAndSubmittedQuestionaire(
        @Req() req: AuthedRequest,
        @Param('id') campaignId: number,
        @Query('id') id?: number
    ) {
        let submittedQuestionaire;
        if (req._authObject.isMon || req._authObject.isNio)
            submittedQuestionaire = await this.service.getSubmittedQuestionaireByID(id);
        else
            submittedQuestionaire = await this.service.getSubmittedQuestionaireByCampaignAndUser(
                req._authObject.selectedRole.SysUserID,
                campaignId,
                req._authObject.selectedRole.SysRoleID
            );
        submittedQuestionaire = await this.service.getWholeQuestionaireByCampaign(
            submittedQuestionaire
        );

        return submittedQuestionaire;
    }
}
