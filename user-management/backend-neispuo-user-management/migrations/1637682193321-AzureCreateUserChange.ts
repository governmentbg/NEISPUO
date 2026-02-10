import { MigrationInterface, QueryRunner } from 'typeorm';

export class AzureCreateUserChange1637682193321 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP PROCEDURE IF EXISTS azure_temp.CREATE_AZURE_USER;', undefined);

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
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP PROCEDURE IF EXISTS azure_temp.CREATE_AZURE_USER;', undefined);

        await queryRunner.query(
            `
            CREATE PROCEDURE azure_temp.CREATE_AZURE_USER  
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
    }
}
