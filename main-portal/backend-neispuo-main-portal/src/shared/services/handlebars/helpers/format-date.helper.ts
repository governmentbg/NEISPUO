/**
 * Registers the `formatDate` helper with Handlebars.
 * Usage within templates: `{{formatDate someDate}}`
 */
export const formatDateHelper = (hbs: typeof Handlebars) => {
  hbs.registerHelper('formatDate', function(date?: Date) {
    if (!date) return '';

    // util to pad any number to 2 digits
    const pad = (num: number) => String(num).padStart(2, '0');

    const day = pad(date.getDate());
    const month = pad(date.getMonth() + 1);
    const year = date.getFullYear();

    const hours = pad(date.getHours());
    const minutes = pad(date.getMinutes());
    const seconds = pad(date.getSeconds());

    return `${day}.${month}.${year} ${hours}:${minutes}:${seconds}`;
  });
};
