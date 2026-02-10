import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class RemoveUserCreateTrigger1649933549807 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP TRIGGER azure_temp.AfterInsert', undefined);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE TRIGGER azure_temp.AfterInsert
            ON azure_temp.Users
            AFTER INSERT
            AS
            BEGIN
                --declare variables
                DECLARE @RowID INT;
                DECLARE @FirstName VARCHAR(100);
                DECLARE @MiddleName VARCHAR(100);
                DECLARE @Surname VARCHAR(100);
                DECLARE @Email VARCHAR(100);
                DECLARE @UserRole VARCHAR(100);
                DECLARE @PersonID INT;
                DECLARE @WorkflowType VARCHAR(100);
                BEGIN
                    --fill variables
                    SELECT
                        @RowID = i.RowID,
                        @FirstName = i.FirstName,
                        @MiddleName = i.MiddleName,
                        @Surname = i.Surname,
                        @Email = i.Email,
                        @UserRole = i.UserRole,
                        @PersonID = i.PersonID,
                        @WorkflowType = i.WorkflowType
                    FROM
                        INSERTED i
                END
                BEGIN
                    
                    SET NOCOUNT ON
                    DECLARE @RETURNVALUE int;
                    DECLARE @RESULTOUT varchar(100) = '';
                    DECLARE @ERRORSOUT varchar(100) = '';
                
                    IF @WorkflowType = 'USER_CREATE'
                        BEGIN
                            --call generate function
                            SELECT @FirstName = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@FirstName);
                            SELECT @MiddleName = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@MiddleName);
                            SELECT @Surname = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@Surname);
                            EXEC @RETURNVALUE = azure_temp.GENERATE_USERNAME 
                                @FirstName = @FirstName,
                                @MiddleName = @MiddleName,
                                @LastName = @Surname, 
                                @Email = @Email, 
                                @AzureUserRole = @UserRole, 
                                @PersonID = @PersonID, 
                                @ERRORS = @ERRORSOUT OUTPUT,
                                @RESULT = @RESULTOUT OUTPUT;
                            IF LEN(@ERRORSOUT) > 0
                                BEGIN
                                    --this will set the status to error and set the error message
                                    UPDATE
                                        azure_temp.Users
                                    SET
                                        Status = ${EventStatus.FAILED_USERNAME_GENERATION},
                                        ErrorMessage = @ERRORSOUT
                                    WHERE RowID = @RowID;
                                END
                            ELSE
                                BEGIN
                                    --if all is good the username should be generated and the status should be set to 0 so the create job can pick up the row.
                                    UPDATE
                                        azure_temp.Users
                                    SET
                                        Status = ${EventStatus.AWAITING_CREATION},
                                        Username = @RESULTOUT
                                    WHERE RowID = @RowID;
                                END
                        END
                    ELSE
                        BEGIN
                            --if all is good the username should be generated and the status should be set to 0 so the create job can pick up the row.
                            UPDATE
                                azure_temp.Users
                            SET
                                Status = ${EventStatus.AWAITING_CREATION},
                                Username = NULL
                            WHERE RowID = @RowID;
                        END
                END;
            END
            `,
            undefined,
        );
    }
}
