import { Injectable } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";

@Injectable()
export class SnackbarService {
  constructor(private snackBar: MatSnackBar) {}

  openErrorSnackbar(message: string) {
    this.openSnackbar(message, "error")
  }

  openSuccessSnackbar(message: string) {
    this.openSnackbar(message, "success")
  }

  private openSnackbar(message: string, panelClass: string) {
    this.snackBar.open(message, "x", {
      verticalPosition: "top",
      panelClass,
      duration: 5000
    });
  }
}
