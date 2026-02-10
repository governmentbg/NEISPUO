import { Injectable } from '@nestjs/common';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { StudentResponseDTO } from 'src/common/dto/responses/student-response.dto';
import { StudentMapper } from 'src/common/mappers/student.mapper';
import { Connection, EntityManager, getManager } from 'typeorm';
@Injectable()
export class StudentRepository {
    // @TODO rewrite get all methods to use ENTITies
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async getStudentByPersonID(personID: number) {
        // @TODO find out where to get Phone grade and accountenabled from the database
        const result = await this.entityManager.query(
            `
            SELECT
                "person"."PersonID" as personID,
                "person"."PersonalID" as personalID,
                "person"."firstName" as firstName,
                "person"."middleName" as middleName,
                "person"."lastName" as lastName,
                "person"."BirthDate" as birthDate,
                "person"."AzureID" as azureID,
                "person"."PublicEduNumber" as publicEduNumber,
                null as phone,
                null as grade,
                null as accountEnabled,
                ${RoleEnum.STUDENT} as additionalRole,
                1 as hasNeispuoAccess,
               'student' as role
            FROM "core"."Person" "person"
            WHERE
                1 = 1
                AND "person"."PersonID" = @0
        `,
            [personID],
        );

        const transformedResult: StudentResponseDTO[] = StudentMapper.transform(result);
        return transformedResult[0];
    }

    async isStudentGraduate(personID: number): Promise<boolean> {
        const result = await this.entityManager.query(
            `
                SELECT COUNT(DISTINCT p.PersonID) as count FROM core.Person p
                INNER JOIN document.Diploma d ON p.PersonID = d.PersonId AND p.PersonalIDType = d.PersonalIDType
                INNER JOIN document.BasicDocument bd ON bd.Id = d.BasicDocumentId
                WHERE d.IsPublic = 1
                AND d.BasicDocumentId IN (1,2,3,35,36,50,51,60,61,253)
                AND p.PersonID = @0
            `,
            [personID],
        );

        return result.length >= 1 && result[0].count > 0;
    }

    async getGraduatedStudents(limit: number) {
        const result = await this.entityManager.query(
            `
            SELECT TOP ${limit} 
                person.personID,
                person.personalID,
                person.firstName,
                person.middleName,
                person.lastName,
                person.birthDate,
                person.azureID,
                null as phone,
                null as grade,
                null as accountEnabled,
                1 as hasNeispuoAccess,
                'student' as role
                FROM
				core.Person person
			INNER JOIN document.Diploma diploma ON
				person.PersonID = diploma.PersonId
				AND person.PersonalIDType = diploma.PersonalIDType
			INNER JOIN document.BasicDocument basicDocument ON
				basicDocument.Id = diploma.BasicDocumentId
				-- We can INNER JOIN as the table contains historical data so everyone will have at least one record
			INNER JOIN sob.EducationalState eduStateHistory ON
				eduStateHistory.PersonID = person.PersonID
			WHERE
				diploma.IsPublic = 1
				AND diploma.BasicDocumentId IN (1, 2, 3, 35, 36, 50, 51, 60, 61, 253)
				AND person.AzureID IS NOT NULL
				and person.PublicEduNumber IS NOT NULL
				AND person.PublicEduNumber NOT LIKE '%.%'
				--- Make sure the user is not currently a teacher
				AND 
			                    ( 
			                        person.PersonID NOT IN (
                                        SELECT
                                            PersonID
                                        FROM
                                            core.EducationalState
                                        WHERE
                                            PositionID = 2
                                            AND PersonID = person.PersonID)
			                    )
                AND
                    person.PersonID NOT IN (SELECT u.PersonID
                    FROM azure_temp.Users u
                    WHERE u.WorkflowType = ${WorkflowType.USER_DELETE} AND u.PersonID  = person.PersonID AND u.UserRole = '${UserRoleType.STUDENT}')
			GROUP BY
				person.personID,
				person.personalID,
				person.firstName,
				person.middleName,
				person.lastName,
				person.birthDate,
				person.azureID
            `,
            [WorkflowType.USER_DELETE],
        );
        const transformedResult: StudentResponseDTO[] = StudentMapper.transform(result);
        return transformedResult;
    }

    async getStudentPersonIDsByCurriculumID(curriculumID: number) {
        const result = await this.entityManager.query(
            `
            SELECT
                personID,
                institutionID
            FROM
                inst_year.CurriculumStudent cs
            INNER JOIN inst_year.CurriculumList cl ON
                cl.CurriculumID = cs.CurriculumID
                AND cl.SchoolYear = cs.SchoolYear
            JOIN inst_basic.CurrentYear cy ON
                cl.SchoolYear = cy.CurrentYearID
            WHERE
                1 = 1
                AND cl.CurriculumID = @0
                AND cl.IsValid = 1
                AND cy.isValid = 1
                AND cs.IsValid = 1
            `,
            [curriculumID],
        );
        const transformedResult: StudentResponseDTO[] = StudentMapper.transform(result);
        return transformedResult;
    }

    async getStudentGradeByPersonID(personID: number, entityManager?: EntityManager) {
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `
            SELECT 
              TOP 1 
                sc.personID ,
                sc.BasicClassID as grade
            FROM
                inst_year.CurriculumStudent cs
                inner join core.Person p on cs.PersonID = p.PersonID
                inner join student.StudentClass sc on cs.StudentID = sc.ID
                inner join inst_nom.ClassType ct on sc.ClassTypeId = ct.ClassTypeID
                inner join core.Institution i on sc.InstitutionId = i.InstitutionID
                inner join noms.DetailedSchoolType dst on i.DetailedSchoolTypeID = dst.DetailedSchoolTypeID
                inner join inst_basic.CurrentYear cy on sc.SchoolYear = cy.CurrentYearID 
            WHERE
                1 = 1
                and cs.IsValid = 1
                and ct.ClassKind = 1
                and sc.IsCurrent = 1
                and ((dst.InstType in (1,2) and sc.PositionId = 3) or (dst.InstType = 4 and sc.PositionId = 7))
                and p."PersonID" = @0
            ORDER BY
                sc.EnrollmentDate
            `,
            [personID],
        );
        const transformedResult: StudentResponseDTO[] = StudentMapper.transform(result);
        return transformedResult[0];
    }
}
