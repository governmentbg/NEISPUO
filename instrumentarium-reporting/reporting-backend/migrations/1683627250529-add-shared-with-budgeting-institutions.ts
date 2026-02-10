import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class addSharedWithBudgetingInstitutions1683627250529
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    /**
     * Remove field R_Students.StudentsNO_CSOPCount
     */
    const rStudentReports = await queryRunner.query(
      `select ReportID, SharedWith from reporting.Report r  where SharedWith  LIKE '%3%'`,
    );

    let updateStatements = '';

    for (const rStudentReport of rStudentReports) {
      if (!rStudentReport.SharedWith) {
        continue;
      }
      let sharedWith = JSON.parse(rStudentReport.SharedWith);
      sharedWith.push(SysRoleEnum.BUDGETING_INSTITUTION);

      updateStatements += `UPDATE reporting.Report set SharedWith = '${JSON.stringify(
        sharedWith,
      )}' where ReportID = ${rStudentReport.ReportID};`;
    }
    await queryRunner.query(`
        BEGIN TRANSACTION;  
        ${updateStatements}
        COMMIT;
    `);
  }
  public async down(queryRunner: QueryRunner): Promise<void> {}
}
