import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { AzureUsersRepository } from '../azure-users.repository';

@Injectable()
export class UsersRestartService {
    constructor(private azureUsersRepository: AzureUsersRepository) {}

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

    isForRestart(dto: AzureUsersResponseDTO) {
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

    async restartFailedWorkflows(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        await this.azureUsersRepository.restartFailedWorkflows(dtos);
    }
}
