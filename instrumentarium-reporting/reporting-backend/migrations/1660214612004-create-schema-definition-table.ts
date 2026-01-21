import { MigrationInterface, QueryRunner } from 'typeorm';

export class createSchemaDefinitionTable1660214612004
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `CREATE TABLE "reporting"."SchemaDefinition" (
                      "SchemaDefinitionID" INT NOT NULL IDENTITY(1,1), 
                      "Name" NVARCHAR(1024) NOT NULL, 
                      "Definition" NVARCHAR(MAX) NOT NULL, 
                      CONSTRAINT "PK_SchemaDefinition_SchemaDefinitionID" PRIMARY KEY ("SchemaDefinitionID")
                  )`,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
