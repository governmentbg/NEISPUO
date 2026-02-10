import { MigrationInterface, QueryRunner } from 'typeorm';

export class UpdatePersonnelSchoolBookAccessTable1678649463077 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
           DROP TABLE [school_books].[PersonnelSchoolBookAccess]
            `,
        );
        await queryRunner.query(
            `        
             CREATE TABLE [school_books].[PersonnelSchoolBookAccess]
            (
                 RowID int IDENTITY(1, 1) PRIMARY KEY NOT NULL,
                 SchoolYear smallint,
                 ClassBookID int,
                 PersonID int,
                 HasAdminAccess bit NOT NULL,
                 CONSTRAINT FK_ClassBook
                 FOREIGN KEY (SchoolYear, ClassBookID) REFERENCES [school_books].[ClassBook](SchoolYear, ClassBookId),
                 CONSTRAINT FK_Person
         FOREIGN KEY (PersonID) REFERENCES [core].[Person] (PersonID),
);
            `,
        );

        await queryRunner.query(
            `CREATE UNIQUE INDEX uidx_PersonID_SchoolYear_ClassBookID
ON [school_books].PersonnelSchoolBookAccess (SchoolYear, ClassBookID, PersonID);
`,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `DROP INDEX uidx_PersonID_SchoolYear_ClassBookID
ON [school_books].PersonnelSchoolBookAccess;`,
        );
    }
}
