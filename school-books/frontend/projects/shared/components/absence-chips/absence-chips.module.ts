import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatChipsModule } from '@angular/material/chips';
import { AbsenceChipsComponent } from './absence-chips.component';

@NgModule({
  declarations: [AbsenceChipsComponent],
  imports: [CommonModule, MatChipsModule],
  exports: [AbsenceChipsComponent]
})
export class AbsenceChipsModule {}
