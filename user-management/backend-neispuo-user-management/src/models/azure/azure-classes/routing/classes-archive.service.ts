import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { IsForArchivation } from 'src/common/constants/enum/is-for-archivation.enum';
import { AzureClassesResponseDTO } from 'src/common/dto/responses/azure-classes-response.dto';
import { AzureClassesRepository } from '../azure-classes.repository';

@Injectable()
export class ClassesArchiveService {
    constructor(private azureClassesRepository: AzureClassesRepository) {}

    private knownTelelinkErrors: string[] = [
        'must not be empty',
        'Another object with the same value for propert',
        'does not exist or one of its queried refere',
        'not found in AzureAD',
        'must not be empty',
        'Sequence contains more than one element',
        'Conflicting object with one or more of the specified property',
    ];

    isForArchivation(dtos: AzureClassesResponseDTO[]) {
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
