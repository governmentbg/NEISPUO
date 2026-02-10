import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddUserGuideColumns1694444914127 implements MigrationInterface {
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      ` ALTER TABLE [portal].[UserGuide] ADD
        Filename VARCHAR(2048) DEFAULT NULL,
        MimeType VARCHAR(1024)DEFAULT NULL;
      `,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
