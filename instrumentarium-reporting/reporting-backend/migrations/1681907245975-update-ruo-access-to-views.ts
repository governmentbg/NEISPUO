import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class updateRuoAccessToViews1681907245975 implements MigrationInterface {
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
       DELETE FROM "reporting"."SchemaRoleAccess"
       WHERE SchemaName='${SysSchemaEnum.R_CLASSES}' AND AllowedSysRole=${SysRoleEnum.RUO}`);

    await queryRunner.query(`
       DELETE FROM "reporting"."SchemaRoleAccess"
       WHERE SchemaName='${SysSchemaEnum.R_CLASSES}' AND AllowedSysRole=${SysRoleEnum.RUO_EXPERT}`);

    await queryRunner.query(`
       DELETE FROM "reporting"."SchemaRoleAccess"
       WHERE SchemaName='${SysSchemaEnum.R_FOREIGN_LANGUAGES}' AND AllowedSysRole=${SysRoleEnum.RUO}`);

    await queryRunner.query(`
       DELETE FROM "reporting"."SchemaRoleAccess"
       WHERE SchemaName='${SysSchemaEnum.R_FOREIGN_LANGUAGES}' AND AllowedSysRole=${SysRoleEnum.RUO_EXPERT}`);

    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.PHONES}', ${SysRoleEnum.RUO});`);

    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.PHONES}', ${SysRoleEnum.RUO_EXPERT});`);
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_CLASSES}', ${SysRoleEnum.RUO});`);

    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_CLASSES}', ${SysRoleEnum.RUO_EXPERT});`);

    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_FOREIGN_LANGUAGES}', ${SysRoleEnum.RUO});`);

    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_FOREIGN_LANGUAGES}', ${SysRoleEnum.RUO_EXPERT});`);

    await queryRunner.query(`
       DELETE FROM "reporting"."SchemaRoleAccess"
       WHERE SchemaName='${SysSchemaEnum.PHONES}' AND AllowedSysRole=${SysRoleEnum.RUO}`);

    await queryRunner.query(`
       DELETE FROM "reporting"."SchemaRoleAccess"
       WHERE SchemaName='${SysSchemaEnum.PHONES}' AND AllowedSysRole=${SysRoleEnum.RUO_EXPERT}`);
  }
}
