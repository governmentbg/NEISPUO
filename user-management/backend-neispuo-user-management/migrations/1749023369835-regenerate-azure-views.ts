import { MigrationInterface, QueryRunner } from 'typeorm';

export class RegenerateAzureViews1749023369835 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
                DROP VIEW azure_temp.AzureOrganizationsView
            `,
        );
        await queryRunner.query(
            `        
                DROP VIEW azure_temp.AzureUsersView
            `,
        );
        await queryRunner.query(
            `        
                DROP VIEW azure_temp.AzureClassesView
            `,
        );
        await queryRunner.query(
            `        
                DROP VIEW azure_temp.AzureEnrollmentsView
            `,
        );

        await queryRunner.query(`
            CREATE VIEW azure_temp.AzureOrganizationsView AS
            SELECT
                rowID,
                organizationID,
                workflowType,
                name,
                description,
                principalId,
                principalName,
                principalEmail,
                highestGrade,
                lowestGrade,
                phone,
                city,
                area,
                country,
                postalCode,
                street,
                inProcessing,
                errorMessage,
                createdOn,
                updatedOn,
                guid,
                retryAttempts,
                status,
                username,
                password,
                personID,
                isForArchivation,
                azureID,
                inProgressResultCount
            FROM
                azure_temp.Organizations o
            UNION
            SELECT
                rowID,
                organizationID,
                workflowType,
                name,
                description,
                principalId,
                principalName,
                principalEmail,
                highestGrade,
                lowestGrade,
                phone,
                city,
                area,
                country,
                postalCode,
                street,
                inProcessing,
                errorMessage,
                createdOn,
                updatedOn,
                guid,
                retryAttempts,
                status,
                username,
                password,
                personID,
                isForArchivation,
                azureID,
                inProgressResultCount
            FROM
                azure_temp.OrganizationsArchived oa
        `);

        await queryRunner.query(`
            CREATE VIEW azure_temp.AzureUsersView AS
            SELECT
                rowID,
                userID,
                workflowType,
                identifier,
                firstName,
                middleName,
                surname,
                password,
                email,
                phone,
                grade,
                schoolId,
                birthDate,
                userRole,
                accountEnabled,
                inProcessing,
                errorMessage,
                createdOn,
                updatedOn,
                guid,
                retryAttempts,
                username,
                status,
                personID,
                deletionType,
                additionalRole,
                hasNeispuoAccess,
                assignedAccountantSchools,
                azureID,
                inProgressResultCount,
                isForArchivation,
                sisAccessSecondaryRole,
                createdBy
            FROM
                azure_temp.Users u
            UNION
            SELECT
                rowID,
                userID,
                workflowType,
                identifier,
                firstName,
                middleName,
                surname,
                password,
                email,
                phone,
                grade,
                schoolId,
                birthDate,
                userRole,
                accountEnabled,
                inProcessing,
                errorMessage,
                createdOn,
                updatedOn,
                guid,
                retryAttempts,
                username,
                status,
                personID,
                deletionType,
                additionalRole,
                hasNeispuoAccess,
                assignedAccountantSchools,
                azureID,
                inProgressResultCount,
                isForArchivation,
                sisAccessSecondaryRole,
                createdBy
            FROM
                azure_temp.UsersArchived ua
        `);

        await queryRunner.query(`
            CREATE VIEW azure_temp.AzureClassesView AS
            SELECT
                rowID,
                classID,
                workflowType,
                title,
                classCode,
                orgID,
                termID,
                termName,
                termStartDate,
                termEndDate,
                inProcessing,
                errorMessage,
                createdOn,
                updatedOn,
                guid,
                retryAttempts,
                status,
                azureID,
                inProgressResultCount,
                isForArchivation
            FROM
                azure_temp.Classes c
            UNION
            SELECT
                rowID,
                classID,
                workflowType,
                title,
                classCode,
                orgID,
                termID,
                termName,
                termStartDate,
                termEndDate,
                inProcessing,
                errorMessage,
                createdOn,
                updatedOn,
                guid,
                retryAttempts,
                status,
                azureID,
                inProgressResultCount,
                isForArchivation
            FROM
                azure_temp.ClassesArchived ca
        `);

        await queryRunner.query(`
            CREATE VIEW azure_temp.AzureEnrollmentsView AS
            SELECT
                rowID,
                workflowType,
                userAzureID,
                classAzureID,
                organizationAzureID,
                inProcessing,
                errorMessage,
                createdOn,
                updatedOn,
                guid,
                retryAttempts,
                status,
                inProgressResultCount,
                userPersonID,
                organizationPersonID,
                curriculumID,
                userRole,
                isForArchivation
            FROM
                azure_temp.Enrollments e
            UNION
            SELECT
                rowID,
                workflowType,
                userAzureID,
                classAzureID,
                organizationAzureID,
                inProcessing,
                errorMessage,
                createdOn,
                updatedOn,
                guid,
                retryAttempts,
                status,
                inProgressResultCount,
                userPersonID,
                organizationPersonID,
                curriculumID,
                userRole,
                isForArchivation
            FROM
                azure_temp.EnrollmentsArchived ea
        `);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
                DROP VIEW azure_temp.AzureOrganizationsView
            `,
        );
        await queryRunner.query(
            `        
                DROP VIEW azure_temp.AzureUsersView
            `,
        );
        await queryRunner.query(
            `        
                DROP VIEW azure_temp.AzureClassesView
            `,
        );
        await queryRunner.query(
            `        
                DROP VIEW azure_temp.AzureEnrollmentsView
            `,
        );

        await queryRunner.query(
            `
            CREATE VIEW azure_temp.AzureOrganizationsView
                AS
                    SELECT
                        rowID,
                        organizationID,
                        workflowType,
                        name,
                        description,
                        principalId,
                        principalName,
                        principalEmail,
                        highestGrade,
                        lowestGrade,
                        phone,
                        city,
                        area,
                        country,
                        postalCode,
                        street,
                        inProcessing,
                        errorMessage,
                        createdOn,
                        updatedOn,
                        guid,
                        retryAttempts,
                        status,
                        username
                    FROM
                    azure_temp.Organizations o
                UNION
                    SELECT
                        rowID,
                        organizationID,
                        workflowType,
                        name,
                        description,
                        principalId,
                        principalName,
                        principalEmail,
                        highestGrade,
                        lowestGrade,
                        phone,
                        city,
                        area,
                        country,
                        postalCode,
                        street,
                        inProcessing,
                        errorMessage,
                        createdOn,
                        updatedOn,
                        guid,
                        retryAttempts,
                        status,
                        username
                    FROM
                        azure_temp.OrganizationsArchived oa
            `,
        );
        await queryRunner.query(
            `
                CREATE VIEW azure_temp.AzureUsersView
                            AS
                      SELECT
                    rowID,
                    userID,
                    workflowType,
                    identifier,
                    firstName,
                    middleName,
                    surname,
                    password,
                    email,
                    phone,
                    grade,
                    schoolId,
                    birthDate,
                    userRole,
                    accountEnabled,
                    inProcessing,
                    errorMessage,
                    createdOn,
                    updatedOn,
                    guid,
                    retryAttempts,
                    username,
                    status,
                    personID
                FROM
                    azure_temp.Users u
                UNION
                                     SELECT
                         rowID,
                    userID,
                    workflowType,
                    identifier,
                    firstName,
                    middleName,
                    surname,
                    password,
                    email,
                    phone,
                    grade,
                    schoolId,
                    birthDate,
                    userRole,
                    accountEnabled,
                    inProcessing,
                    errorMessage,
                    createdOn,
                    updatedOn,
                    guid,
                    retryAttempts,
                    username,
                    status,
                    personID
                FROM
                    azure_temp.UsersArchived ua
            `,
        );

        await queryRunner.query(
            `
                CREATE VIEW azure_temp.AzureClassesView
                            AS
                    SELECT
                    rowID,
                    classID,
                    workflowType,
                    title,
                    classCode,
                    orgID,
                    termID,
                    termName,
                    termStartDate,
                    termEndDate,
                    inProcessing,
                    errorMessage,
                    createdOn,
                    updatedOn,
                    guid,
                    retryAttempts,
                    status
                FROM
                    azure_temp.Classes c
                UNION
                                    SELECT
                    rowID,
                    classID,
                    workflowType,
                    title,
                    classCode,
                    orgID,
                    termID,
                    termName,
                    termStartDate,
                    termEndDate,
                    inProcessing,
                    errorMessage,
                    createdOn,
                    updatedOn,
                    guid,
                    retryAttempts,
                    status
                FROM
                    azure_temp.ClassesArchived ca
                
            `,
        );

        await queryRunner.query(
            `
                CREATE VIEW azure_temp.AzureEnrollmentsView
                            AS
                        SELECT
                            rowID,
                            workflowType,
                            userAzureID,
                            classAzureID,
                            organizationAzureID,
                            inProcessing,
                            errorMessage,
                            createdOn,
                            updatedOn,
                            guid,
                            retryAttempts,
                            status
                FROM
                    azure_temp.Enrollments e
                UNION
                                     SELECT
                            rowID,
                            workflowType,
                            userAzureID,
                            classAzureID,
                            organizationAzureID,
                            inProcessing,
                            errorMessage,
                            createdOn,
                            updatedOn,
                            guid,
                            retryAttempts,
                            status
                FROM
                    azure_temp.EnrollmentsArchived ea
                
            `,
        );
    }
}
