import { Injectable } from '@angular/core';
import { MatDatepickerIntl } from '@angular/material/datepicker';

@Injectable({ providedIn: 'root' })
export class SbDatepickerIntl extends MatDatepickerIntl {
  /** A label for the calendar popup (used by screen readers). */
  calendarLabel = 'Календар';

  /** A label for the button used to open the calendar popup (used by screen readers). */
  openCalendarLabel = 'Отвори календара';

  /** Label for the button used to close the calendar popup. */
  closeCalendarLabel = 'Затвори календара';

  /** A label for the previous month button (used by screen readers). */
  prevMonthLabel = 'Предходен месец';

  /** A label for the next month button (used by screen readers). */
  nextMonthLabel = 'Следващ месец';

  /** A label for the previous year button (used by screen readers). */
  prevYearLabel = 'Предходна година';

  /** A label for the next year button (used by screen readers). */
  nextYearLabel = 'Следваща година';

  /** A label for the previous multi-year button (used by screen readers). */
  prevMultiYearLabel = 'Предходни 20 години';

  /** A label for the next multi-year button (used by screen readers). */
  nextMultiYearLabel = 'Следващи 20 години';

  /** A label for the 'switch to month view' button (used by screen readers). */
  switchToMonthViewLabel = 'Избери дата';

  /** A label for the 'switch to year view' button (used by screen readers). */
  switchToMultiYearViewLabel = 'Избери месец и година';
}
