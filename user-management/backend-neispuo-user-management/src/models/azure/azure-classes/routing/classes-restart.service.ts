import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { AzureClassesResponseDTO } from 'src/common/dto/responses/azure-classes-response.dto';
import { AzureClassesRepository } from '../azure-classes.repository';

@Injectable()
export class ClassesRestartService {
    constructor(private azureClassesRepository: AzureClassesRepository) {}

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

    isForRestart(dto: AzureClassesResponseDTO) {
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

    async restartFailedWorkflows(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        await this.azureClassesRepository.restartFailedWorkflows(dtos);
    }
}
