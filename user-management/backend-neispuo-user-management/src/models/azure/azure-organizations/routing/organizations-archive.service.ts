import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { IsForArchivation } from 'src/common/constants/enum/is-for-archivation.enum';
import { AzureOrganizationsResponseDTO } from 'src/common/dto/responses/azure-organizations-response.dto';
import { AzureOrganizationsRepository } from '../azure-organizations.repository';

@Injectable()
export class OrganizationsArchiveService {
    constructor(private azureOrganizationsRepository: AzureOrganizationsRepository) {}

    private knownTelelinkErrors: string[] = [];

    isForArchivation(dtos: AzureOrganizationsResponseDTO[]) {
        for (const dto of dtos) {
            if (dto?.inProgressResultCount >= Number(CONSTANTS.JOBS_IN_PROGRESS_RESULT_COUNT_LIMIT) - 1) {
                dto.isForArchivation = IsForArchivation.YES;
                continue;
            }
            if (dto?.retryAttempts >= Number(CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT) - 1) {
                for (const knownTelelinkError of this.knownTelelinkErrors) {
                    if (dto.errorMessage?.includes(knownTelelinkError)) {
                        dto.isForArchivation = IsForArchivation.YES;
                        break;
                    }
                }
            }
        }
    }
}
