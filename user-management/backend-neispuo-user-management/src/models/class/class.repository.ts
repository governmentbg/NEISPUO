import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { InProcessing } from 'src/common/constants/enum/in-processing.enum';
import { IsForArchivation } from 'src/common/constants/enum/is-for-archivation.enum';
import { AzureClassesResponseDTO } from 'src/common/dto/responses/azure-classes-response.dto';
import { AzureEnrollmentsResponseDTO } from 'src/common/dto/responses/azure-enrollments-response.dto';
import { ClassesResponseDTO } from 'src/common/dto/responses/classes-response.dto';
import { CurriculumStudentResponseDTO } from 'src/common/dto/responses/curriculum-student-response.dto';
import { CurriculumTeacherResponseDTO } from 'src/common/dto/responses/curriculum-teacher-response.dto';
import { SysUserEntity } from 'src/common/entities/sys-user.entity';
import { AzureClassesMapper } from 'src/common/mappers/azure-classes.mapper';
import { AzureEnrollmentsMapper } from 'src/common/mappers/azure-enrollments.mapper';
import { ClassesMapper } from 'src/common/mappers/classes.mapper';
import { CurriculumStudentMapper } from 'src/common/mappers/curriculum-student.mapper';
import { CurriculumTeacherMapper } from 'src/common/mappers/curriculum-teacher.mapper';
import { Connection, EntityRepository, getManager } from 'typeorm';

