import {
    Controller,
    Get,
    Inject,
    Logger,
    NotFoundException,
    Param,
    Res,
    UseGuards,
} from '@nestjs/common';
import { Response } from 'express';
import { Connection } from 'typeorm';
import { BlobGuard } from './blob.guard';
import { BlobService } from './blob.service';

@UseGuards(BlobGuard)
@Controller('v1/blob')
export class BlobController {
    constructor(
        private connection: Connection,
        private blobService: BlobService,
        @Inject('winston') private readonly logger: Logger,
    ) {}

    @Get('meta/:blobId')
    async getBlobMeta(@Param('blobId') blobId: number) {
        const meta = this.blobService.getBlobWithContentSize(blobId);
        if (!meta) {
            throw new NotFoundException();
        }
        return meta;
    }

    @Get('download/:blobId')
    async downloadBlob(@Param('blobId') blobId: number, @Res() response: Response) {
        const location = this.blobService.getBlobLocation(blobId);
        if (!location) {
            throw new NotFoundException();
        }
        response.redirect(location);
    }

    @Get('location/:blobId')
    async getLocation(@Param('blobId') blobId: number) {
        const location = this.blobService.getBlobLocation(blobId);
        if (!location) {
            throw new NotFoundException();
        }
        return { location };
    }
}
