import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { Paging } from 'src/common/dto/paging.dto';
import { SysUserResponseDTO } from 'src/common/dto/responses/sys-user.response.dto';
import { AzureTeacherService } from 'src/models/azure/azure-teacher/routing/azure-teacher.service';
import { SyncAzureAccountantsRepository } from './sync-azure-accountants.repository';

@Injectable()
export class SyncAzureAccountantsService {
    constructor(
        private syncAzureAccountantsRepository: SyncAzureAccountantsRepository,
        private azureTeacherService: AzureTeacherService,
    ) {}

    async getAllAccountantsCount() {
        return this.syncAzureAccountantsRepository.getAllAccountantsCount();
    }

    async getPaginatedAccountants(paging: Paging) {
        return this.syncAzureAccountantsRepository.getPaginatedAccountants(paging);
    }

    async syncAccountantsWithAzure() {
        console.log(`Started: ${Date.now()}`);
        const institutionsCount = await this.syncAzureAccountantsRepository.getAllAccountantsCount();
        const itemsPerPage = CONSTANTS.DB_QUERY_PARAM_ROWS_PER_PAGE_GET_PERSONAL_IDS;
        const totalPages = institutionsCount / itemsPerPage;
        const totalCount = institutionsCount;
        let currentTotalCount = institutionsCount;
        let currentArrayIndex = 0;
        let syncedTotalCount = 0;
        let failedTotalCount = 0;
        while (currentTotalCount > 0) {
            let accountants: SysUserResponseDTO[];
            try {
                console.log(`CURRENT: ${currentArrayIndex} ---- TOTAL:  ${totalCount} `);
                accountants = await this.syncAzureAccountantsRepository.getPaginatedAccountants({
                    from: failedTotalCount,
                    numberOfElements: itemsPerPage,
                });
            } catch (e) {
                console.log(e);
                console.log(`Exiting script due to faulure `);
                break;
            }

            for (const accountant of accountants) {
                let result;
                const { sysUserID } = accountant;
                currentArrayIndex = syncedTotalCount + failedTotalCount;
                currentTotalCount--;
                try {
                    result = await this.azureTeacherService.adjustAccountantRole({ sysUserID });
                } catch (e) {
                    console.log(e);
                }
                if (!result?.data) {
                    console.log(`${currentArrayIndex}. Skipping. Cannot adjust sysUserID: ${sysUserID}`);
                    failedTotalCount++;
                    continue;
                }
                syncedTotalCount++;
                console.log(`${currentArrayIndex}. Synced accountant: ${sysUserID}`);
            }
        }
    }
}
