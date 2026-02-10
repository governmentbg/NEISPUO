import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddEmailTemplate1748419073280 implements MigrationInterface {
  name = 'AddEmailTemplate1748419073280';

  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `CREATE TABLE "portal"."EmailTemplate" ("Id" int NOT NULL IDENTITY(1,1), "Title" nvarchar(150) NOT NULL, "Content" nvarchar(max) NOT NULL, "Recipients" nvarchar(max), "IsActive" bit NOT NULL CONSTRAINT "DF_8c6582d83b9e5c01b252a7b5daa" DEFAULT 1, "CreatedBy" nvarchar(50) NOT NULL, "UpdatedBy" nvarchar(50) NOT NULL, "CreatedAt" datetime2 NOT NULL CONSTRAINT "DF_91b454a29ed2a6203c488aa2a8f" DEFAULT getdate(), "UpdatedAt" datetime2 NOT NULL CONSTRAINT "DF_13bb4fa3bac81ddf8b475372fd2" DEFAULT getdate(), "EmailTemplateTypeId" int NOT NULL, CONSTRAINT "PK_25ed505c2f947f3c71fab854798" PRIMARY KEY ("Id"))`,
    );
    await queryRunner.query(
      `CREATE TABLE "portal"."EmailTemplateType" ("Id" int NOT NULL IDENTITY(1,1), "DisplayName" nvarchar(100) NOT NULL, "ContentProvider" nvarchar(200) NOT NULL, "Description" nvarchar(max) NOT NULL, "VariableMappings" nvarchar(max) NOT NULL, CONSTRAINT "UQ_482acc7032289b4d10424f8eecc" UNIQUE ("DisplayName"), CONSTRAINT "UQ_4bc8138fa7adcc017508ca1f276" UNIQUE ("ContentProvider"), CONSTRAINT "PK_f746d3fc04e29d3c4a36989b0e4" PRIMARY KEY ("Id"))`,
    );

    await queryRunner.query(
      `ALTER TABLE "portal"."EmailTemplate" ADD CONSTRAINT "FK_4cdc798b70ed07916e2da197ea8" FOREIGN KEY ("EmailTemplateTypeId") REFERENCES "portal"."EmailTemplateType"("Id") ON DELETE NO ACTION ON UPDATE NO ACTION`,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `ALTER TABLE "portal"."EmailTemplate" DROP CONSTRAINT "FK_4cdc798b70ed07916e2da197ea8"`,
    );
    await queryRunner.query(`DROP TABLE "portal"."EmailTemplateType"`);
    await queryRunner.query(`DROP TABLE "portal"."EmailTemplate"`);
  }
}
