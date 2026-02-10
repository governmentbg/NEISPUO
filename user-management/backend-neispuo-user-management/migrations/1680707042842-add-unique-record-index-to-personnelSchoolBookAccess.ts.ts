import { MigrationInterface, QueryRunner } from 'typeorm';

export class addUniqueRecordIndexToPersonnelSchoolBookAccess1680707042842 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `CREATE UNIQUE INDEX [UQ_PersonnelSchoolBookAccess_SchoolYear_ClassBookID_PersonID]
            ON [school_books].[PersonnelSchoolBookAccess] ([SchoolYear], [ClassBookID], [PersonID]) INCLUDE ([HasAdminAccess])
            WHERE
            [SchoolYear] IS NOT NULL AND
            [ClassBookID] IS NOT NULL AND
            [PersonID] IS NOT NULL;`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `DROP INDEX UQ_PersonnelSchoolBookAccess_SchoolYear_ClassBookID_PersonID ON [school_books].[PersonnelSchoolBookAccess]`,
            undefined,
        );
    }
}
