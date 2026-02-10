import { Injectable } from '@nestjs/common';
import { VariableMapping } from 'src/shared/interfaces/variable-mapping.interface';
import { HandlebarsService } from 'src/shared/services/handlebars/handlebars.service';
import { withTempConnection } from 'src/shared/utils/with-temp-connection';
import { Connection } from 'typeorm';
import { EventStatusEntity } from '../../azure-entities/event-status.entity';
import { WorkflowTypesEntity } from '../../azure-entities/workflow-types.entity';
import { EmailTemplateType } from '../../email-template-type.entity';
import { executeTemplatePipeline } from '../../engine/execute-template-pipeline.engine';
import { ContentProvider } from '../../interfaces/content-provider.interface';
import { DataFetchResult } from '../../interfaces/data-fetch-result.interface';
import { TemplateEnv } from '../../interfaces/template-env.interface';
import { tableTokenModifier } from '../../modifiers/table.modifier';
import { addRowIndex } from '../../utils/add-row-index.util';
import { VIEW_CONFIG } from './failed-synchronizations-constants';
import { ViewName } from './types/view-name.type';

@Injectable()
export class FailedSynchronizationsCP_Service implements ContentProvider {
  constructor(
    private readonly connection: Connection,
    private readonly handlebarsService: HandlebarsService,
  ) {}

  supports(template: EmailTemplateType) {
    return (
      template.contentProvider ===
      FailedSynchronizationsCP_Service.name
    );
  }

  async fetchData(fromDate: Date, toDate: Date) {
    const [users, organizations, classes, enrollments] = await Promise.all([
      this.getFailedSynchronizations('AzureUsersView', fromDate, toDate),
      this.getFailedSynchronizations(
        'AzureOrganizationsView',
        fromDate,
        toDate,
      ),
      this.getFailedSynchronizations(
        'AzureClassesView',
        fromDate,
        toDate,
        100000,
      ),
      this.getFailedSynchronizations(
        'AzureEnrollmentsView',
        fromDate,
        toDate,
        150000,
      ),
    ]);

    return {
      tableValues: [
        ...addRowIndex('users', users),
        ...addRowIndex('organizations', organizations),
        ...addRowIndex('classes', classes),
        ...addRowIndex('enrollments', enrollments),
      ],
      scalarValues: {
        startDate: fromDate,
        endDate: toDate,
      },
    };
  }

  hasSufficientData(data: DataFetchResult) {
    return data.tableValues.length > 0;
  }

  async getPopulatedTemplate(
    template: string,
    mappings: VariableMapping[],
    dataFetchResult: DataFetchResult,
  ): Promise<string> {
    const env: TemplateEnv = { dataFetchResult, mappings };

    const {
      template: modifiedTemplate,
      context,
    } = executeTemplatePipeline(template, env, [tableTokenModifier]);
    return this.handlebarsService.compile(modifiedTemplate, context);
  }

  private async getFailedSynchronizations(
    view: ViewName,
    start: Date,
    end: Date,
    /** If specified, this query will use a one‐off Connection with requestTimeout = timeoutMs */
    timeoutMs?: number,
  ): Promise<any[]> {
    const buildQuery = (conn: Connection) => {
      const cfg = VIEW_CONFIG[view];
      if (!cfg) {
        throw new Error(`Unsupported view "${view}"`);
      }

      const qb = conn
        .createQueryBuilder()
        .select(`COUNT(${cfg.alias}.RowID)`, `${cfg.alias}.occurrences`)
        .addSelect('es.Name', `${cfg.alias}.eventStatusName`)
        .addSelect(`${cfg.alias}.WorkflowType`, `${cfg.alias}.workflowType`)
        .addSelect(`${cfg.alias}.Status`, `${cfg.alias}.status`)
        .addSelect('wt.Name', `${cfg.alias}.workflowTypeName`)
        .addSelect(cfg.errorCategoryCase, `${cfg.alias}.errorMessageCategory`)
        .addSelect(
          `MIN(${cfg.alias}.ErrorMessage)`,
          `${cfg.alias}.exampleErrorMessage`,
        )
        .addSelect(`MIN(${cfg.alias}.RowID)`, `${cfg.alias}.rowID_Of_Example`);

      cfg.extraSelect?.forEach(expr => qb.addSelect(expr));

      qb.from(cfg.from, cfg.alias)
        .innerJoin(EventStatusEntity, 'es', `es.RowID = ${cfg.alias}.Status`)
        .innerJoin(
          WorkflowTypesEntity,
          'wt',
          `wt.RowID = ${cfg.alias}.WorkflowType`,
        );

      qb.where(`${cfg.alias}.CreatedOn BETWEEN :start AND :end`, { start, end })
        .andWhere(`${cfg.alias}.Status NOT IN (0,1,4)`)
        .andWhere(`${cfg.alias}.WorkflowType IN (:...wfs)`, {
          wfs: cfg.workflowTypes,
        });

      cfg.extraWhere?.forEach(expr => qb.andWhere(expr));

      qb.groupBy('es.Name')
        .addGroupBy(`${cfg.alias}.WorkflowType`)
        .addGroupBy(`${cfg.alias}.Status`)
        .addGroupBy('wt.Name')
        .addGroupBy(cfg.errorCategoryCase);

      cfg.extraGroupBy?.forEach(expr => qb.addGroupBy(expr));

      qb.orderBy(`${cfg.alias}.WorkflowType`)
        .addOrderBy(`'${cfg.alias}.occurrences'`, 'DESC')
        .addOrderBy(`'${cfg.alias}.errorMessageCategory'`)
        .addOrderBy(
          `CASE
           WHEN ${cfg.alias}.Status = 2   THEN 1
           WHEN ${cfg.alias}.Status = 3   THEN 2
           WHEN ${cfg.alias}.Status = 5   THEN 3
           WHEN ${cfg.alias}.Status = 6   THEN 4
           WHEN ${cfg.alias}.Status = 7   THEN 5
           WHEN ${cfg.alias}.Status = 8   THEN 6
           WHEN ${cfg.alias}.Status = 9   THEN 7
           WHEN ${cfg.alias}.Status = 10  THEN 8
           WHEN ${cfg.alias}.Status = 500 THEN 9
           ELSE 10
         END`,
        );

      return qb;
    };

    return timeoutMs
      ? withTempConnection(this.connection, timeoutMs, async conn => {
          const qb = buildQuery(conn);
          return qb.getRawMany();
        })
      : buildQuery(this.connection).getRawMany();
  }
}
