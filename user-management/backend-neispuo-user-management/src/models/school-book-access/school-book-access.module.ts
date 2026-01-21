import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { PersonnelSchoolBookAccess } from 'src/common/entities/personnel-school-book-access.entity';
import { TransactionEventHandlerModule } from '../transaction-event-handler/transaction-event-handler.module';
import { SchoolBookAccessController } from './routing/school-book-access.controller';
import { SchoolBookAccessService } from './routing/school-book-access.service';

@Module({
    imports: [TransactionEventHandlerModule, TypeOrmModule.forFeature([PersonnelSchoolBookAccess])],
    providers: [SchoolBookAccessService],
    controllers: [SchoolBookAccessController],
    exports: [SchoolBookAccessService],
})
export class SchoolBookAccessModule {}
