import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { NumberFieldComponent } from './number-field.component';
import { NumberPrecisionDirective } from './number-precision.directive';
import { NumberDirective } from './number.directive';

@NgModule({
  declarations: [NumberDirective, NumberFieldComponent, NumberPrecisionDirective],
  imports: [CommonModule, MatInputModule, ReactiveFormsModule],
  exports: [NumberFieldComponent, NumberDirective, NumberPrecisionDirective]
})
export class NumberFieldModule {}
