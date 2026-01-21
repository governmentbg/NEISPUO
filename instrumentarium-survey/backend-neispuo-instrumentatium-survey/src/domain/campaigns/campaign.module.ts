import { AuditLogsService } from '@domain/audit-logs/routes/audit-logs.service';
import { TeachersQuestionaireReminderService } from '@domain/notifications/teachers-questionaire-reminder/routes/teachers-questionaire-reminder.service';
import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { DSSMailService } from '@shared/services/dss-mail/dss-mail.service';
import { Campaign } from './campaign.entity';
import { CampaignController } from './routes/campaign.controller';
import { CampaignService } from './routes/campaign.service';

@Module({
    imports: [TypeOrmModule.forFeature([Campaign])],
    exports: [CampaignService, TeachersQuestionaireReminderService, DSSMailService, AuditLogsService],
    controllers: [CampaignController],
    providers: [CampaignService, TeachersQuestionaireReminderService, DSSMailService, AuditLogsService]
})
export class CampaignModule {}
