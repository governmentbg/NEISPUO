import { RIDocumentTemp } from '@domain/ri-document-temp/ri-document-temp.entity';
import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { Connection } from 'typeorm';
import { HttpMethodEnum } from '../../../../shared/enums/http-method.enum';
import { AuthObject } from '../../../../shared/interfaces/authed-request.interface';
import { AuthedRequest } from '../../../../shared/middleware/auth.middleware';
import { RIDocument } from '../../../ri-document/ri-document.entity';

@Injectable()
export class BlobGuard implements CanActivate {
    private riDocumentRepo = this.connection.getRepository(RIDocument);

    private riDocumentTempRepo = this.connection.getRepository(RIDocumentTemp);

    constructor(private connection: Connection) {}

    canActivate(ctx: ExecutionContext): Promise<boolean> {
        const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
        const authObject: AuthObject = authedRequest._authObject;
        const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

        return this.grantAccess(method, authObject, authedRequest);
    }

    private async grantAccess(
        reqMethod: HttpMethodEnum,
        authObject: AuthObject,
        req: AuthedRequest,
    ) {
        if (!authObject) {
            return false;
        }

        let accessGranted = false;
        if (reqMethod === HttpMethodEnum.POST) {
            accessGranted = this.grantCreateAccess(authObject);
        }
        if (reqMethod === HttpMethodEnum.PUT || reqMethod === HttpMethodEnum.PATCH) {
            accessGranted = this.grantUpdateAccess(authObject, req);
        }
        if (reqMethod === HttpMethodEnum.DELETE) {
            accessGranted = await this.grantDeleteAccess(authObject, req);
        }
        if (reqMethod === HttpMethodEnum.GET) {
            accessGranted = await this.grantReadAccess(authObject, req);
        }

        return accessGranted;
    }

    private grantCreateAccess(authObject: AuthObject) {
        return false;
    }

    private grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private async grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private async grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
        const blobId = +req.params.blobId;
        const currentUserMunicipalityID = authObject?.selectedRole?.MunicipalityID;

        /* eslint no-restricted-globals: ["error", "event"] */
        if (isNaN(+req.params.blobId)) {
            return false;
        }

        if (authObject.isMon || authObject.isRuo) {
            return true;
        }

        if (authObject.isMunicipality) {
            return (
                (await this.fileBelongsToUsersMunicipality(blobId, currentUserMunicipalityID))
                || (await this.isFileUploadedByMunicipality(blobId, currentUserMunicipalityID))
            );
        }

        // defaults to NO access granted, although this should not be reached.
        return false;
    }

    private async fileBelongsToUsersMunicipality(
        blobId: number,
        currentUserMunicipalityID: number,
    ): Promise<boolean> {
        const municipalityRiDocument = await this.riDocumentRepo.findOne({
            where: { DocumentFile: blobId },
            relations: [
                'RIProcedure',
                'RIProcedure.RIInstitutions',
                'RIProcedure.RIInstitutions.Town',
                'RIProcedure.RIInstitutions.Town.Municipality',
            ],
        });

        if (!municipalityRiDocument) {
            return false;
        }

        const municipalityIds = municipalityRiDocument.RIProcedure.RIInstitutions.map(
            (v) => v?.Town?.Municipality?.MunicipalityID,
        );

        const municipalityParticipatesInDocumentProcedure = municipalityIds.includes(
            currentUserMunicipalityID,
        );

        return municipalityParticipatesInDocumentProcedure;
    }

    private async isFileUploadedByMunicipality(
        blobId: number,
        currentUserMunicipalityID: number,
    ): Promise<boolean> {
        const isUploadedByCurrentMunicipality = await this.riDocumentTempRepo.findOne({
            where: { BlobId: blobId, MunicipalityID: currentUserMunicipalityID },
        });

        return !!isUploadedByCurrentMunicipality;
    }
}
