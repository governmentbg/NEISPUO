import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class ristudentsRinstitutionsGiveAccessToBudgetingInstitutions1683204298382
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_STUDENTS}', ${SysRoleEnum.BUDGETING_INSTITUTION});`);
    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_INSTITUTIONS}', ${SysRoleEnum.BUDGETING_INSTITUTION});`);
    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_RZI_STUDENTS}', ${SysRoleEnum.BUDGETING_INSTITUTION});`);
    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_PERSONAL}', ${SysRoleEnum.BUDGETING_INSTITUTION});`);
    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.PHONES}', ${SysRoleEnum.BUDGETING_INSTITUTION});`);
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