@EntityRepository(SysUserEntity)
export class ClassRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async getCurriculumsByCurriculumID(curriculumID: number) {
        const result = await this.entityManager.query(
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
                END AS endDate,
                    curriculum.AzureID as azureID,
                    person.AzureID AS institutionAzureID
            FROM inst_year.CurriculumList curriculumList
                INNER JOIN inst_year.Curriculum curriculum ON curriculum.CurriculumID = curriculumList.CurriculumID
                INNER JOIN inst_basic.CurrentYear currentYear ON currentYear.CurrentYearID = curriculumList.SchoolYear
                INNER JOIN core.SysUser sysUser ON Username = CONCAT(curriculumList.InstitutionID, '@edu.mon.bg')
                INNER JOIN core.Person person ON  sysUser.PersonID = person.PersonID
            WHERE curriculumList.CurriculumID = @0
            `,
            [curriculumID],
        );
        const transformedResult: ClassesResponseDTO[] = ClassesMapper.transform(result);
        return transformedResult;
    }

    async getCurriculumsStudentByPersonID(personID: number) {
        const year = await this.getCurrentSchoolYear();
        const result = await this.entityManager.query(
            `
            SELECT
                cs.CurriculumID AS curriculumID
            FROM inst_year.CurriculumStudent cs
            JOIN inst_year.Curriculum c on cs.CurriculumID = c.CurriculumID 
            WHERE
            1=1
                AND c.IsValid = 1
                AND cs.IsValid = 1
                AND cs.PersonID = @0
                AND cs.SchoolYear = @1 
            `,
            [personID, await this.getCurrentSchoolYear()],
        );
        const transformedResult: ClassesResponseDTO[] = ClassesMapper.transform(result);
        return transformedResult;
    }

    async getCurriculumsTeacherByPersonID(personID: number) {
        const result = await this.entityManager.query(
            `
            SELECT
                ct.CurriculumID AS curriculumID
            FROM inst_year.CurriculumTeacher ct
            JOIN inst_year.Curriculum c on ct.CurriculumID = c.CurriculumID 
            JOIN inst_basic.StaffPosition sp on ct.StaffPositionID = sp.StaffPositionID 
            WHERE
                1=1
                AND c.IsValid = 1
                AND ct.IsValid = 1
                AND sp.PersonID = @0
                AND ct.SchoolYear = @1 
            `,
            [personID, await this.getCurrentSchoolYear()],
        );
        const transformedResult: ClassesResponseDTO[] = ClassesMapper.transform(result);
        return transformedResult;
    }

    async getCurrentSchoolYear() {
        const result = await this.entityManager.query(
            `
            
            SELECT 
                TOP 1 CurrentYearID as currentYearID
            FROM inst_basic.CurrentYear cy
            WHERE cy.IsValid =1 
            ORDER BY CurrentYearID DESC
            `,
        );
        return result[0].currentYearID;
    }

    async createAzureClassProcedure(dto: AzureClassesResponseDTO) {
        let result = false;
        const { rowID, azureID, classID } = dto;
        try {
            await this.connection.transaction(async (manager) => {
                const curriculumResult = await manager.query(
                    `
                    UPDATE
                        inst_year.Curriculum
                    SET
                        AzureID = @0
                    OUTPUT
                        INSERTED.CurriculumID as curriculumID
                    WHERE
                        CurriculumID = @1
                        `,
                    [azureID, classID],
                );
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Classes 
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = @0,
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = @1
                    OUTPUT INSERTED.RowID as rowID
                    WHERE
                        RowID = @2;
                        `,
                    [EventStatus.SYNCHRONIZED, InProcessing.NO, rowID],
                );
                const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;
                if (jobRowID) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async updateAzureClassProcedure(dto: AzureClassesResponseDTO) {
        let result = false;
        const { rowID } = dto;
        try {
            await this.connection.transaction(async (manager) => {
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Classes 
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = @0,
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = @1
                    OUTPUT INSERTED.RowID as rowID
                    WHERE
                        RowID = @2;
                        `,
                    [EventStatus.SYNCHRONIZED, InProcessing.NO, rowID],
                );
                const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;
                if (jobRowID) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async deleteAzureClassProcedure(dto: AzureClassesResponseDTO) {
        let result = false;
        const { rowID } = dto;
        try {
            await this.connection.transaction(async (manager) => {
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Classes 
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = @0,
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = @1
                    OUTPUT INSERTED.RowID as rowID
                    WHERE
                        RowID = @2;
                        `,
                    [EventStatus.SYNCHRONIZED, InProcessing.NO, rowID],
                );
                const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;
                if (jobRowID) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async getCurriculumByCurriculumID(curriculumID: number) {
        const result = await this.connection.query(
            `
                SELECT 
                    cl.CurriculumID as curriculumID,
                    cl.azureID,
                    cl.InstitutionID as institutionID
                FROM
                    inst_year.CurriculumList cl
                    JOIN inst_basic.CurrentYear cy ON cl.SchoolYear =cy.CurrentYearID
                WHERE 
                    1=1
                    AND cl.isValid = 1
                    AND cy.isValid = 1
                    AND cl.CurriculumID = @0
            `,
            [curriculumID],
        );
        const transformedResult: ClassesResponseDTO[] = ClassesMapper.transform(result);
        return transformedResult;
    }

    async getUnsyncedClasses() {
        const result = await this.entityManager.query(
            `
                EXEC [azure_temp].[DAILY_PROCEDURE_CLASS_CREATE];
            `,
            [],
        );
        const transformedResult: ClassesResponseDTO[] = ClassesMapper.transform(result);
        return transformedResult;
    }

    async setIsAzureEnrolledForStudent(dtos: AzureEnrollmentsResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                inst_year.CurriculumStudent
            SET
                isAzureEnrolled = 1
            OUTPUT 
                INSERTED.CurriculumID as curriculumID,
                INSERTED.PersonID as personID
            FROM (
                VALUES ${dtos.map((dto) => `(${dto.curriculumID}, ${dto.userPersonID})`).join(',')}
            ) AS dto(CurriculumID, UserPersonID)
            WHERE
                1=1
                AND CurriculumStudent.CurriculumID = dto.CurriculumID
                AND CurriculumStudent.PersonID = dto.UserPersonID
                AND CurriculumStudent.isValid = 1
            `,
        );
        return AzureEnrollmentsMapper.transform(result);
    }

    async setIsAzureEnrolledForTeacher(dtos: AzureEnrollmentsResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                ct
            SET
                isAzureEnrolled = 1
            OUTPUT 
                INSERTED.CurriculumTeacherID as curriculumTeacherID
            FROM (
                VALUES ${dtos.map((dto) => `(${dto.curriculumID}, ${dto.userPersonID})`).join(',')}
            ) AS dto(CurriculumID, UserPersonID) 
            JOIN inst_year.CurriculumTeacher ct on
                dto.CurriculumID = ct.CurriculumID
            JOIN inst_basic.StaffPosition sp ON
                sp.StaffPositionID = ct.StaffPositionID
            WHERE
                1=1
                AND ct.CurriculumID = dto.CurriculumID
                AND sp.PersonID = dto.UserPersonID
                AND ct.isValid = 1
            `,
        );
        return AzureEnrollmentsMapper.transform(result);
    }

    async getStudentsNotEnrolledForCurriculums() {
        const currentSchoolYear = await this.getCurrentSchoolYear();
        const result = await this.entityManager.query(
            `
                EXEC [azure_temp].[DAILY_PROCEDURE_STUDENT_CLASS_ENROLLMENT_CREATE]
            `,
            [currentSchoolYear],
        );
        const transformedResult: CurriculumStudentResponseDTO[] = CurriculumStudentMapper.transform(result);
        return transformedResult;
    }

    async getTeachersNotEnrolledForCurriculums() {
        const currentSchoolYear = await this.getCurrentSchoolYear();
        const result = await this.entityManager.query(
            `
                EXEC [azure_temp].[DAILY_PROCEDURE_TEACHER_CLASS_ENROLLMENT_CREATE];
            `,
        );
        const transformedResult: CurriculumTeacherResponseDTO[] = CurriculumTeacherMapper.transform(result);
        return transformedResult;
    }
}
