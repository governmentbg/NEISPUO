import { Injectable } from '@nestjs/common';
import { IsForArchivation } from 'src/common/constants/enum/is-for-archivation.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { AzureClassesResponseDTO } from 'src/common/dto/responses/azure-classes-response.dto';
import { AzureClassesMapper } from 'src/common/mappers/azure-classes.mapper';
import { Connection, EntityManager, getManager } from 'typeorm';

@Injectable()
export class SyncClassesAzureIDsRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getAllCurriculumsWithoutAzureIDs() {
        //write your query to select missing curriculums. all of the rest of the code should be unchanged

        const result = await getManager().query(
            `
            select DISTINCT c.curriculumID from azure_temp.Enrollments c WHERE
            Status =2 and ErrorMessage LIKE '%EducationClass with prov%'
            `,
        );
        return result;
    }

    async insertAzureClass(dto: AzureClassesResponseDTO, entityManager?: EntityManager) {
        const { title, orgID, termStartDate, termEndDate, classID, status, azureID } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `           
                INSERT
                INTO
                azure_temp.Classes (
                    WorkflowType,
                    Title,
                    OrgID,
                    TermStartDate,
                    TermEndDate,
                    ClassID,
                    Status,
                    AzureID,
                    IsForArchivation
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8);
            `,
            [
                WorkflowType.CLASS_CREATE,
                title,
                orgID,
                termStartDate,
                termEndDate,
                classID,
                status,
                azureID,
                IsForArchivation.YES,
            ],
        );
        const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(result);
        return transformedResult[0];
    }

    async updateCurriculum(dto: AzureClassesResponseDTO, entityManager?: EntityManager) {
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `           
                UPDATE
                    inst_year.Curriculum 
                SET
                    AzureID = @0
                WHERE
                    CurriculumID =@1
            `,
            [dto.azureID, dto.classID],
        );
    }
}
