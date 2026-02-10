import { MigrationInterface, QueryRunner } from 'typeorm';

export class addDataInRStudentsDetailsTable1694674474229
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `INSERT INTO [reporting].R_Students_Details_Table WITH (TABLOCK)
        SELECT * from [reporting].R_StudentsDetails`,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
