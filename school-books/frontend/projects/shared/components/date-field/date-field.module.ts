import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { ClearInvalidDateDirective } from './clear-invalid-date.directive';
import { DateFieldComponent } from './date-field.component';
import { SbDateAdapter, SB_DATE_FORMATS } from './SbDateAdapter';
import { SbDatepickerIntl } from './SbDatepickerIntl';

@NgModule({
  declarations: [ClearInvalidDateDirective, DateFieldComponent],
  imports: [CommonModule, FontAwesomeWithConfigModule, MatDatepickerModule, MatInputModule, ReactiveFormsModule],
  exports: [DateFieldComponent],
  providers: [
    SbDatepickerIntl,
    { provide: MAT_DATE_FORMATS, useValue: SB_DATE_FORMATS },
    { provide: DateAdapter, useClass: SbDateAdapter }
  ]
})
export class DateFieldModule {}
