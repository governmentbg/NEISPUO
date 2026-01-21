import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { ActionService } from './action.service';
import { ConfirmDialogComponent } from './confirm-dialog/confirm-dialog.component';
import { ErrorsDialogComponent } from './errors-dialog/errors-dialog.component';

@NgModule({
  declarations: [ConfirmDialogComponent, ErrorsDialogComponent],
  imports: [CommonModule, MatButtonModule, MatDialogModule],
  exports: [],
  providers: [{ provide: ActionService }]
})
export class ActionServiceModule {}
