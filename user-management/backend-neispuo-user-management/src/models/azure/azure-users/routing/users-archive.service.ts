import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { IsForArchivation } from 'src/common/constants/enum/is-for-archivation.enum';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { AzureUsersRepository } from '../azure-users.repository';

@Injectable()
export class UsersArchiveService {
    constructor(private azureUsersRepository: AzureUsersRepository) {}

    private knownTelelinkErrors: string[] = [
        'must not be empty',
        'not found in AzureAD',
        'MUST BETWEEN 1-100 CHARS',
        'GUID is not found in Azu',
        'ROR WITH PARENT NAME GENERATION OR PARENT ALREADY EXISTS',
        'not found in AzureAD',
        'INVALID PARAM',
        'The duplicate key value is',
        'nvalid request parameters provided for user',
        'Cannot read property',
    ];

    isForArchivation(dtos: AzureUsersResponseDTO[]) {
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
