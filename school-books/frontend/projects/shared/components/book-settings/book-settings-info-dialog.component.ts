import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { TypedDialog } from 'projects/shared/utils/dialog';

@Component({
  selector: 'sb-book-settings-info-dialog',
  templateUrl: './book-settings-info-dialog.component.html'
})
export class BookSettingsInfoDialogComponent implements TypedDialog<void, void> {
  d!: void;
  r!: void;

  constructor(private dialogRef: MatDialogRef<BookSettingsInfoDialogComponent>) {}
}
