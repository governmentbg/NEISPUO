import { MigrationInterface, QueryRunner } from 'typeorm';

export class ExtendErrorMessageField1643819338672 implements MigrationInterface {
    /*
        This migration will help us contain responses from telelink so in the future we could improve code.
    */
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`ALTER TABLE azure_temp.Users ALTER COLUMN ErrorMessage VARCHAR (1000);`, undefined);
        await queryRunner.query(
            `ALTER TABLE azure_temp.UsersArchived ALTER COLUMN ErrorMessage VARCHAR (1000);`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Organizations ALTER COLUMN ErrorMessage VARCHAR (1000);`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.OrganizationsArchived ALTER COLUMN ErrorMessage VARCHAR (1000);`,
            undefined,
        );
        await queryRunner.query(`ALTER TABLE azure_temp.Classes ALTER COLUMN ErrorMessage VARCHAR (1000);`, undefined);
        await queryRunner.query(
            `ALTER TABLE azure_temp.ClassesArchived ALTER COLUMN ErrorMessage VARCHAR (1000);`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments ALTER COLUMN ErrorMessage VARCHAR (1000);`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.EnrollmentsArchived ALTER COLUMN ErrorMessage VARCHAR (1000);`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        /*
            previous value of the fields was 255. Im putting this in this comment if
            someone can help in the future with the down as i cannot figure a way to 
            make it work now
            Problem is that i cannot update a field to a smaller length if there is 
            a bigger value in the column lets say i want to make the lenght 1 but 
            the re is the world 'hello'(5) in the table 
            03.02.2022 Ivelin
        */
    }
}
