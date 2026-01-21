import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { IsForArchivation } from 'src/common/constants/enum/is-for-archivation.enum';
import { AzureEnrollmentsResponseDTO } from 'src/common/dto/responses/azure-enrollments-response.dto';
import { AzureEnrollmentsRepository } from '../azure-enrollments.repository';

@Injectable()
export class EnrollmentsArchiveService {
    constructor(private azureEnrollmentsRepository: AzureEnrollmentsRepository) {}

    private knownTelelinkErrors: string[] = [
        'The group must have at least one owner',
        'not found in AzureAD',
        'conflicting object with one or more of the specified property values is present in the dire',
        'already exists as member',
        'Invalid object identifier',
        'One or more removed object references do not exist for the fol',
        'One or more added object references already exist ',
        'one of the objects being referenced do not exist',
        'EducationUser with provided id ',
        'es not exist or one of its queried referen',
    ];

    isForArchivation(dtos: AzureEnrollmentsResponseDTO[]) {
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

    async archiveNotStartedWorkflows() {
        await this.azureEnrollmentsRepository.archiveNotStartedWorkflows();
    }
}
