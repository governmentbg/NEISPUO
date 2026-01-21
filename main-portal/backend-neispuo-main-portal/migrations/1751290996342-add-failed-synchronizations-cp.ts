import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddFailedSynchronizationsCP_1751290996342
  implements MigrationInterface {
  name = 'AddFailedSynchronizationsCP_1751290996342';

  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `
      INSERT
      	INTO
      	"portal"."EmailTemplateType"
      ( DisplayName,
      	ContentProvider,
      	Description,
      	VariableMappings)
      VALUES('Неуспешни Синхронизации', 'FailedSynchronizationsCP_Service', 'Неуспешни Синхронизации', '[  { "label": "Start date", "key": "formatDate startDate" },  { "label": "End date", "key": "formatDate endDate" },  { "label": "Occurrences", "key": "users.occurrences" },  { "label": "Event Status Name", "key": "users.eventStatusName" },  { "label": "Workflow Type", "key": "users.workflowType" },  { "label": "Status", "key": "users.status" },  { "label": "Workflow Type Name", "key": "users.workflowTypeName" },  { "label": "Error Message Category", "key": "users.errorMessageCategory" },  { "label": "Example Error Message", "key": "users.exampleErrorMessage" },  { "label": "Row ID Of Example", "key": "users.rowID_Of_Example" },  { "label": "Row", "key": "users.row" },  { "label": "Occurrences", "key": "organizations.occurrences" },  { "label": "Event Status Name", "key": "organizations.eventStatusName" },  { "label": "Workflow Type", "key": "organizations.workflowType" },  { "label": "Status", "key": "organizations.status" },  { "label": "Workflow Type Name", "key": "organizations.workflowTypeName" },  {    "label": "Error Message Category",    "key": "organizations.errorMessageCategory"  },  {    "label": "Example Error Message",    "key": "organizations.exampleErrorMessage"  },  { "label": "Row ID Of Example", "key": "organizations.rowID_Of_Example" },  { "label": "Row", "key": "organizations.row" },  { "label": "Occurrences", "key": "classes.occurrences" },  { "label": "Event Status Name", "key": "classes.eventStatusName" },  { "label": "Workflow Type", "key": "classes.workflowType" },  { "label": "Status", "key": "classes.status" },  { "label": "Workflow Type Name", "key": "classes.workflowTypeName" },  { "label": "Error Message Category", "key": "classes.errorMessageCategory" },  { "label": "Example Error Message", "key": "classes.exampleErrorMessage" },  { "label": "Row ID Of Example", "key": "classes.rowID_Of_Example" },  { "label": "Row", "key": "classes.row" },  { "label": "Occurrences", "key": "enrollments.occurrences" },  { "label": "Event Status Name", "key": "enrollments.eventStatusName" },  { "label": "Workflow Type", "key": "enrollments.workflowType" },  { "label": "Status", "key": "enrollments.status" },  { "label": "Workflow Type Name", "key": "enrollments.workflowTypeName" },  { "label": "Error Message Category", "key": "enrollments.errorMessageCategory" },  { "label": "Example Error Message", "key": "enrollments.exampleErrorMessage" },  { "label": "Row ID Of Example", "key": "enrollments.rowID_Of_Example" },  { "label": "Row", "key": "enrollments.row" }]');
      `,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `
      DELETE
      FROM
      	"portal"."EmailTemplateType"
      WHERE
      	ContentProvider = 'FailedSynchronizationsCP_Service';
      `,
    );
  }
}
