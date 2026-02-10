import { MigrationInterface, QueryRunner } from 'typeorm';

export class DropAzureCreateProcudures1638440407220 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP PROCEDURE IF EXISTS azure_temp.CREATE_AZURE_USER;', undefined);
        await queryRunner.query('DROP PROCEDURE IF EXISTS azure_temp.CREATE_AZURE_INSTITUTION_USER;', undefined);
        await queryRunner.query('DROP PROCEDURE IF EXISTS azure_temp.UPDATE_AZURE_USER_STUDENT;', undefined);
        await queryRunner.query('DROP PROCEDURE IF EXISTS azure_temp.UPDATE_AZURE_USER_TEACHER;', undefined);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `CREATE PROCEDURE azure_temp.CREATE_AZURE_USER  
            (
                @PersonID int,
                @UserName VARCHAR(100),
                @InitialPassword VARCHAR(100),
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
                                    InitialPassword,
                                    PersonID,
                                    IsAzureUser,
                                    IsAzureSynced
                                )
                            VALUES (
                                @UserName,
                                @InitialPassword,
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
            `CREATE PROCEDURE azure_temp.CREATE_AZURE_INSTITUTION_USER
            (
                @InstitutionID int,
                @FirstName VARCHAR(100),
                @MiddleName VARCHAR(100),
                @LastName VARCHAR(100),
                @UserName VARCHAR(100),
                @InitialPassword VARCHAR(100),
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
                                RowID = @JobID;
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
                                    InitialPassword,
                                    PersonID,
                                    IsAzureUser,
                                    IsAzureSynced
                                )
                            OUTPUT inserted.SysUserID into @tableThatHoldsInsertedSysUserID
                            VALUES (
                                @UserName,
                                @InitialPassword,
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
            `
            CREATE PROCEDURE azure_temp.UPDATE_AZURE_USER_STUDENT
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
                END;
            `,
            undefined,
        );

        await queryRunner.query(
            `
            CREATE PROCEDURE azure_temp.UPDATE_AZURE_USER_TEACHER
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
                END;
            `,
            undefined,
        );
    }
}
