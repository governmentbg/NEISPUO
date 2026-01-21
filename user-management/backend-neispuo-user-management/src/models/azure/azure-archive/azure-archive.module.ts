import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AzureArchiveRepository } from './azure-archive.repository';
import { AzureArchiveService } from './routing/azure-archive.service';

@Module({
    imports: [TypeOrmModule.forFeature()],
    providers: [AzureArchiveService, AzureArchiveRepository],
    exports: [AzureArchiveService],
})
export class AzureArchiveModule {}
