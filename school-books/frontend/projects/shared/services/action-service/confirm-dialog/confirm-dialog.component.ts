import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TypedDialog } from 'projects/shared/utils/dialog';

export type ConfirmDialogData = {
  message: string;
  okBtnText?: string;
  okBtnHidden?: boolean;
  cancelBtnText?: string;
  cancelBtnHidden?: boolean;
};

export enum ConfirmDialogResult {
  Ok = 1,
  Cancel = 2
}

@Component({
  selector: 'sb-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.scss']
})
export class ConfirmDialogComponent implements TypedDialog<ConfirmDialogData, ConfirmDialogResult> {
  d!: ConfirmDialogData;
  r!: ConfirmDialogResult;

  readonly okResult = ConfirmDialogResult.Ok;
  readonly cancelResult = ConfirmDialogResult.Cancel;

  constructor(
    dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: ConfirmDialogData
  ) {
    dialogRef.disableClose = !!data.cancelBtnHidden;
  }
}
