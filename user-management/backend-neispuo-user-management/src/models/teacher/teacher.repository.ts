import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { TeacherResponseDTO } from 'src/common/dto/responses/teacher-response.dto';
import { TeacherMapper } from 'src/common/mappers/teacher.mapper';
import { EntityRepository, Repository, getManager } from 'typeorm';
import { SysUserEntity } from '../../common/entities/sys-user.entity';

@EntityRepository(SysUserEntity)
export class TeacherRepository extends Repository<SysUserEntity> {
    entityManager = getManager();

    async getTeacherByUserID(userID: number) {
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
                null as phone,
                null as grade,
                null as accountEnabled,
                CASE  
                    WHEN "rol"."SysRoleID" = ${RoleEnum.STUDENT} THEN 'student'
                    ELSE 'teacher'
                END as role,
                1 as hasNeispuoAccess,
                "ins"."InstitutionID" as schoolId
            FROM
                "core"."SysUser" "user"
            LEFT JOIN "core"."Person" "person" ON
                "person"."personID" = "user"."personID"
            LEFT JOIN "core"."EducationalState" "edu" ON
                "edu"."personID" = "person"."personID"
            LEFT JOIN "core"."Institution" "ins" ON
                "ins"."InstitutionID" = "edu"."institutionID"
            LEFT JOIN "core"."Position" "pos" ON
                "pos"."positionID" = "edu"."positionID"
            LEFT JOIN "core"."sysRole" "rol" ON
                "rol"."sysRoleID" = "pos"."sysRoleID"
                    WHERE
            1 = 1
            AND "user"."SysUserID" = @0
            AND "user"."DeletedOn" is NULL
        `,
            [userID],
        );

        const transformedResult: TeacherResponseDTO[] = TeacherMapper.transform(result);
        return transformedResult[0];
    }

    async getTeacherByPersonID(personID: number) {
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
                'teacher' as role,
                1 as hasNeispuoAccess,
                ${RoleEnum.TEACHER} as additionalRole,
                "ins"."InstitutionID" as institutionID
            FROM "core"."Person" "person"
            LEFT JOIN "core"."EducationalState" "edu" ON
                "edu"."personID" = "person"."personID"
            LEFT JOIN "core"."Institution" "ins" ON
                "ins"."InstitutionID" = "edu"."institutionID"
            LEFT JOIN "core"."Position" "pos" ON
                "pos"."positionID" = "edu"."positionID"
            LEFT JOIN "core"."sysRole" "rol" ON
                "rol"."sysRoleID" = "pos"."sysRoleID"
            WHERE
            1 = 1
                AND "person"."PersonID" = @0 
                AND "rol"."SysRoleID" = ${RoleEnum.TEACHER}
        `,
            [personID],
        );

        const transformedResult: TeacherResponseDTO[] = TeacherMapper.transform(result);
        return transformedResult[0];
    }

    async getTeacherPersonIDsByCurriculumID(curriculumID: number) {
        const result = await this.entityManager.query(
            `
			SELECT
                cl.curriculumID, sp.personID
            FROM
                inst_year.CurriculumTeacher ct
            INNER JOIN inst_year.CurriculumList cl ON
                cl.CurriculumID = ct.CurriculumID
                AND cl.SchoolYear = ct.SchoolYear  
            INNER JOIN inst_basic.StaffPosition sp ON
                sp.StaffPositionID = ct.StaffPositionID
            JOIN inst_basic.CurrentYear cy ON
                cl.SchoolYear = cy.CurrentYearID
            WHERE
                1 = 1
                AND cl.CurriculumID = @0
                AND cl.IsValid = 1
                AND ct.IsValid = 1
                AND cy.isValid = 1
                AND sp.IsValid = 1
            GROUP BY
                cl.CurriculumID, sp.PersonID 
            `,
            [curriculumID],
        );
        const transformedResult: TeacherResponseDTO[] = TeacherMapper.transform(result);
        return transformedResult;
    }
}
