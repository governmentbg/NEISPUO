import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { PlaceholderFieldComponent } from './placeholder-field.component';

@NgModule({
  declarations: [PlaceholderFieldComponent],
  imports: [CommonModule, MatInputModule],
  exports: [PlaceholderFieldComponent]
})
export class PlaceholderFieldModule {}
