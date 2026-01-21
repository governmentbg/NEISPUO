import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { SharedModule } from '../../shared/shared.module';
import { RIDocumentTemp } from './ri-document-temp.entity';
import { BlobTempController } from './routes/blob-temp/blob-temp.controller';
import { BlobTempService } from './routes/blob-temp/blob-temp.service';

@Module({
    imports: [TypeOrmModule.forFeature([RIDocumentTemp]), SharedModule],
    exports: [TypeOrmModule],

    controllers: [BlobTempController],
    providers: [BlobTempService],
})
export class BlobTempModule {}
