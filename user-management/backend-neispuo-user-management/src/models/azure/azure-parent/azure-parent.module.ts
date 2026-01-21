import { Module } from '@nestjs/common';
import { TransactionEventHandlerModule } from 'src/models/transaction-event-handler/transaction-event-handler.module';
import { AzureParentRepository } from './azure-parent.repository';
import { AzureParentController } from './routing/azure-parent.controller';
import { AzureParentService } from './routing/azure-parent.service';

@Module({
    imports: [TransactionEventHandlerModule],
    controllers: [AzureParentController],
    providers: [AzureParentService, AzureParentRepository],
    exports: [AzureParentService],
})
export class AzureParentModule {}
