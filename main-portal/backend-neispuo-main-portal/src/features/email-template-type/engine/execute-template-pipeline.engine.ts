import { ModifierResult } from '../interfaces/modifier-result.interface';
import { TemplateEnv } from '../interfaces/template-env.interface';
import { TemplateModifier } from '../types/template-modifier.type';

/**
 * Run a sequence of template‐modifiers against a base template and environment.
 * Each modifier returns a (template, contextPatch) pair; we pass the updated template
 * to the next modifier and merge all contextPatches into a single context object.
 *
 * @param initialTemplate  The original Handlebars template string.
 * @param env              The TemplateEnv (rows, mappings, scalars).
 * @param modifiers        An array of TemplateModifier functions to apply in order.
 * @returns                An object containing the final template string and the merged context.
 */
export function executeTemplatePipeline(
  initialTemplate: string,
  env: TemplateEnv,
  modifiers: readonly TemplateModifier[],
): ModifierResult {
  // Start with the initial template and a shallow clone of scalars as the base context
  let template = initialTemplate;
  const context: Record<string, any> = { ...env.dataFetchResult.scalarValues };

  // Apply each modifier in sequence
  for (const modify of modifiers) {
    const { template: updatedTemplate, context: contextPatch } = modify(
      template,
      env,
    );
    template = updatedTemplate;
    Object.assign(context, contextPatch);
  }

  return { template, context };
}
