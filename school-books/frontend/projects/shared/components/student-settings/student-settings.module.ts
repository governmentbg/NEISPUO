import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { StudentSettingsFormComponent } from './student-settings-form.component';

@NgModule({
  declarations: [StudentSettingsFormComponent],
  imports: [
    CommonModule,
    FontAwesomeWithConfigModule,
    MatButtonModule,
    MatDialogModule,
    MatSlideToggleModule,
    ReactiveFormsModule
  ],
  exports: [StudentSettingsFormComponent]
})
export class StudentSettingsModule {}
