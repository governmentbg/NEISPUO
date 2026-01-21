import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { AuthedRequest } from '@shared/middleware/auth.middleware';
import { Connection, Repository } from 'typeorm';
import { RIDocumentTemp } from '../../ri-document-temp.entity';

@Injectable()
export class BlobTempService extends TypeOrmCrudService<RIDocumentTemp> {
    constructor(
        @InjectRepository(RIDocumentTemp)
        public repo: Repository<RIDocumentTemp>,
    ) {
        super(repo);
    }

    async addTempBlobID(
        authedRequest: AuthedRequest,
        blobId: number,
        transactionManager: Connection,
    ) {
        await transactionManager.getRepository(RIDocumentTemp).insert({
            BlobId: blobId,
            MunicipalityID: authedRequest._authObject.selectedRole.MunicipalityID,
        });

        return true;
    }
}
