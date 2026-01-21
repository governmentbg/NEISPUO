import { Module, forwardRef } from '@nestjs/common';
import { StudentModule } from 'src/models/student/student.module';
import { TransactionEventHandlerModule } from 'src/models/transaction-event-handler/transaction-event-handler.module';
import { AzureEnrollmentsModule } from '../azure-enrollments/azure-enrollments.module';
import { AzureStudentRepository } from './azure-student.repository';
import { AzureStudentController } from './routing/azure-student.controller';
import { AzureStudentService } from './routing/azure-student.service';

@Module({
    imports: [AzureEnrollmentsModule, TransactionEventHandlerModule, forwardRef(() => StudentModule)],
    controllers: [AzureStudentController],
    providers: [AzureStudentService, AzureStudentRepository],
    exports: [AzureStudentService],
})
export class AzureStudentModule {}
