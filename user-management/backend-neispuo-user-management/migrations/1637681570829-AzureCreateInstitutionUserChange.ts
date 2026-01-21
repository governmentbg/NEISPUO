import { MigrationInterface, QueryRunner } from 'typeorm';

export class AzureCreateInstitutionUserChange1637681570829 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP PROCEDURE IF EXISTS azure_temp.CREATE_AZURE_INSTITUTION_USER;', undefined);

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
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP PROCEDURE IF EXISTS azure_temp.CREATE_AZURE_INSTITUTION_USER;', undefined);

        await queryRunner.query(
            `
            CREATE PROCEDURE azure_temp.CREATE_AZURE_INSTITUTION_USER
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
    }
}
