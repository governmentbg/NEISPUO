import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { Filter } from 'src/common/dto/filter.dto';
import { Paging } from 'src/common/dto/paging.dto';
import { AzureOrganizationsResponseDTO } from 'src/common/dto/responses/azure-organizations-response.dto';
import { InstitutionResponseDTO } from 'src/common/dto/responses/institution-response.dto';
import { DataNotFoundException } from 'src/common/exceptions/data-not-found.exception';
import { EntityNotCreatedException } from 'src/common/exceptions/entity-not-created.exception';
import { AzureOrganizationResponseFactory } from 'src/common/factories/azure-organization-response-dto.factory';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { InstitutionService } from 'src/models/institution/routing/institution.service';
import { SyncAzureOrganizationsRepository } from './sync-azure-organizations.repository';

@Injectable()
export class SyncAzureOrganizationsService {
    constructor(
        private syncAzureOrganizationsRepository: SyncAzureOrganizationsRepository,
        private institutionService: InstitutionService,
        private graphApiService: GraphApiService,
    ) {}

    async organizationExists(dto: InstitutionResponseDTO) {
        const { institutionID } = dto;
        const azureOrganizationDTO = AzureOrganizationResponseFactory.createFromInstitutionResponseDTO({
            institutionID,
        });
        const result = await this.syncAzureOrganizationsRepository.getAllAzureOrganizationsByOrganizationID(
            azureOrganizationDTO,
        );
        if (result?.length > 0) {
            return true;
        }
        return false;
    }

    async syncAzureOrganization(dto: AzureOrganizationsResponseDTO) {
        return this.syncAzureOrganizationsRepository.syncAzureOrganization(dto);
    }

    async getAllInstitutions(paging: Paging, filters: Filter[]) {
        const result = await this.syncAzureOrganizationsRepository.getAllInstitutionsAccounts(paging, filters);
        return result;
    }

    async getAllInstitutionsCount(filters: Filter[]) {
        const result = await this.syncAzureOrganizationsRepository.getAllInstitutionsAccountsCount(filters);
        return result;
    }

    async insertAzureOrganizationsIntoOrganizations(dto: InstitutionResponseDTO) {
        const institution = await this.institutionService.getInstitutionByInstitutionID(dto.institutionID);
        if (!institution?.institutionID) throw new DataNotFoundException();
        const azureOrganizationDTO = AzureOrganizationResponseFactory.createFromInstitutionResponseDTO(institution);
        azureOrganizationDTO.status = EventStatus.SYNCHRONIZED.toString();
        const username = `${azureOrganizationDTO.organizationID}${CONSTANTS.DOMAIN_NAME_PRODUCTION}`;
        azureOrganizationDTO.username = username;
        const result = await this.syncAzureOrganization(azureOrganizationDTO);
        if (!result?.rowID) throw new EntityNotCreatedException();
        return {
            data: {
                [CONSTANTS.RESPONSE_PARAM_NAME_ORGANIZATION_CREATED]: result.rowID,
            },
        };
    }

    async syncExistingNEISPUOOrganizationsWithAzureOrganizations() {
        console.log(`Started: ${Date.now()}`);
        // eslint-disable-next-line prettier/prettier
        const institutionsCount = await this.getAllInstitutionsCount([{ value: 'ss', matchMode: 'd' }]);
        const itemsPerPage = CONSTANTS.DB_QUERY_PARAM_ROWS_PER_PAGE_GET_PERSONAL_IDS;

        // estimated to run for 30 seconds for every 100 personalIDs.
        // 2 days for 500 000 aproximatelyconst
        const totalCount = institutionsCount;
        let currentTotalCount = institutionsCount;
        let currentArrayIndex = 0;
        let syncedTotalCount = 0;
        let failedTotalCount = 0;
        while (currentTotalCount > 0) {
            let schools;
            try {
                console.log(`CURRENT: ${currentArrayIndex} ---- TOTAL:  ${totalCount} `);
                schools = await this.getAllInstitutions(
                    {
                        from: failedTotalCount,
                        numberOfElements: itemsPerPage,
                    },
                    [],
                );
            } catch (e) {
                console.log(e);
                console.log(`Exiting script due to faulure `);
                break;
            }
            // take small portions of data to avoid blocking the app.
            for (const currentElement of schools) {
                let result, result2, result3;
                const institutionID = currentElement.institutionID;
                currentArrayIndex = syncedTotalCount + failedTotalCount;
                currentTotalCount--;
                try {
                    result = await this.organizationExists({ institutionID });
                } catch (e) {
                    console.log(e);
                    failedTotalCount++;
                    continue;
                }
                if (result) {
                    console.log(`${currentArrayIndex}. Skipping. InstitutionID: ${institutionID} exists.`);
                    failedTotalCount++;
                    continue;
                }
                try {
                    result2 = await this.graphApiService.getSchoolInfo(institutionID);
                } catch (e) {
                    console.log(e);
                    failedTotalCount++;
                    continue;
                }
                // if there is no graph api result dont update DB
                if (!result2?.response) {
                    console.log(`${currentArrayIndex}. Skipping. No Azure object for institutionID: ${institutionID}`);
                    failedTotalCount++;
                    continue;
                }
                try {
                    result3 = await this.insertAzureOrganizationsIntoOrganizations({
                        institutionID,
                    });
                } catch (e) {
                    console.log(`${currentArrayIndex}. Failed to sync school: ${institutionID}`);
                    failedTotalCount++;
                    continue;
                }
                syncedTotalCount++;
                console.log(`${currentArrayIndex}. Synced school: ${institutionID}`);
            }
        }
        console.log(`Ended: ${Date.now()}`);
    }
}
