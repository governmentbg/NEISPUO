import { VariableMapping } from 'src/shared/interfaces/variable-mapping.interface';
import { EmailTemplateType } from '../email-template-type.entity';
import { DataFetchResult } from './data-fetch-result.interface';

export interface ContentProvider {
  /**
   * Should return `true` if this provider can handle the given template.
   */
  supports(template: EmailTemplateType): boolean;

  /**
   * Load, transform, and prefix *all* data needed to render
   * a template for the given time window.
   *
   * @param fromDate  Inclusive start of the reporting period.
   * @param toDate    Inclusive end of the reporting period.
   * @returns         A `DataFetchResult` containing:
   *                  - `tableValues`: rows for any auto-built tables
   *                  - `scalarValues`: global variables for the template
   */
  fetchData(fromDate: Date, toDate: Date): Promise<DataFetchResult>;

  /**
   * Inspect the result of `fetchData()` and return:
   *  - `true`  if there is **enough** data to continue rendering,
   *  - `false` if this run should be skipped (nothing to send).
   *
   * @param data  The `DataFetchResult` returned by `fetchData()`.
   */
  hasSufficientData(data: DataFetchResult): boolean;

  /**
   * Render the final HTML by:
   * 1. Running any template-modifiers (e.g. table injection).
   * 2. Compiling and executing the resulting Handlebars template
   *    with the merged context.
   *
   * @param hbsSource         The raw Handlebars template string.
   * @param mappings          VariableMapping list defining each
   *                          placeholder’s label and key.
   * @param dataFetchResult   The object returned from `fetchData()`,
   *                          containing `tableValues` and `scalarValues`.
   * @returns                 A Promise that resolves to the fully-rendered
   *                          HTML string, ready for email delivery.
   */
  getPopulatedTemplate(
    hbsSource: string,
    mappings: VariableMapping[],
    dataFetchResult: DataFetchResult,
  ): Promise<string>;
}
