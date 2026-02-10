import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { InsertAzureCreateTaskRunner } from './insert-azure-create.command';
import { InsertAzureCreateRepository } from './insert-azure-create.repository';
import { InsertAzureCreateService } from './insert-azure-create.service';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule],
    providers: [InsertAzureCreateService, InsertAzureCreateRepository, InsertAzureCreateTaskRunner],
})
export class InsertAzureCreateModule {}
