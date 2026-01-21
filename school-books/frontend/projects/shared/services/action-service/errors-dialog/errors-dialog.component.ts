import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TypedDialog } from 'projects/shared/utils/dialog';

export type ErrorsDialogData = {
  header: string;
  errorMessages: string[];
};

@Component({
  selector: 'sb-errors-dialog',
  templateUrl: './errors-dialog.component.html'
})
export class ErrorsDialogComponent implements TypedDialog<ErrorsDialogData, void> {
  d!: ErrorsDialogData;
  r!: void;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: ErrorsDialogData
  ) {}
}
