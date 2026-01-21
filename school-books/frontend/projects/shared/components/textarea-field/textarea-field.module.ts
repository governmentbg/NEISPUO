import { TextFieldModule } from '@angular/cdk/text-field';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { TextareaFieldComponent } from './textarea-field.component';

@NgModule({
  declarations: [TextareaFieldComponent],
  imports: [CommonModule, MatInputModule, ReactiveFormsModule, TextFieldModule],
  exports: [TextareaFieldComponent]
})
export class TextareaFieldModule {}
