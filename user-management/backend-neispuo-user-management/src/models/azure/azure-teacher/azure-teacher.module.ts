import { Module } from '@nestjs/common';
import { TransactionEventHandlerModule } from 'src/models/transaction-event-handler/transaction-event-handler.module';
import { AzureTeachersRepository } from './azure-teacher.repository';
import { AzureTeacherController } from './routing/azure-teacher.controller';
import { AzureTeacherService } from './routing/azure-teacher.service';

@Module({
    imports: [TransactionEventHandlerModule],
    controllers: [AzureTeacherController],
    providers: [AzureTeacherService, AzureTeachersRepository],
    exports: [AzureTeacherService],
})
export class AzureTeacherModule {}
