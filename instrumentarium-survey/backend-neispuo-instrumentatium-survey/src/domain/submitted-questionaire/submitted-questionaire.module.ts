import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';

import { SubmittedQuestionaireService } from './routes/submitted-questionaire.service';
import { SubmittedQuestionaireController } from './routes/submitted-questionaire.controller';
import { SubmittedQuestionaire } from './submitted-questionaire.entity';
import { AuditLogsService } from '@domain/audit-logs/routes/audit-logs.service';

@Module({
  imports: [TypeOrmModule.forFeature([SubmittedQuestionaire])],
  exports: [TypeOrmModule],

  controllers: [SubmittedQuestionaireController],
  providers: [SubmittedQuestionaireService, AuditLogsService]
})
export class SubmittedQuestionaireModule {}
