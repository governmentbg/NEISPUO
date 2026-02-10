/* eslint-disable @typescript-eslint/no-loop-func */
import { Injectable } from '@nestjs/common';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { AzureClassesResponseFactory } from 'src/common/factories/azure-classes-response-dto.factory';
import { ClassService } from 'src/models/class/routing/class.service';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { getManager } from 'typeorm';
import { SyncClassesAzureIDsRepository } from './sync-classes-azure-ids.repository';

@Injectable()
export class SyncClassesAzureIDsService {
    constructor(
        private syncClassesAzureIDsRepository: SyncClassesAzureIDsRepository,
        private graphApiService: GraphApiService,
        private classesService: ClassService,
    ) {}

    entityManager = getManager();

    async getAllCurriculumsWithoutAzureIDs() {
        return this.syncClassesAzureIDsRepository.getAllCurriculumsWithoutAzureIDs();
    }

    async updateCurriculumsFromAzure() {
        console.log(`Started: ${Date.now()}`);
        let curriculums;
        try {
            curriculums = await this.getAllCurriculumsWithoutAzureIDs();
            console.log(` ---- TOTAL:  ${curriculums.length} `);
        } catch (e) {
            console.log(e);
            console.log(`Exiting script due to failure `);
        }
        let total = 0;
        let successCount = 0;
        let failedCount = 0;
        let transactionFails = 0;
        const result = null;
        for (const currentElement of curriculums) {
            try {
                await this.entityManager.transaction(async (manager) => {
                    let result;
                    const { curriculumID } = currentElement;
                    total++;
                    console.log(`Total: ${total} Syncing Class: ${curriculumID}`);
                    try {
                        result = await this.graphApiService.getClassInfoByClassID(curriculumID);
                    } catch (e) {
                        console.log(e);
                        failedCount++;
                        return;
                    }
                    if (result.response.value.length == 0) {
                        await this.syncClassesAzureIDsRepository.updateCurriculum(
                            { classID: curriculumID, azureID: null },
                            manager,
                        );
                        return;
                    }
                    const curriculums = await this.classesService.getCurriculumsByCurriculumID(curriculumID);
                    const generatedClassDTO = await this.classesService.generateClassesDTOFromSubjects(curriculums);
                    const azureClassesDto = AzureClassesResponseFactory.createFromClassesResponseDTO(generatedClassDTO);
                    azureClassesDto.status = `${EventStatus.SYNCHRONIZED}`;
                    azureClassesDto.azureID = result.response.value[0].id;
                    await this.syncClassesAzureIDsRepository.insertAzureClass(azureClassesDto, manager);
                    await this.syncClassesAzureIDsRepository.updateCurriculum(azureClassesDto, manager);
                    successCount++;
                });
            } catch (ee) {
                transactionFails++;
            }
        }
        console.log(`TOTAL: ${curriculums.length}`);
        console.log(`ERROR: ${failedCount}`);
        console.log(`TRANSACTION ERROR: ${failedCount}`);
        console.log(`SUCCESS: ${successCount}`);
        console.log(`Ended: ${Date.now()}`);
    }
}
