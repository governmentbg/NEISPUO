import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class addNewPersonalViewVisibilityForMONAndRUO1724664461698
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
            INSERT INTO reporting.SchemaRoleAccess(SchemaName, AllowedSysRole)
            VALUES
            ('${SysSchemaEnum.R_PERSONAL_LP}', ${SysRoleEnum.MON_ADMIN}),
            ('${SysSchemaEnum.R_PERSONAL_LP}', ${SysRoleEnum.RUO}),
            ('${SysSchemaEnum.R_PERSONAL_LP}', ${SysRoleEnum.MON_ADMIN}),
            ('${SysSchemaEnum.R_PERSONAL_LP}', ${SysRoleEnum.RUO_EXPERT}),
            ('${SysSchemaEnum.R_PERSONAL_POKS}', ${SysRoleEnum.MON_ADMIN}),
            ('${SysSchemaEnum.R_PERSONAL_POKS}', ${SysRoleEnum.RUO}),
            ('${SysSchemaEnum.R_PERSONAL_POKS}', ${SysRoleEnum.MON_ADMIN}),
            ('${SysSchemaEnum.R_PERSONAL_POKS}', ${SysRoleEnum.RUO_EXPERT}),
            ('${SysSchemaEnum.R_PERSONAL_P}', ${SysRoleEnum.MON_ADMIN}),
            ('${SysSchemaEnum.R_PERSONAL_P}', ${SysRoleEnum.RUO}),
            ('${SysSchemaEnum.R_PERSONAL_P}', ${SysRoleEnum.MON_ADMIN}),
            ('${SysSchemaEnum.R_PERSONAL_P}', ${SysRoleEnum.RUO_EXPERT})
        `);
  }
  public async down(queryRunner: QueryRunner): Promise<void> {}
}
