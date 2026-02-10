import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddRegionAndMunicipalityToReport1657896137427
  implements MigrationInterface
{
  name = 'AddRegionAndMunicipalityToReport1657896137427';

  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `ALTER TABLE "reporting"."Report" ADD "RegionID" INT DEFAULT NULL`,
    );
    await queryRunner.query(
      `ALTER TABLE "reporting"."Report" ADD "MunicipalityID" INT DEFAULT NULL`,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `ALTER TABLE "reporting"."Report" DROP COLUMN "MunicipalityID"`,
    );
    await queryRunner.query(
      `ALTER TABLE "reporting"."Report" DROP COLUMN "RegionID"`,
    );
  }
}
