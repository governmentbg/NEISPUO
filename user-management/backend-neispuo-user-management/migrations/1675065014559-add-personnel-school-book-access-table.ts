import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddPersonnelSchoolBookAccess1675065014559 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
           CREATE TABLE [school_books].[PersonnelSchoolBookAccess]
            (
                 RowID int IDENTITY(1, 1) NOT NULL,
                 SchoolYear smallint,
                 ClassBookID int,
                 PersonID int,
                 CONSTRAINT Teacher_Classbook_PK PRIMARY KEY (PersonID, ClassBookID),
                 CONSTRAINT FK_ClassBook
                 FOREIGN KEY (SchoolYear, ClassBookID) REFERENCES [school_books].[ClassBook](SchoolYear, ClassBookId),
                 CONSTRAINT FK_Person
         FOREIGN KEY (PersonID) REFERENCES [core].[Person] (PersonID),
);
            `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
             DROP TABLE [school_books].[PersonnelSchoolBookAccess];
            `,
            undefined,
        );
    }
}
