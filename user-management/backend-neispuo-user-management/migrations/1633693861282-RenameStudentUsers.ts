import { MigrationInterface, QueryRunner } from 'typeorm';

export class RenameStudentUsers1633693861282 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `EXEC sp_rename
            @objname = 'azure_temp.StudentUsers',
            @newname = 'StudentTeacherUsers'; `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `EXEC sp_rename
            @objname = 'azure_temp.StudentTeacherUsers',
            @newname = 'StudentUsers'; `,
            undefined,
        );
    }
}
