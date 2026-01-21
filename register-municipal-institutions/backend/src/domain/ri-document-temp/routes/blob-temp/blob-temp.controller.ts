import {
 Body, Controller, Inject, Logger, NotFoundException, Post, Req,
} from '@nestjs/common';
import { AuthedRequest } from '@shared/middleware/auth.middleware';
import { Connection } from 'typeorm';
import { BlobTempService } from './blob-temp.service';

@Controller('v1/blob-temp')
export class BlobTempController {
    constructor(
        private connection: Connection,
        private blobService: BlobTempService,
        @Inject('winston') private readonly logger: Logger,
    ) {}

    @Post('create')
    async addTempBlobID(@Req() authedRequest: AuthedRequest, @Body() body: any) {
        const createTempBlob = this.blobService.addTempBlobID(
            authedRequest,
            body.blobId,
            this.connection,
        );
        if (!createTempBlob) {
            throw new NotFoundException();
        }
        return createTempBlob;
    }
}
