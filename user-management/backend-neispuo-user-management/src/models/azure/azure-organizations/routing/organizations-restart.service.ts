import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { AzureOrganizationsResponseDTO } from 'src/common/dto/responses/azure-organizations-response.dto';
import { AzureOrganizationsRepository } from '../azure-organizations.repository';

@Injectable()
export class OrganizationsRestartService {
    constructor(private azureOrganizationsRepository: AzureOrganizationsRepository) {}

    private knownTelelinkErrors: string[] = [
        'Service Unavailable',
        'Request failed with status code 502',
        'Failed to execute',
        'Code: UnknownError',
        'Error: Request failed with status code 500',
        'n error occurred sending the request',
        'The request timed out',
        `read ECONNRESET`,
        `ETIMEDOUT`,
        `ECONNREFUSED`,
        'more than 3',
    ];

    isForRestart(dto: AzureOrganizationsResponseDTO) {
        let result = false;
        if (dto?.retryAttempts >= Number(CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT) - 1) {
            for (const knownTelelinkError of this.knownTelelinkErrors) {
                if (dto.errorMessage?.includes(knownTelelinkError)) {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }

    async restartFailedWorkflows(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        await this.azureOrganizationsRepository.restartFailedWorkflows(dtos);
    }
}
