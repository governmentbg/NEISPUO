import { Component } from '@angular/core';
import { faCalendarAlt as fadCalendarAlt } from '@fortawesome/pro-duotone-svg-icons/faCalendarAlt';

@Component({
  selector: 'sb-my-schedule',
  templateUrl: './my-schedule.component.html'
})
export class MyScheduleComponent {
  readonly fadCalendarAlt = fadCalendarAlt;
}
