import { MigrationInterface, QueryRunner } from 'typeorm';

export class reportsTable1654845518422 implements MigrationInterface {
  name = 'createSchemaRoleAccessTable1654845518430';

  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `CREATE TABLE "reporting"."SchemaRoleAccess" (
                "SchemaRoleAccessID" INT NOT NULL IDENTITY(1,1), 
                "SchemaName" NVARCHAR(1024) NOT NULL, 
                "AllowedSysRole" INT NOT NULL, 
                CONSTRAINT "PK_SchemaRoleAccess_SchemaRoleAccessID" PRIMARY KEY ("SchemaRoleAccessID"),
                CONSTRAINT "FK_SchemaRoleAccess_AllowedSysRole" FOREIGN KEY ("AllowedSysRole")
                REFERENCES core.SysRole (SysRoleID)
            )`,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`DROP TABLE "reporting"."SchemaRoleAccess"`);
  }
}
