import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddParentView1673528975868 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `   
            CREATE VIEW azure_temp.ParentView AS 
                SELECT
                    p.PublicEduNumber AS username,
                    CONCAT(p.firstName, ' ', p.middleName, ' ', p.lastName) as threeNames,
                    p.personID,
                    p.azureID
                FROM
                    core.Person p
                WHERE 
                    p.SysUserType = 3
            `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `   
                DROP VIEW azure_temp.ParentView 
            `,
        );
    }
}
