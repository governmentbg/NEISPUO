import {MigrationInterface, QueryRunner} from "typeorm";

export class DailyJobProcedureRemake1704288822341 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        
        await queryRunner.query(
            `
                CREATE PROCEDURE  azure_temp.DAILY_PROCEDURE_TEACHERS_CREATE AS
                    BEGIN
                        BEGIN TRY
                            SELECT
                                DISTINCT p.PersonID as personID
                            FROM
                                core.Person p
                            JOIN core.EducationalState es on
                                p.PersonID = es.PersonID
                            WHERE 
                                1=1
                                AND es.PositionID = 2  
                                AND es.PositionID NOT IN (9)  
                                AND p.AzureID IS NULL
                                AND NOT EXISTS (
                                    SELECT
                                        1
                                    FROM
                                        core.SysUser su
                                    WHERE
                                        1 = 1
                                        AND su.PersonID = p.PersonID
                                        AND su.DeletedOn IS NULL -- this checks for active accounts
                                        AND su.Username LIKE '%@edu.mon.bg%' -- this removes parent accounts
                                        AND LEN(su.Username) - len(REPLACE(su.Username,'.','')) > 2 -- this checks if the account is a teacher
                                )
                                AND NOT EXISTS (
                                    SELECT
                                        1
                                    FROM
                                        azure_temp.UsersArchived e
                                    WHERE
                                        e.personID = p.PersonID
                                        AND e.WorkflowType = 7
                                        AND e.UserRole = 'TEACHER'
                                )
                                AND NOT EXISTS (
                                    SELECT
                                        1
                                    FROM
                                        azure_temp.Users e
                                    WHERE
                                        e.personID = p.PersonID
                                        AND e.WorkflowType = 7
                                        AND e.UserRole = 'TEACHER'
                                )
                                AND EXISTS (
                                    SELECT
                                        1
                                    FROM
                                        inst_basic.StaffPosition sp
                                    WHERE
                                        sp.personID = p.PersonID
                                        AND sp.IsValid = 1
                                        AND sp.CurrentlyValid = 1
                                        AND sp.StaffTypeID = 1
                                );
                        END TRY
                        BEGIN CATCH
                        END CATCH
                    END;
            `,
        );

        await queryRunner.query(
            `
            	
            CREATE PROCEDURE  azure_temp.DAILY_PROCEDURE_STUDENTS_CREATE AS
                BEGIN
                    BEGIN TRY
                        SELECT
                            DISTINCT p.PersonID as personID
                        FROM
                            core.Person p
                        JOIN core.EducationalState es on
                            p.PersonID = es.PersonID
                        WHERE
                            1=1
                            AND es.PositionID NOT IN (2,9)
                            AND NOT EXISTS (
                                SELECT
                                    1 
                                FROM
                                    core.SysUser su
                                WHERE
                                    1 = 1
                                    AND su.PersonID = p.PersonID
                                    AND su.DeletedOn IS NULL -- this checks for active accounts
                                    AND su.Username LIKE '%@edu.mon.bg%' -- this removes parent accounts
                                    AND LEN(su.Username) - len(REPLACE(su.Username,'.','')) = 2 -- this checks if the account is a student
                            )
                            AND p.AzureID IS NULL
                            AND NOT EXISTS (
                                SELECT
                                    1
                                FROM
                                    azure_temp.UsersArchived e
                                WHERE
                                    e.personID = p.PersonID
                                    AND e.WorkflowType = 7
                                    AND e.UserRole = 'STUDENT'
                            )
                            AND NOT EXISTS (
                                SELECT
                                    1
                                FROM
                                    azure_temp.Users e
                                WHERE
                                    e.personID = p.PersonID
                                    AND e.WorkflowType = 7
                                    AND e.UserRole = 'STUDENT'
                            )	
                            AND EXISTS (
                                SELECT
                                    1
                                FROM
                                    student.StudentClass sc
                                    inner join inst_nom.ClassType ct on sc.ClassTypeId = ct.ClassTypeID
                                    join inst_basic.CurrentYear cy on sc.SchoolYear = cy.CurrentYearID 
                                WHERE
                                    sc.personID = p.PersonID
                                    AND sc.IsCurrent = 1
                                    AND ct.ClassKind = 1
                                    AND cy.IsValid = 1
                                    AND ct.ClassKind = 1
                            );
                    END TRY
                    BEGIN CATCH
                    END CATCH
                END;
            `,
            undefined,
        );
        
        await queryRunner.query(
            `
                CREATE PROCEDURE  azure_temp.DAILY_PROCEDURE_CLASS_CREATE AS
                    BEGIN
                        BEGIN TRY
                            SELECT 
                                cl.curriculumID
                            FROM 
                                inst_year.CurriculumList cl 
                            JOIN inst_basic.CurrentYear cy ON
                                cl.SchoolYear = cy.CurrentYearID
                            WHERE	
                                cl.IsValid = 1 AND
                                cy.IsValid = 1 AND
                                cl.AzureID IS NULL
                            GROUP BY cl.curriculumID;
                        END TRY
                        BEGIN CATCH
                        END CATCH
                    END;
            `,
            undefined,
        );
        await queryRunner.query(
            `
            CREATE PROCEDURE  azure_temp.DAILY_PROCEDURE_ORGANIZATIONS_CREATE AS
                BEGIN
                    BEGIN TRY
                        SELECT
                            "ins"."name" AS "institutionName",
                            "ins"."abbreviation" AS "description",
                            null AS "highestGrade",
                            null AS "lowestGrade",
                            "ins"."InstitutionID" AS "institutionID",
                            null AS "street",
                            null AS "postalCode",
                            su.sysUserID as sysUserID,
                            p.AzureID as azureID
            
                        FROM
                            "core"."Institution" "ins"
                            LEFT JOIN core.SysUserSysRole susr on "ins".InstitutionID = susr.InstitutionID
                            LEFT JOIN core.SysUser su on su.SysUserID = susr.SysUserID
                            LEFT JOIN core.Person p ON
                                p.personID = su.personID
                        WHERE
                            1 = 1
                            AND su.DeletedOn IS NULL 
                            AND p.AzureID IS NULL
                            AND su.Username = CONCAT(ins.institutionID, '@edu.mon.bg') 
                    END TRY
                    BEGIN CATCH
                    END CATCH
                END;
            `,
            undefined,
        );
        await queryRunner.query(
            `
            CREATE PROCEDURE  azure_temp.DAILY_PROCEDURE_TEACHER_CLASS_ENROLLMENT_CREATE AS
                BEGIN
                    BEGIN TRY
                        SELECT
                            sp.personID,
                            ct.curriculumID
                        FROM
                            inst_year.CurriculumTeacher ct
                            join inst_year.Curriculum c on ct.CurriculumID = c.CurriculumID 
                            join inst_basic.StaffPosition sp on sp.StaffPositionID  = ct.StaffPositionID
                            join inst_basic.CurrentYear cy on c.SchoolYear = cy.CurrentYearID 
                        WHERE
                            1=1
                            AND c.IsValid =1 
                            AND ct.IsValid =1 
                            AND sp.IsValid = 1 
                            AND sp.CurrentlyValid = 1
                            AND ct.isAzureEnrolled = 0
                            AND cy.IsValid = 1
                            AND NOT EXISTS (
                                Select
                                    1
                                From
                                    azure_temp.EnrollmentsArchived e
                                Where
                                    e.userPersonID = sp.PersonID
                                    AND ct.CurriculumID = e.curriculumID
                                    AND e.WorkflowType = 11
                            )
                            AND NOT EXISTS (
                                Select
                                    1
                                From
                                    azure_temp.Enrollments e
                                Where
                                    e.userPersonID = sp.PersonID
                                    AND ct.CurriculumID = e.curriculumID
                                    AND e.WorkflowType = 11
                            )
                    END TRY
                    BEGIN CATCH
                    END CATCH
                END;
            `,
            undefined,
        );
        await queryRunner.query(
            `	
                CREATE PROCEDURE  azure_temp.DAILY_PROCEDURE_STUDENT_CLASS_ENROLLMENT_CREATE AS
                    BEGIN
                        BEGIN TRY
                            SELECT
                                ct.personID,
                                ct.curriculumID
                            FROM
                                inst_year.CurriculumStudent ct
                                join inst_year.Curriculum c on ct.CurriculumID = c.CurriculumID 
                                join inst_basic.CurrentYear cy on ct.SchoolYear = cy.CurrentYearID 
                            WHERE 
                                ct.isAzureEnrolled = 0
                                and ct.IsValid = 1
                                and c.IsValid = 1
                                and cy.IsValid = 1
                                AND NOT EXISTS (
                                    Select
                                        1
                                    From
                                        azure_temp.EnrollmentsArchived e
                                    Where
                                        e.userPersonID = ct.PersonID
                                        AND ct.CurriculumID = e.curriculumID
                                        AND e.WorkflowType = 11
                                )
                                AND NOT EXISTS (
                                    Select
                                        1
                                    From
                                        azure_temp.Enrollments e
                                    Where
                                        e.userPersonID = ct.PersonID
                                        AND ct.CurriculumID = e.curriculumID
                                        AND e.WorkflowType = 11
                                );
                        END TRY
                        BEGIN CATCH
                        END CATCH
                    END;
            `,
            undefined,
        );
        await queryRunner.query(
            `
                CREATE PROCEDURE  azure_temp.DAILY_PROCEDURE_STUDENT_SCHOOL_ENROLLMENT_CREATE AS
                    BEGIN
                        BEGIN TRY
                            SELECT
                            es.PersonID as personID ,
                            es.InstitutionID as institutionID
                        from
                            core.EducationalState es
                        join core.SysUser su on
                            su.Username = CONCAT(es.InstitutionID, '@edu.mon.bg')
                        join core.Person pp on
                            su.PersonID = pp.PersonID
                        join core.Person p on
                            es.PersonID = p.PersonID
                        WHERE
                            p.PublicEduNumber IS NOT NULL AND
                            p.AzureID IS NOT NULL AND
                            pp.AzureID IS NOT NULL AND
                            NOT EXISTS (
                            Select
                                1
                            From
                                azure_temp.EnrollmentsArchived e
                            Where
                                e.userPersonID = es.PersonID
                                AND pp.PersonID = e.organizationPersonID
                                AND e.WorkflowType = 12
                                        )
                            AND NOT EXISTS (
                            Select
                                1
                            From
                                azure_temp.Enrollments e
                            Where
                                e.userPersonID = es.PersonID
                                AND pp.PersonID = e.organizationPersonID
                                AND e.WorkflowType = 12
                                        )
                            And PositionID <> 2
                            AND PositionID NOT IN (9)
                        Group By
                            es.PersonID,
                            es.InstitutionID
                        END TRY
                        BEGIN CATCH
                        END CATCH
                    END;
            `,
            undefined,
        );
        await queryRunner.query(
            `
                CREATE PROCEDURE  azure_temp.DAILY_PROCEDURE_TEACHER_SCHOOL_ENROLLMENT_CREATE AS
                    BEGIN
                        BEGIN TRY
                            SELECT
                            es.PersonID as personID ,
                            es.InstitutionID as institutionID
                        from
                            core.EducationalState es
                        join core.SysUser su on
                            su.Username = CONCAT(es.InstitutionID, '@edu.mon.bg')
                        join core.Person pp on
                            su.PersonID = pp.PersonID
                        join core.Person p on
                            es.PersonID = p.PersonID
                        WHERE
                            p.PublicEduNumber IS NOT NULL AND
                            p.AzureID IS NOT NULL AND
                            pp.AzureID IS NOT NULL AND
                            NOT EXISTS (
                            Select
                                1
                            From
                                azure_temp.EnrollmentsArchived e
                            Where
                                e.userPersonID = es.PersonID
                                AND pp.PersonID = e.organizationPersonID
                                AND e.WorkflowType = 12
                                        )
                            AND NOT EXISTS (
                            Select
                                1
                            From
                                azure_temp.Enrollments e
                            Where
                                e.userPersonID = es.PersonID
                                AND pp.PersonID = e.organizationPersonID
                                AND e.WorkflowType = 12
                                        )
                            And PositionID = 2
                            AND PositionID NOT IN (9)
                        Group By
                            es.PersonID,
                            es.InstitutionID
                        END TRY
                        BEGIN CATCH
                        END CATCH
                    END;
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
                DROP PROCEDURE azure_temp.DAILY_PROCEDURE_TEACHERS_CREATE
            `,
            undefined,
        );
        await queryRunner.query(
            `
                DROP PROCEDURE azure_temp.DAILY_PROCEDURE_STUDENTS_CREATE
            `,
            undefined,
        );
        await queryRunner.query(
            `
                DROP PROCEDURE azure_temp.DAILY_PROCEDURE_CLASS_CREATE
            `,
            undefined,
        );
        await queryRunner.query(
            `
                DROP PROCEDURE azure_temp.DAILY_PROCEDURE_ORGANIZATIONS_CREATE
            `,
            undefined,
        );
        await queryRunner.query(
            `
                DROP PROCEDURE azure_temp.DAILY_PROCEDURE_TEACHER_CLASS_ENROLLMENT_CREATE
            `,
            undefined,
        );
        await queryRunner.query(
            `
                DROP PROCEDURE azure_temp.DAILY_PROCEDURE_STUDENT_CLASS_ENROLLMENT_CREATE
            `,
            undefined,
        );
        await queryRunner.query(
            `
                DROP PROCEDURE azure_temp.DAILY_PROCEDURE_STUDENT_SCHOOL_ENROLLMENT_CREATE
            `,
            undefined,
        );
        await queryRunner.query(
            `
                DROP PROCEDURE azure_temp.DAILY_PROCEDURE_TEACHER_SCHOOL_ENROLLMENT_CREATE
            `,
            undefined,
        );
    }

}
