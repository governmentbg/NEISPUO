import { MigrationInterface, QueryRunner } from 'typeorm';

export class RStudentsNoCSOPRemoveField1683627250525
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    const noCSOPPropertyName = 'RStudents.StudentsNO_CSOPCount';
    /**
     * Remove field R_Students.StudentsNO_CSOPCount
     */
    const rStudentReports = await queryRunner.query(
      `select ReportID, Query from reporting.Report r  where DatabaseView  = 'RStudents'`,
    );

    let updateStatements = '';

    for (const rStudentReport of rStudentReports) {
      if (!rStudentReport.Query) {
        continue;
      }
      let savedQuery = JSON.parse(rStudentReport.Query);
      delete savedQuery.filters[`${noCSOPPropertyName}`];
      const institutionDepartmentNameArray =
        savedQuery.filters['RStudents.InstitutionDepartmentName'];
      if (institutionDepartmentNameArray) {
        for (let f of savedQuery.filters[
          'RStudents.InstitutionDepartmentName'
        ]) {
          if (f.value === 'Основна институция') {
            f.value = 'Основна сграда';
          }
        }
      }
      savedQuery.dimensions = savedQuery.dimensions.filter(
        (d) => d.name !== noCSOPPropertyName,
      );
      savedQuery.measures = savedQuery.measures.filter(
        (m) => m.name !== noCSOPPropertyName,
      );
      savedQuery.columns = savedQuery.columns.filter(
        (c) => c.name !== noCSOPPropertyName,
      );
      for (let col of savedQuery.columns) {
        if (col.name === 'RStudents.RomeClassName') {
          col.shortTitle = 'Випуск';
        } else if (col.name === 'RStudents.ClassKind') {
          col.shortTitle = 'Категория';
        } else if (col.name === 'RStudents.ClassType') {
          col.shortTitle = 'Профил/вид на паралелка/група';
        } else if (col.name === 'RStudents.StudentCSOPCount') {
          col.shortTitle = 'Брой ученици, обучаващи се в ЦСОП';
        } else if (col.name === 'RStudents.StudentSOPCount') {
          col.shortTitle = 'Брой ученици със СОП';
        } else if (col.name === 'RStudents.IsCSOP') {
          col.shortTitle = 'Паралелка/група за деца със СОП';
        } else if (col.name === 'RStudents.IsNotPresentForm') {
          col.shortTitle = 'СФО';
        } else if (col.name === 'RStudents.IsHourlyOrganization') {
          col.shortTitle = 'Брой деца на почасова организация';
        } else if (col.name === 'RStudents.IsStateFunded') {
          col.shortTitle = 'С национално значение';
        } else if (col.name === 'RStudents.HasMunDecisionFor4') {
          col.shortTitle = 'Решение за задължително обучение на 4 годишните';
        }
      }

      for (let dim of savedQuery.dimensions) {
        if (dim.name === 'RStudents.RomeClassName') {
          dim.shortTitle = 'Випуск';
        } else if (dim.name === 'RStudents.ClassKind') {
          dim.shortTitle = 'Категория';
        } else if (dim.name === 'RStudents.ClassType') {
          dim.shortTitle = 'Профил/вид на паралелка/група';
        } else if (dim.name === 'RStudents.StudentCSOPCount') {
          dim.shortTitle = 'Брой ученици, обучаващи се в ЦСОП';
        } else if (dim.name === 'RStudents.StudentSOPCount') {
          dim.shortTitle = 'Брой ученици със СОП';
        } else if (dim.name === 'RStudents.IsCSOP') {
          dim.shortTitle = 'Паралелка/група за деца със СОП';
        } else if (dim.name === 'RStudents.IsNotPresentForm') {
          dim.shortTitle = 'СФО';
        } else if (dim.name === 'RStudents.IsHourlyOrganization') {
          dim.shortTitle = 'Брой деца на почасова организация';
        } else if (dim.name === 'RStudents.IsStateFunded') {
          dim.shortTitle = 'С национално значение';
        } else if (dim.name === 'RStudents.HasMunDecisionFor4') {
          dim.shortTitle = 'Решение за задължително обучение на 4 годишните';
        }
      }
      updateStatements += `UPDATE reporting.Report set Query = '${JSON.stringify(
        savedQuery,
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
