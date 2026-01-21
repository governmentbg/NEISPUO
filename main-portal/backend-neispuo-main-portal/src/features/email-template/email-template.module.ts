import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { EmailTemplate } from 'src/features/email-template/email-template.entity';
import { MailerWrapperModule } from 'src/shared/services/mailer-wrapper/mailer-wrapper.module';
import { EmailTemplateTypeModule } from '../email-template-type/email-template-type.module';
import { EmailTemplateController } from './routing/email-template.controller';
import { EmailTemplateService } from './routing/email-template.service';

@Module({
  imports: [
    TypeOrmModule.forFeature([EmailTemplate]),
    EmailTemplateTypeModule,
    MailerWrapperModule,
  ],
  providers: [EmailTemplateService],
  controllers: [EmailTemplateController],
  exports: [EmailTemplateService],
})
export class EmailTemplateModule {}
