import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { Repository } from 'typeorm';
import * as crypto from 'crypto';
import { BlobEntity } from '../../blob.entity';

@Injectable()
export class BlobService extends TypeOrmCrudService<BlobEntity> {
    constructor(
        @InjectRepository(BlobEntity, process.env.BLOBS_DB_CONNECTION)
        public repo: Repository<BlobEntity>,
    ) {
        super(repo);
    }

    async getBlobWithContentSize(blobId: number): Promise<BlobEntity> {
        const blob = await this.repo
            .createQueryBuilder('blob')
            .innerJoin(
                'blob.BlobContent',
                'blobContent',
                'blob.BlobContentId = blobContent.BlobContentId',
            )
            .select(['blob.BlobId', 'blob.FileName', 'blobContent.Size'])
            .where('blob.BlobId = :blobId', { blobId })
            .getOne();

        return blob;
    }

    getBlobLocation(blobId: number) {
        // Bellow adapted from demo code: https://github.com/Neispuo/school-books/blob/dev/blobs/samples/expressjs/index.js
        const HMACKey = process.env.HMAC_KEY;
        const blobServiceUrl = process.env.BLOB_SERVER_URL;

        const unixTimeSeconds = Math.floor(Date.now() / 1000);
        const message = `${blobId}/${unixTimeSeconds}`;

        const hmac = crypto.createHmac('sha256', HMACKey).update(message);
        const urlSafeBase64HMAC = hmac
            .digest('base64')
            // Url-safe Base64 / RFC 4648
            // https://tools.ietf.org/html/rfc4648
            .replace('+', '-')
            .replace('/', '_')
            .replace(/=+$/, '');

        const location = `${blobServiceUrl}/${blobId}?t=${unixTimeSeconds}&h=${urlSafeBase64HMAC}`;
        return location;
    }
}
