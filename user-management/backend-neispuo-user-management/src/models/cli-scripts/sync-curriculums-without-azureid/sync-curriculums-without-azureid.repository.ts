import { Injectable } from '@nestjs/common';
import { InjectConnection } from '@nestjs/typeorm';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { Paging } from 'src/common/dto/paging.dto';
import { AzureClassesResponseDTO } from 'src/common/dto/responses/azure-classes-response.dto';
import { ClassesResponseDTO } from 'src/common/dto/responses/classes-response.dto';
import { PersonResponseDTO } from 'src/common/dto/responses/person-response.dto';
import { AzureClassesMapper } from 'src/common/mappers/azure-classes.mapper';
import { ClassesMapper } from 'src/common/mappers/classes.mapper';
import { PersonMapper } from 'src/common/mappers/person.mapper';

import { Connection, EntityManager, getManager } from 'typeorm';
import { SyncCurriculumOptions } from './sync-curriculums-without-azureid.command';

@Injectable()
export class SyncCurriculumsWithoutAzureIDRepository {
    entityManager = getManager();

    fromCurriculum = 0;

    toCurriculum = 1000;

    public readConnection: Connection;

    public writeConnection: Connection;

    constructor(
        @InjectConnection('mssql-read') readConnection: Connection,
        @InjectConnection('mssql-write') writeConnection: Connection,
    ) {
        this.readConnection = readConnection;
        this.writeConnection = writeConnection;
    }

    async setSyncCurriculumOptions(options: SyncCurriculumOptions) {
        this.fromCurriculum = options.from;
        this.toCurriculum = options.to;
    }

    async dropCurriculumTempTable() {
        const result = await this.writeConnection.query(
            `
            IF OBJECT_ID('azure_temp.CurriculumTemp', 'U') IS NOT NULL 
            DROP TABLE azure_temp.CurriculumTemp;
            `,
            [],
        );
    }

    async createCurriculumTempTable() {
        const result = await this.writeConnection.query(
            ` 
            CREATE TABLE azure_temp.CurriculumTemp (
                CurriculumID int NULL,
                HasFailed int DEFAULT 0,
                RowID int IDENTITY(0,1) NOT NULL
            );
            `,
            [],
        );
    }

    async fillCurriculumTempTable() {
        const result = await this.writeConnection.query(
            ` 
            INSERT INTO azure_temp.CurriculumTemp (CurriculumID) 
                SELECT 
                    cl.CurriculumID 
                FROM
                    inst_year.CurriculumList cl
                    JOIN inst_basic.CurrentYear cy ON cl.SchoolYear =cy.CurrentYearID
                WHERE 
                    1=1
                    AND cl.isValid = 1
                    AND cy.isValid = 1
                    AND cl.AzureID is null
            `,
            [],
        );
    }

    async getCurriculumsWithoutAzureID(paging: Paging) {
        const result = await this.readConnection.query(
            `
            SELECT
                ct.CurriculumID 
            FROM
                azure_temp.CurriculumTemp ct
            WHERE
                ct.HasFailed = 0 AND
                ct.RowID BETWEEN ${this.fromCurriculum} AND ${this.toCurriculum}
            ORDER BY
                ct.CurriculumID ASC OFFSET ${paging.from} ROWS FETCH NEXT ${paging.numberOfElements} ROWS ONLY
            `,
            [],
        );
        return result;
    }

    async deleteCurriculumsWithoutAzureID(curriculumID: number, entityManager?: EntityManager) {
        const result = await entityManager.query(
            `
            DELETE
            FROM
                azure_temp.CurriculumTemp
            WHERE
                CurriculumID = @0
            `,
            [curriculumID],
        );
        return result;
    }

    async getCurriculumsWithoutAzureIDCount() {
        const result = await this.writeConnection.query(
            ` 
                SELECT 
                    count(*) as count
                FROM
                    azure_temp.CurriculumTemp ct
                WHERE
                    ct.HasFailed = 0 AND
                    ct.RowID BETWEEN ${this.fromCurriculum} AND ${this.toCurriculum}
            `,
        );
        return result;
    }

