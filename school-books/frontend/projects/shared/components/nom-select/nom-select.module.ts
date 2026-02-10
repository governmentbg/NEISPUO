import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { NgSelectModule } from '@ng-select/ng-select';
import { NomSelectComponent } from './nom-select.component';

@NgModule({
  declarations: [NomSelectComponent],
  imports: [CommonModule, MatInputModule, NgSelectModule, ReactiveFormsModule],
  exports: [NomSelectComponent]
})
export class NomSelectModule {}
