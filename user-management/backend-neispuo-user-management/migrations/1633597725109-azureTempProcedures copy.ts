import { MigrationInterface, QueryRunner } from 'typeorm';

// eslint-disable-next-line @typescript-eslint/naming-convention
export class azureTempProcedures1633597725109 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `CREATE PROCEDURE  azure_temp.ARCHIVE_CLASSES AS
                BEGIN
                    INSERT
                        INTO
                        "azure_temp"."Classes_History"
                    SELECT
                        *
                    FROM
                        "azure_temp"."Classes";
                    TRUNCATE TABLE "azure_temp"."Classes"
                END;`,
            undefined,
        );

        await queryRunner.query(
            `CREATE PROCEDURE  azure_temp.ARCHIVE_ENROLLMENTS AS
                BEGIN
                    INSERT
                        INTO
                        "azure_temp"."Enrollments_History"
                    SELECT
                        *
                    FROM
                        "azure_temp"."Enrollments";
                    TRUNCATE TABLE "azure_temp"."Enrollments"
                END;`,
            undefined,
        );

        await queryRunner.query(
            `CREATE PROCEDURE  azure_temp.ARCHIVE_ORGANIZATIONS AS
                BEGIN
                    INSERT
                        INTO
                        "azure_temp"."Organizations_History"
                    SELECT
                        *
                    FROM
                        "azure_temp"."Organizations";
                    TRUNCATE TABLE "azure_temp"."Organizations"
                END;`,
            undefined,
        );

        await queryRunner.query(
            `CREATE PROCEDURE  azure_temp.ARCHIVE_USERS AS
                BEGIN
                    INSERT
                        INTO
                        "azure_temp"."Users_History"
                    SELECT
                        *
                    FROM
                        "azure_temp"."Users";
                    TRUNCATE TABLE "azure_temp"."Users"
                END;`,
            undefined,
        );

        await queryRunner.query(
            `CREATE PROCEDURE azure_temp.CREATE_AZURE_INSTITUTION_USER
                (
                    @InstitutionID int,
                    @FirstName VARCHAR(100),
                    @MiddleName VARCHAR(100),
                    @LastName VARCHAR(100),
                    @UserName VARCHAR(100),
                    @IsAzureUser int,
                    @IsAzureSynced int,
                    @JobID int, 
                    @JobStatus int, 
                    @InProcessing int
                )  
                AS
                BEGIN
                    DECLARE @ResultValue int
                        SET @ResultValue = 0;
                        BEGIN TRY
                            BEGIN TRANSACTION
                            DECLARE @PersonID int;
                            DECLARE @tableThatHoldsInsertedPersonID TABLE(PersonID int);
                            DECLARE @SysUserID int;
                            DECLARE @tableThatHoldsInsertedSysUserID TABLE(SysUserID int);
                                UPDATE
                                    azure_temp.Organizations
                                SET
                                    UpdatedOn = GETDATE(),
                                    Status = @JobStatus,
                                    InProcessing = @InProcessing
                                WHERE
                                    RowID = @JobID
                                INSERT 
                                    INTO
                                    core.Person (
                                        firstName,
                                        middleName,
                                        lastName
                                    )
                                OUTPUT inserted.PersonID into @tableThatHoldsInsertedPersonID
                                VALUES (
                                @FirstName,
                                @MiddleName,
                                @LastName
                                );        
                                SELECT @PersonID = PersonID from @tableThatHoldsInsertedPersonID     
                                INSERT INTO
                                    core.SysUser (
                                        Username,
                                        PersonID,
                                        IsAzureUser,
                                        IsAzureSynced
                                    )
                                OUTPUT inserted.SysUserID into @tableThatHoldsInsertedSysUserID
                                VALUES (
                                    @UserName,
                                    @PersonID,
                                    @IsAzureUser,
                                    @IsAzureSynced
                                );
                                SELECT @SysUserID = SysUserID from @tableThatHoldsInsertedSysUserID;
                            INSERT INTO
                                    core.SysUserSysRole (
                                        SysUserID,
                                        SysRoleID,
                                        InstitutionID
                                    )
                                VALUES (
                                    @SysUserID,
                                    0,
                                    @InstitutionID
                                );
                                SET @ResultValue = 1;
                            COMMIT;
                        END TRY
                        BEGIN CATCH
                        ROLLBACK;
                        UPDATE
                                    azure_temp.Organizations
                                SET
                                    UpdatedOn = GETDATE(),
                                    InProcessing = 0
                                WHERE
                                    RowID = @JobID;
                            RETURN @ResultValue; 
                        END CATCH
                    RETURN @ResultValue; 
                END;`,
            undefined,
        );

        await queryRunner.query(
            `CREATE PROCEDURE azure_temp.CREATE_AZURE_USER  
                (
                    @PersonID int,
                    @UserName VARCHAR(100),
                    @IsAzureUser int,
                    @IsAzureSynced int,
                    @JobID int,
                    @JobStatus int,
                    @InProcessing int
                )  
                AS
                BEGIN
                    DECLARE @ResultValue int
                        SET @ResultValue = 0;
                        BEGIN TRY
                            BEGIN TRANSACTION
                                UPDATE
                                    azure_temp.Users
                                SET
                                    UpdatedOn = GETDATE(),
                                    Status = @JobStatus,
                                    InProcessing = @InProcessing
                                WHERE
                                    RowID = @JobID;
                                INSERT INTO
                                    core.SysUser (
                                        Username,
                                        PersonID,
                                        IsAzureUser,
                                        IsAzureSynced
                                    )
                                VALUES (
                                    @UserName,
                                    @PersonID,
                                    @IsAzureUser,
                                    @IsAzureSynced
                                );
                                SET @ResultValue = 1;
                            COMMIT;
                        END TRY
                        BEGIN CATCH
                        ROLLBACK;
                        UPDATE
                                    azure_temp.Users
                                SET
                                    UpdatedOn = GETDATE(),
                                    InProcessing = 0
                                WHERE
                                    RowID = @JobID;
                            RETURN @ResultValue; 
                        END CATCH
                    RETURN @ResultValue; 
                END;`,
            undefined,
        );

        await queryRunner.query(
            `CREATE PROCEDURE azure_temp.UPDATE_AZURE_USER_STUDENT
                (
                    @PersonID int,
                    @IsAzureSynced int,
                    @JobID int,
                    @JobStatus int,
                    @InProcessing int
                )  
                AS
                BEGIN
                        BEGIN TRY
                            BEGIN TRANSACTION
                                UPDATE
                                    azure_temp.Users
                                SET
                                    UpdatedOn = GETDATE(),
                                    Status = @JobStatus,
                                    InProcessing = @InProcessing
                                WHERE
                                    RowID = @JobID;
                                UPDATE core.SysUser SET
                                        IsAzureSynced = @IsAzureSynced
                                WHERE
                                    PersonID = CAST(@PersonID AS int);
                            COMMIT;
                        END TRY
                        BEGIN CATCH
                        ROLLBACK;
                        UPDATE
                                    azure_temp.Users
                                SET
                                    UpdatedOn = GETDATE(),
                                    InProcessing = 0
                                WHERE
                                    RowID = @JobID;
                        END CATCH
                END;`,
            undefined,
        );

        await queryRunner.query(
            `CREATE PROCEDURE azure_temp.UPDATE_AZURE_USER_TEACHER
                (
                    @PersonID int,
                    @IsAzureSynced int,
                    @JobID int,
                    @JobStatus int,
                    @InProcessing int
                )  
                AS
                BEGIN
                        BEGIN TRY
                            BEGIN TRANSACTION
                                UPDATE
                                    azure_temp.Users
                                SET
                                    UpdatedOn = GETDATE(),
                                    Status = @JobStatus,
                                    InProcessing = @InProcessing
                                WHERE
                                    RowID = @JobID;
                                UPDATE core.SysUser SET
                                        IsAzureSynced = @IsAzureSynced
                                WHERE
                                    PersonID = CAST(@PersonID AS int);
                            COMMIT;
                        END TRY
                        BEGIN CATCH
                        ROLLBACK;
                        UPDATE
                                    azure_temp.Users
                                SET
                                    UpdatedOn = GETDATE(),
                                    InProcessing = 0
                                WHERE
                                    RowID = @JobID;
                        END CATCH
                END;`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_CLASSES;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_ENROLLMENTS;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_ORGANIZATIONS;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_USERS;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.CREATE_AZURE_INSTITUTION_USER;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.CREATE_AZURE_USER;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.UPDATE_AZURE_USER_STUDENT;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.UPDATE_AZURE_USER_TEACHER;', undefined);
    }
}
