import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { SelectFieldComponent } from './select-field.component';

@NgModule({
  declarations: [SelectFieldComponent],
  imports: [CommonModule, MatInputModule, MatSelectModule, ReactiveFormsModule],
  exports: [SelectFieldComponent]
})
export class SelectFieldModule {}
