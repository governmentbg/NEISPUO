import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class RenameTechRole1644489835330 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
                UPDATE 
                    core.SysRole
                SET 
                    Name = 'Институция (IT Администратор)',
                    Description  ='Институция (IT Администратор)'
                WHERE
                    SysRoleID = ${RoleEnum.TECHNICAL_INSTITUTION}
                    AND NAME = 'Институция (техн. сътрудник)'
                    AND Description = 'Институция (техн. сътрудник)'
                `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            UPDATE 
                core.SysRole
            SET 
                NAME = 'Институция (техн. сътрудник)',
                Description = 'Институция (техн. сътрудник)'
            WHERE
                SysRoleID = ${RoleEnum.TECHNICAL_INSTITUTION}
                AND Name = 'Институция (IT Администратор)'
                AND Description  ='Институция (IT Администратор)'
            `,
            undefined,
        );
    }
}
