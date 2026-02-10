import { VariableMapping } from '../interfaces/variable-mapping.interface';
import { PLACEHOLDER_INSERTED_STYLES } from '../editor.config';

/**
 * Converts visual placeholder <span> elements back to storage format {{key}}.
 */
export function transformPlaceholdersToStorage(html: string): string {
  const tempDiv = document.createElement('div');
  tempDiv.innerHTML = html;
  const insertedElements = tempDiv.querySelectorAll('span.placeholder-inserted');
  insertedElements.forEach((el) => {
    const key = el.getAttribute('data-placeholder-key');
    if (key) {
      const placeholderNode = document.createTextNode(`{{${key}}}`);
      el.replaceWith(placeholderNode);
    }
  });
  return tempDiv.innerHTML;
}

/**
 * Converts {{key}} placeholders to visual <span> elements using provided variable mappings.
 */
export function transformPlaceholdersToEditor(html: string, variableMappings: VariableMapping[]): string {
  if (!variableMappings?.length) return html;
  let result = html;
  for (const variable of variableMappings) {
    const search = `{{${variable.key}}}`;
    const replacement = buildPlaceholderSpan(variable);
    result = result.split(search).join(replacement);
  }
  return result;
}

/**
 * Builds HTML span for a placeholder variable.
 */
export function buildPlaceholderSpan(variable: VariableMapping): string {
  return `<span class="placeholder-inserted" contenteditable="false" data-placeholder-key="${variable.key}" style="${PLACEHOLDER_INSERTED_STYLES}">${variable.label}</span>`;
} 