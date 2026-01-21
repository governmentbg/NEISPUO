import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { HandlebarsModule } from 'src/shared/services/handlebars/handlebars.module';
import { EmailTemplateType } from './email-template-type.entity';
import { ContentProvider } from './interfaces/content-provider.interface';
import { FailedSynchronizationsCP_Service } from './providers/failed-synchronizations/failed-synchronizations-cp.service';
import { EmailTemplateTypeService } from './routing/email-template-type.service';
import { EmailTemplateTypeController } from './routing/email-template-type.controller';

const ALL_CVP = [FailedSynchronizationsCP_Service] as const;

@Module({
  imports: [TypeOrmModule.forFeature([EmailTemplateType]), HandlebarsModule],
  controllers: [EmailTemplateTypeController],
  providers: [
    ...ALL_CVP,
    {
      provide: 'CONTENT_PROVIDERS',
      useFactory: (...instances: ContentProvider[]) => instances,
      inject: [...ALL_CVP],
    },
    EmailTemplateTypeService,
  ],
  exports: ['CONTENT_PROVIDERS', EmailTemplateTypeService],
})
export class EmailTemplateTypeModule {}
