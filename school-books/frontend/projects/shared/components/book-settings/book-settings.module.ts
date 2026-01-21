import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { BookSettingsFormComponent } from './book-settings-form.component';
import { BookSettingsInfoDialogComponent } from './book-settings-info-dialog.component';

@NgModule({
  declarations: [BookSettingsFormComponent, BookSettingsInfoDialogComponent],
  imports: [
    CommonModule,
    FontAwesomeWithConfigModule,
    MatButtonModule,
    MatDialogModule,
    MatSlideToggleModule,
    ReactiveFormsModule
  ],
  exports: [BookSettingsFormComponent]
})
export class BookSettingsModule {}
