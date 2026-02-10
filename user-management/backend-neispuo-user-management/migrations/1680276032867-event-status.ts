import {MigrationInterface, QueryRunner} from "typeorm";

export class EventStatus1680276032867 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE TABLE azure_temp.EventStatus (
                RowID int NOT NULL,
                Name varchar(100) NOT NULL
                PRIMARY KEY (RowID)
            );
            
            `,
            undefined,
        );
        await queryRunner.query(
            `
                INSERT INTO AZURE_TEMP.EVENTSTATUS (ROWID,NAME) VALUES
                (0,N'AWAITING_CREATION'),
                (1,N'IN_CREATION'),
                (2,N'FAILED'),
                (3,N'SUCCESSFUL'),
                (4,N'SYNCHRONIZED'),
                (5,N'FAILED_CREATION'),
                (6,N'FAILED_SYNCRONIZATION'),
                (7,N'IN_USERNAME_GENERATION'),
                (8,N'FAILED_USERNAME_GENERATION'),
                (500,N'STUCK');
            
            `,
            undefined,
        );
        await queryRunner.query(
            `
            UPDATE azure_temp.Users 
                SET Status =4
                WHERE Status NOT IN (0,1,2,3,4,5,6,7,8,500)
            `,
            undefined,
        );
        await queryRunner.query(
            `
            UPDATE azure_temp.Enrollments 
                SET Status =4
                WHERE Status NOT IN (0,1,2,3,4,5,6,7,8,500)
            `,
            undefined,
        );
        
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Classes  ADD CONSTRAINT FK_Classes_Status FOREIGN KEY (Status) REFERENCES azure_temp.EventStatus(RowID)
            `,
            undefined,
        );
        
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Organizations  ADD CONSTRAINT FK_Organizations_Status FOREIGN KEY (Status) REFERENCES azure_temp.EventStatus(RowID)
            `,
            undefined,
        );
        
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Users  ADD CONSTRAINT FK_Users_Status FOREIGN KEY (Status) REFERENCES azure_temp.EventStatus(RowID)
            `,
            undefined,
        );
        
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Enrollments  ADD CONSTRAINT FK_Enrollments_Status FOREIGN KEY (Status) REFERENCES azure_temp.EventStatus(RowID)
            `,
            undefined,
        );
        
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.ClassesArchived  ADD CONSTRAINT FK_ClassesArchived_Status FOREIGN KEY (Status) REFERENCES azure_temp.EventStatus(RowID)
            `,
            undefined,
        );
        
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.OrganizationsArchived  ADD CONSTRAINT FK_OrganizationsArchived_Status FOREIGN KEY (Status) REFERENCES azure_temp.EventStatus(RowID)
            `,
            undefined,
        );
        
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.UsersArchived  ADD CONSTRAINT FK_UsersArchived_Status FOREIGN KEY (Status) REFERENCES azure_temp.EventStatus(RowID)
            `,
            undefined,
        );
        
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.EnrollmentsArchived  ADD CONSTRAINT FK_EnrollmentsArchived_Status FOREIGN KEY (Status) REFERENCES azure_temp.EventStatus(RowID)
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Classes  DROP CONSTRAINT FK_Classes_Status 
            `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Organizations  DROP CONSTRAINT FK_Organizations_Status
            `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Users  DROP CONSTRAINT FK_Users_Status  
            `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Enrollments  DROP CONSTRAINT FK_Enrollments_Status
            `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.ClassesArchived  DROP CONSTRAINT FK_ClassesArchived_Status
            `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.OrganizationsArchived  DROP CONSTRAINT FK_OrganizationsArchived_Status
            `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.UsersArchived  DROP CONSTRAINT FK_UsersArchived_Status
            `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.EnrollmentsArchived  DROP CONSTRAINT FK_EnrollmentsArchived_Status 
            `,
            undefined,
        );
        await queryRunner.query(
            `
            DROP TABLE azure_temp.EventStatus
            `,
            undefined,
        );
    }

}
