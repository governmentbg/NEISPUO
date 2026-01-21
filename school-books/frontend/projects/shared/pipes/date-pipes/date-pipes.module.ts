import { NgModule } from '@angular/core';
import { DateTimePipe } from './date-time.pipe';
import { DatePipe } from './date.pipe';
import { TimeAgoPipe } from './time-ago-pipe';

@NgModule({
  declarations: [DatePipe, DateTimePipe, TimeAgoPipe],
  imports: [],
  exports: [DatePipe, DateTimePipe, TimeAgoPipe]
})
export class DatePipesModule {}
