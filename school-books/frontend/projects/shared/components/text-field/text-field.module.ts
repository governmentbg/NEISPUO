import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { TextFieldComponent } from './text-field.component';

@NgModule({
  declarations: [TextFieldComponent],
  imports: [CommonModule, MatInputModule, ReactiveFormsModule],
  exports: [TextFieldComponent]
})
export class TextFieldModule {}
