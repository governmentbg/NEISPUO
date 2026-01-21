import { Module } from '@nestjs/common';
import { TeachersQuestionaireReminderService } from './routes/teachers-questionaire-reminder.service';
import { DSSMailService } from '../../../shared/services/dss-mail/dss-mail.service';

@Module({
    imports: [DSSMailService],
    exports: [],
    controllers: [],
    providers: [TeachersQuestionaireReminderService, DSSMailService]
})
export class TeachersQuestionaireReminderModule {}
