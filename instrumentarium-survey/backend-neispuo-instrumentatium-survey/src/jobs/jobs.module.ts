import { AgreggatedResultsJobService } from "@domain/aggregated-results/aggregated-results-job/aggregated-results-job.service";
import { TeachersQuestionaireReminderService } from "@domain/notifications/teachers-questionaire-reminder/routes/teachers-questionaire-reminder.service";
import { Module } from "@nestjs/common";
import { DSSMailService } from "@shared/services/dss-mail/dss-mail.service";
import { LoggerService } from "@shared/services/logger/logger.service";
import { SetCampaignsisActiveStatus } from "src/jobs/scripts/set-campaigns-is-active-status.job";
import { SendEmailReminderForCampaign } from "./scripts/send-email-reminder-for-campaign.job";

@Module({
  exports: [
    LoggerService,
    AgreggatedResultsJobService
  ],
  providers: [
    LoggerService,
    DSSMailService,
    TeachersQuestionaireReminderService,
    SetCampaignsisActiveStatus,
    SendEmailReminderForCampaign,
    AgreggatedResultsJobService
  ],
})

export class JobsModule {}
