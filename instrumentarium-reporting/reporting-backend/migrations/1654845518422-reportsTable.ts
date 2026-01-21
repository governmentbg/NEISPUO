import { MigrationInterface, QueryRunner } from 'typeorm';

export class reportsTable1654845518422 implements MigrationInterface {
  name = 'reportsTable1654845518422';

  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`DROP TABLE IF EXISTS "reporting"."Report"`);

    await queryRunner.query(
      `CREATE TABLE "reporting"."Report" (
                "ReportID" INT NOT NULL IDENTITY(1,1), 
                "Name" NVARCHAR(512) NOT NULL, 
                "Description" NVARCHAR(MAX) DEFAULT NULL, 
                "SharedWith" NVARCHAR(MAX) DEFAULT NULL, 
                "Query" NVARCHAR(MAX) DEFAULT NULL, 
                "DatabaseView" NVARCHAR(255) NOT NULL, 
                "Visualization" NVARCHAR(255) NOT NULL, 
                "CreatedBy" INT NOT NULL, 
                CONSTRAINT "PK_Report_ReportID" PRIMARY KEY ("ReportID"),
                CONSTRAINT "FK_Report_CreatedBy" FOREIGN KEY ("CreatedBy")
                REFERENCES core.SysUser (SysUserID)
            )`,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`DROP TABLE "reporting"."Report"`);
  }
}
