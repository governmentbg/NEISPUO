import * as Handlebars from 'handlebars';
import { formatDateHelper } from './format-date.helper';

/**
 * This function is responsible for registering **all** helpers at once.
 * Whenever you add a new helper file, simply import it here and add it to `allHelpers`.
 */
const allHelpers: Array<(hbs: typeof Handlebars) => void> = [formatDateHelper];

export function registerAllHelpers(hbs: typeof Handlebars) {
  allHelpers.forEach(fn => fn(hbs));
}
