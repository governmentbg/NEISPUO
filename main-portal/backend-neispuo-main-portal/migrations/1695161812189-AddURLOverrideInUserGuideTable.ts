import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddURLOverrideInUserGuideTable1695161812189
  implements MigrationInterface {
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      ` ALTER TABLE [portal].[UserGuide] ADD
        URLOverride VARCHAR(2048) DEFAULT NULL
      `,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
