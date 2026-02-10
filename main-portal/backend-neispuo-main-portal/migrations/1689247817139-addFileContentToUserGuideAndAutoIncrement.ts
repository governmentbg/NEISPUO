import { MigrationInterface, QueryRunner } from 'typeorm';

export class addFileContentToUserGuideAndAutoIncrement1689247817139
  implements MigrationInterface {
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query('DROP TABLE [portal].[UserGuide]', undefined);

    await queryRunner.query(
      `CREATE TABLE portal.UserGuide (
	    UserGuideID int IDENTITY(1,1) NOT NULL,
	    Name nvarchar(255) NOT NULL,
	    CategoryID int NOT NULL,
	    FileContent varbinary(MAX) NULL,
	    CONSTRAINT PK__UserGuid__FB19A5CC6B8B1470 PRIMARY KEY (UserGuideID));`,
      undefined,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
