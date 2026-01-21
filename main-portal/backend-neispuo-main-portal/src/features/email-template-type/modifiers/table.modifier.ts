import { ModifierResult } from '../interfaces/modifier-result.interface';
import { TableContext } from '../interfaces/table-context.interface';
import { TemplateEnv } from '../interfaces/template-env.interface';

/**
 * Scans the input template for contiguous runs of “table‐tokens” (e.g. `{{users.id}}{{users.name}}`).
 * Replaces each run with a `{{> data-table table=tableN }}`
 * partial call and injects a corresponding `tableN` entry into the context.
 *
 * A “table‐token” is any Handlebars token whose key appears in `env.mappings` and includes a dot.
 *
 * @param template   The raw Handlebars template string.
 * @param env        The TemplateEnv containing:
 *                    - `datasets`: an array of merged row objects (each key is prefixed, e.g. “users.id”)
 *                    - `mappings`: a list of VariableMapping entries (label + full key)
 *                    - `scalars`: ordinary scalar variables
 * @returns          A ModifierResult containing:
 *                    - `template`: the modified template with partial calls
 *                    - `context`: an object containing `table1`, `table2`, … each with TableContext
 */
export function tableTokenModifier(
  template: string,
  env: TemplateEnv,
): ModifierResult {
  // Matches Handlebars tokens like {{users.field}} or {{organizations.status}}
  const tokenRegex = /{{\s*([0-9A-Za-z_.]+)\s*}}/g;

  // Map fullKey → mapping entry for label lookup
  const mappingIndex = new Map(env.mappings.map(m => [m.key, m]));

  // Set of all valid table-keys (those containing a prefix and a dot)
  const tableKeys = new Set(
    env.mappings.filter(m => m.key.includes('.')).map(m => m.key),
  );

  let resultTemplate = '';
  let lastCursor = 0;
  let lastMatchEnd = 0;
  let tableCount = 0;

  const context: Record<string, TableContext> = {};

  const currentRun: string[] = [];
  let runStartPos = 0;

  /**
   * Flush out the current run of table-keys:
   * 1. Build `tableN` with only rows that have non-null values for these keys.
   * 2. Insert the partial call into `resultTemplate`.
   * 3. Reset the run.
   */
  function flushRun(): void {
    if (currentRun.length === 0) {
      return;
    }

    tableCount += 1;
    const tableKey = `table${tableCount}`;

    // Filter to only rows that have at least one non-null value among currentRun keys
    const tableRows = env.dataFetchResult.tableValues
      .filter(row => currentRun.some(key => row[key] != null))
      .map(row => {
        // Build a minimal row object containing only the current keys
        const trimmed: Record<string, any> = {};
        for (const key of currentRun) {
          trimmed[key] = row[key];
        }
        return trimmed;
      });

    // Build column descriptors with labels from mappingIndex
    const columns = currentRun.map(key => ({
      key,
      label: mappingIndex.get(key)?.label ?? key,
    }));

    context[tableKey] = { columns, rows: tableRows };

    // Copy everything from lastCursor up to the start of this run,
    // then insert the partial call
    resultTemplate +=
      template.slice(lastCursor, runStartPos) +
      `{{> data-table table=${tableKey} }}`;

    // Advance the cursor past the tokens we just processed
    lastCursor = lastMatchEnd;
    currentRun.length = 0; // clear the run
  }

  // Iterate over every Handlebars token match in the template
  let match: RegExpExecArray | null;
  while ((match = tokenRegex.exec(template))) {
    const fullKey = match[1]; // e.g. "users.workflowType"
    const matchStart = match.index;
    const matchEnd = tokenRegex.lastIndex;

    const isTableToken = tableKeys.has(fullKey);
    const gapBetween = template.slice(lastMatchEnd, matchStart);

    if (isTableToken) {
      // If this is the first token in a run, record its start position
      if (currentRun.length === 0) {
        runStartPos = matchStart;
      } else if (/\S/.test(gapBetween)) {
        // We’ve hit real text ⇒ the old run ends, a new one begins
        flushRun();
        runStartPos = matchStart; // <-- start position for the new run
      }
      currentRun.push(fullKey);
    } else {
      // A non-table token always forces a flush of the current run
      flushRun();
    }

    lastMatchEnd = matchEnd;
  }

  // Flush any final run after the loop
  flushRun();

  // Append the remainder of the template after the last matched token
  resultTemplate += template.slice(lastCursor);

  return { template: resultTemplate, context };
}
