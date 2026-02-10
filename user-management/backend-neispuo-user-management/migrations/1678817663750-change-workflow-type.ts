import {MigrationInterface, QueryRunner} from "typeorm";

export class ChangeWorkflowType1678817663750 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            BEGIN
                BEGIN TRY
                    BEGIN TRANSACTION
                    CREATE TABLE azure_temp.WorkflowTypes (
                        RowID int NOT NULL,
                        Name varchar(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
                        CONSTRAINT WorkflowTypes_PK PRIMARY KEY (RowID)
                    );
                    
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(1, N'SCHOOL_CREATE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(2, N'SCHOOL_UPDATE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(3, N'SCHOOL_DELETE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(4, N'CLASS_CREATE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(5, N'CLASS_UPDATE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(6, N'CLASS_DELETE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(7, N'USER_CREATE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(8, N'USER_UPDATE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(9, N'USER_DELETE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(10, N'ENROLLMENT_CREATE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(11, N'ENROLLMENT_CLASS_CREATE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(12, N'ENROLLMENT_SCHOOL_CREATE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(13, N'ENROLLMENT_DELETE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(14, N'ENROLLMENT_CLASS_DELETE');
                    INSERT INTO azure_temp.WorkflowTypes
                    (RowID, Name)
                    VALUES(15, N'ENROLLMENT_SCHOOL_DELETE');
                    
                         
                    ALTER TABLE azure_temp.Enrollments  SET ( SYSTEM_VERSIONING = OFF )
                    ALTER TABLE azure_temp.Classes  SET ( SYSTEM_VERSIONING = OFF )
                    ALTER TABLE azure_temp.Organizations  SET ( SYSTEM_VERSIONING = OFF )
                    ALTER TABLE azure_temp.Users  SET ( SYSTEM_VERSIONING = OFF )
                    
                    UPDATE azure_temp.Classes 
                    SET WorkflowType  = 
                        CASE  
                            WHEN WorkflowType = 'SCHOOL_CREATE' THEN 1   
                            WHEN WorkflowType = 'SCHOOL_UPDATE' THEN 2   
                            WHEN WorkflowType = 'SCHOOL_DELETE' THEN 3   
                            WHEN WorkflowType = 'CLASS_CREATE' THEN 4   
                            WHEN WorkflowType = 'CLASS_UPDATE' THEN 5  
                            WHEN WorkflowType = 'CLASS_DELETE' THEN 6  
                            WHEN WorkflowType = 'USER_CREATE' THEN 7    
                            WHEN WorkflowType = 'USER_UPDATE' THEN 8   
                            WHEN WorkflowType = 'USER_DELETE' THEN 9   
                            WHEN WorkflowType = 'ENROLLMENT_CREATE' THEN 10   
                            WHEN WorkflowType = 'ENROLLMENT_CLASS_CREATE' THEN 11      
                            WHEN WorkflowType = 'ENROLLMENT_SCHOOL_CREATE' THEN 12       
                            WHEN WorkflowType = 'ENROLLMENT_DELETE' THEN 13     
                            WHEN WorkflowType = 'ENROLLMENT_CLASS_DELETE' THEN 14      
                            WHEN WorkflowType = 'ENROLLMENT_SCHOOL_DELETE' THEN 15     
                            ELSE WorkflowType  
                         END 
                    
                    UPDATE azure_temp.ClassesArchived 
                    SET WorkflowType  = 
                        CASE    
                            WHEN WorkflowType = 'CLASS_CREATE' THEN 4   
                            WHEN WorkflowType = 'CLASS_UPDATE' THEN 5  
                            WHEN WorkflowType = 'CLASS_DELETE' THEN 6     
                            ELSE 0  
                         END
                         
                    ALTER TABLE azure_temp.Classes ALTER COLUMN WorkflowType INT NOT NULL;   
                    ALTER TABLE azure_temp.ClassesArchived ALTER COLUMN WorkflowType INT NOT NULL;
                    
                    
                    UPDATE azure_temp.Organizations 
                    SET WorkflowType  = 
                        CASE  
                            WHEN WorkflowType = 'SCHOOL_CREATE' THEN 1   
                            WHEN WorkflowType = 'SCHOOL_UPDATE' THEN 2   
                            WHEN WorkflowType = 'SCHOOL_DELETE' THEN 3   
                            ELSE WorkflowType  
                         END 
                    
                    UPDATE azure_temp.OrganizationsArchived 
                    SET WorkflowType =
                        CASE  
                            WHEN WorkflowType = 'SCHOOL_CREATE' THEN 1   
                            WHEN WorkflowType = 'SCHOOL_UPDATE' THEN 2   
                            WHEN WorkflowType = 'SCHOOL_DELETE' THEN 3 
                            ELSE 0  
                         END
                         
                    ALTER TABLE azure_temp.Organizations ALTER COLUMN WorkflowType INT NOT NULL;   
                    ALTER TABLE azure_temp.OrganizationsArchived ALTER COLUMN WorkflowType INT NOT NULL;
                    
                    
                    UPDATE azure_temp.Users  
                    SET WorkflowType  = 
                        CASE    
                            WHEN WorkflowType = 'USER_CREATE' THEN 7    
                            WHEN WorkflowType = 'USER_UPDATE' THEN 8   
                            WHEN WorkflowType = 'USER_DELETE' THEN 9    
                            ELSE WorkflowType  
                         END 
                    
                    UPDATE azure_temp.UsersArchived 
                    SET WorkflowType    = 
                        CASE    
                            WHEN WorkflowType = 'USER_CREATE' THEN 7    
                            WHEN WorkflowType = 'USER_UPDATE' THEN 8   
                            WHEN WorkflowType = 'USER_DELETE' THEN 9   
                            ELSE 0  
                         END
                         
                    ALTER TABLE azure_temp.Users ALTER COLUMN WorkflowType INT NOT NULL;   
                    ALTER TABLE azure_temp.UsersArchived ALTER COLUMN WorkflowType INT NOT NULL;
                    
                    
                    UPDATE azure_temp.Enrollments  
                    SET WorkflowType  = 
                        CASE   
                            WHEN WorkflowType = 'ENROLLMENT_CREATE' THEN 10   
                            WHEN WorkflowType = 'ENROLLMENT_CLASS_CREATE' THEN 11      
                            WHEN WorkflowType = 'ENROLLMENT_SCHOOL_CREATE' THEN 12       
                            WHEN WorkflowType = 'ENROLLMENT_DELETE' THEN 13     
                            WHEN WorkflowType = 'ENROLLMENT_CLASS_DELETE' THEN 14      
                            WHEN WorkflowType = 'ENROLLMENT_SCHOOL_DELETE' THEN 15     
                            ELSE WorkflowType  
                         END 
                    
                    UPDATE azure_temp.EnrollmentsArchived 
                    SET WorkflowType  = 
                        CASE    
                            WHEN WorkflowType = 'ENROLLMENT_CREATE' THEN 10   
                            WHEN WorkflowType = 'ENROLLMENT_CLASS_CREATE' THEN 11      
                            WHEN WorkflowType = 'ENROLLMENT_SCHOOL_CREATE' THEN 12       
                            WHEN WorkflowType = 'ENROLLMENT_DELETE' THEN 13     
                            WHEN WorkflowType = 'ENROLLMENT_CLASS_DELETE' THEN 14      
                            WHEN WorkflowType = 'ENROLLMENT_SCHOOL_DELETE' THEN 15      
                            ELSE 0  
                         END
                         
                    ALTER TABLE azure_temp.Enrollments ALTER COLUMN WorkflowType INT NOT NULL;   
                    ALTER TABLE azure_temp.EnrollmentsArchived ALTER COLUMN WorkflowType INT NOT NULL;
                    
                    ALTER TABLE azure_temp.Classes ADD CONSTRAINT Classes_FK FOREIGN KEY (WorkflowType) REFERENCES azure_temp.WorkflowTypes(RowID);
                    ALTER TABLE azure_temp.Users ADD CONSTRAINT Users_FK FOREIGN KEY (WorkflowType) REFERENCES azure_temp.WorkflowTypes(RowID);
                    ALTER TABLE azure_temp.Organizations  ADD CONSTRAINT Organizations_FK FOREIGN KEY (WorkflowType) REFERENCES azure_temp.WorkflowTypes(RowID);
                    ALTER TABLE azure_temp.Enrollments  ADD CONSTRAINT Enrollments_FK FOREIGN KEY (WorkflowType) REFERENCES azure_temp.WorkflowTypes(RowID);
                    
                    COMMIT;
                END TRY
                BEGIN CATCH
                    ROLLBACK;
                END CATCH
            END;
            `,
        );
    }

    
    public async down(queryRunner: QueryRunner): Promise<void> {
    }

}
