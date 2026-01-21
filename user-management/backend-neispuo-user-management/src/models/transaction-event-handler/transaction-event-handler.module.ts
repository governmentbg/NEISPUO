import { Module } from '@nestjs/common';
import { TransactionEventHandlerService } from './transaction-event-handler.service';
import { SIEMLoggerModule } from '../siem-logger/siem-logger.module';

@Module({
    imports: [SIEMLoggerModule.forFeature()],
    providers: [TransactionEventHandlerService],
    exports: [TransactionEventHandlerService],
})
export class TransactionEventHandlerModule {}
