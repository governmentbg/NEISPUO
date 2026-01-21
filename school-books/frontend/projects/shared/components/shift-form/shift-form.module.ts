import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { BannerModule } from '../banner/banner.module';
import { CardSectionModule } from '../card-section/card-section.module';
import { NumberFieldModule } from '../number-field/number-field.module';
import { ShiftFormComponent } from './shift-form.component';
import { ShiftHoursFormComponent } from './shift-hours-form/shift-hours-form.component';

@NgModule({
  declarations: [ShiftFormComponent, ShiftHoursFormComponent],
  imports: [
    CommonModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatSelectModule,
    ReactiveFormsModule,
    BannerModule,
    CardSectionModule,
    NumberFieldModule,
    FontAwesomeModule
  ],
  exports: [ShiftFormComponent]
})
export class ShiftFormModule {}
