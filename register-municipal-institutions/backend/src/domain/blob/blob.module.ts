import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { SharedModule } from '../../shared/shared.module';
import { BlobEntity } from './blob.entity';
import { BlobController } from './routes/blob/blob.controller';
import { BlobService } from './routes/blob/blob.service';

@Module({
    imports: [
        TypeOrmModule.forFeature([BlobEntity], process.env.BLOBS_DB_CONNECTION),
        SharedModule,
    ],
    exports: [TypeOrmModule],

    controllers: [BlobController],
    providers: [BlobService],
})
export class BlobModule {}
