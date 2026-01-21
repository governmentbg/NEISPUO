import { Module } from '@nestjs/common';
import { ErrorNotificationService } from './routing/error-notification.service';

@Module({
    providers: [ErrorNotificationService],
    exports: [ErrorNotificationService],
})
export class ErrorNotificationModule {}
