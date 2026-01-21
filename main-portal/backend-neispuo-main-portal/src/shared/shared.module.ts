import { Module } from '@nestjs/common';
import { ConfigService } from './services/config.service';
import { MailerWrapperModule } from './services/mailer-wrapper/mailer-wrapper.module';

const CUSTOM_PROVIDERS = [
  {
    provide: ConfigService,
    useValue: new ConfigService(`config/.env`),
  },
];

@Module({
  imports: [],
  controllers: [],
  providers: [...CUSTOM_PROVIDERS, MailerWrapperModule],
  exports: [ConfigService, MailerWrapperModule],
})
export class SharedModule {}
