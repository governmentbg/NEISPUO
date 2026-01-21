import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { AzureEnrollmentsResponseDTO } from 'src/common/dto/responses/azure-enrollments-response.dto';
import { AzureEnrollmentsRepository } from '../azure-enrollments.repository';

@Injectable()
export class EnrollmentsRestartService {
    constructor(private azureEnrollmentsRepository: AzureEnrollmentsRepository) {}

    private knownTelelinkErrors: string[] = [
        'Service Unavailable',
        'Request failed with status code 502',
        'Failed to execute',
        'Cannot create a team',
        'Code: UnknownError',
        'Error: Request failed with status code 500',
        'n error occurred sending the request',
        'The request timed out',
        'ailed to execute Templates backend request CreateTeamFromGroupWithTemplateRequest',
        `read ECONNRESET`,
        `ETIMEDOUT`,
        `ECONNREFUSED`,
        'more than 3',
    ];

    isForRestart(dto: AzureEnrollmentsResponseDTO) {
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

    async restartFailedWorkflows(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        await this.azureEnrollmentsRepository.restartFailedWorkflows(dtos);
    }
}
