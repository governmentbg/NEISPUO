import { Module } from '@nestjs/common';
import { EsuiCampaignController } from './routes/esui-campaign.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { CampaignService } from '@domain/campaigns/routes/campaign.service';
import { Campaign } from '@domain/campaigns/campaign.entity';
import { TeachersQuestionaireReminderService } from '@domain/notifications/teachers-questionaire-reminder/routes/teachers-questionaire-reminder.service';
import { DSSMailService } from '@shared/services/dss-mail/dss-mail.service';
import { EsuiCampaignService } from './routes/esui-campaign.service';
import { AggregatedResultsController } from '@domain/aggregated-results/routes/aggregated-results.controller';
import { AggregatedResultsService } from '@domain/aggregated-results/routes/aggregated-results.service';
import { AggregatedResults } from '@domain/aggregated-results/aggregated-results.entity';
import { LoggerService } from 'src/shared/services/logger/logger.service';
import { AgreggatedResultsJobService } from '@domain/aggregated-results/aggregated-results-job/aggregated-results-job.service';
import { AuditLogsService } from '@domain/audit-logs/routes/audit-logs.service';


@Module({
    imports: [
        TypeOrmModule.forFeature([Campaign]), 
        TypeOrmModule.forFeature([AggregatedResults])
    ],
    exports: [
        EsuiCampaignService, 
        CampaignService, 
        TeachersQuestionaireReminderService, 
        DSSMailService, 
        AggregatedResultsController, 
        AggregatedResultsService,
        LoggerService,
        AgreggatedResultsJobService,
        AuditLogsService
    ],

    controllers: [EsuiCampaignController],
    providers: [
        EsuiCampaignService, 
        CampaignService, 
        TeachersQuestionaireReminderService, 
        DSSMailService, 
        AggregatedResultsController,
        AggregatedResultsService,
        LoggerService,
        AgreggatedResultsJobService,
        AuditLogsService
    ]
})
export class EsuiCampaignModule {}