    async getCurriculumsByCurriculumID(curriculumID: number) {
        const result = await this.readConnection.query(
            `
            SELECT
                curriculumList.CurriculumID AS curriculumID,
                curriculumList.ClassNames AS className,
                curriculumList.SubjectName AS subjectName,
                curriculumList.InstitutionID AS institutionID,
                curriculumList.SchoolYear AS schoolYear,
                curriculumList.SubjectTypeName AS subjectTypeName,
                CASE 
                    WHEN curriculumList.WeeksFirstTerm > 0 THEN currentYear.ValidFrom
                    WHEN curriculumList.WeeksFirstTerm < 0 AND curriculumList.WeeksSecondTerm > 0 THEN DATEADD(week, - curriculumList.WeeksSecondTerm, currentYear.ValidTo)
                END AS startDate,
                CASE 
                    WHEN curriculumList.WeeksSecondTerm > 0 THEN currentYear.ValidTo
                    WHEN curriculumList.WeeksFirstTerm > 0 AND curriculumList.WeeksSecondTerm <= 0 THEN DATEADD(week, curriculumList.WeeksFirstTerm, currentYear.ValidFrom) -- 
                END AS endDate
            FROM inst_year.CurriculumList curriculumList
                INNER JOIN inst_year.Curriculum curriculum ON curriculum.CurriculumID = curriculumList.CurriculumID
                INNER JOIN inst_basic.CurrentYear currentYear ON currentYear.CurrentYearID = curriculumList.SchoolYear
            WHERE curriculumList.CurriculumID = @0
            `,
            [curriculumID],
        );
        const transformedResult: ClassesResponseDTO[] = ClassesMapper.transform(result);
        return transformedResult;
    }

    async insertAzureClass(dto: AzureClassesResponseDTO, manager?: EntityManager) {
        const { title, orgID, termStartDate, termEndDate, classID } = dto;
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
                    ClassID
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3,@4,@5);
            `,
            [WorkflowType.CLASS_CREATE, title, orgID, termStartDate, termEndDate, classID],
        );
        const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(result);
        return transformedResult[0];
    }

    async getStudentPersonIDsByCurriculumID(curriculumID: number, connection?: Connection) {
        const result = await connection.manager.query(
            `
            SELECT
                [PersonID] as personID
            FROM [inst_year].[CurriculumStudent]
            INNER JOIN [inst_year].[CurriculumList] ON [CurriculumList].CurriculumID=[CurriculumStudent].CurriculumID AND [CurriculumList].SchoolYear=[CurriculumStudent].SchoolYear
            JOIN inst_basic.CurrentYear cy ON [CurriculumList].SchoolYear =cy.CurrentYearID
            WHERE
            1=1
            AND [CurriculumList].CurriculumID = @0
            AND [CurriculumList].IsValid =1
            AND cy.isValid = 1
            AND [CurriculumStudent].IsValid=1
            `,
            [curriculumID],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult;
    }

    async getTeacherPersonIDsByCurriculumID(curriculumID: number, connection?: Connection) {
        const result = await connection.manager.query(
            `
            SELECT 
                [StaffPosition].PersonID as personID
            FROM [inst_year].[CurriculumTeacher] 
                INNER JOIN [inst_year].[CurriculumList] ON [CurriculumList].CurriculumID=[CurriculumTeacher].CurriculumID AND [CurriculumList].SchoolYear=[CurriculumTeacher].SchoolYear  
                INNER JOIN [inst_basic].[StaffPosition] ON [StaffPosition].StaffPositionID=[CurriculumTeacher].StaffPositionID 
                JOIN inst_basic.CurrentYear cy ON [CurriculumList].SchoolYear =cy.CurrentYearID
            WHERE
                1=1
                AND [CurriculumList].CurriculumID = @0
                AND [CurriculumList].IsValid =1
                AND [CurriculumTeacher].IsValid=1
                AND cy.isValid = 1 
                AND [StaffPosition].IsValid=1 
            `,
            [curriculumID],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult;
    }

    async getCurrentSchoolYear() {
        const result = await this.entityManager.query(
            `
            
            SELECT 
                TOP 1 CurrentYearID as currentYearID
            FROM inst_basic.CurrentYear cy 
            WHERE
                cy.IsValid = 1
            ORDER BY CurrentYearID DESC
            `,
        );
        return result[0].currentYearID;
    }

    async markCurriculumAsFailed(curriculumID: number) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.CurriculumTemp
            Set
                HasFailed = 1
            WHERE
                CurriculumID = @0
            `,
            [curriculumID],
        );
    }

    async getCreatedAzureClass(dto: AzureClassesResponseDTO) {
        const { classID } = dto;
        const result = await this.entityManager.query(
            `
            SELECT
                RowID as rowID
            FROM
                inst_year.CurriculumList
            WHERE
                1 = 1
                AND AzureID IS NOT NULL
                AND IsValid = 1
                AND CurriculumID = @0
            `,
            [classID],
        );
        const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(result);
        return transformedResult[0];
    }
}
