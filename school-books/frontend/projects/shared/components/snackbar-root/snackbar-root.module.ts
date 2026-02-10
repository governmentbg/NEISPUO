import { NgModule } from '@angular/core';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { RouterModule } from '@angular/router';
import { SnackbarRootComponent } from './snackbar-root.component';

@NgModule({
  declarations: [SnackbarRootComponent],
  imports: [RouterModule, MatSnackBarModule]
})
export class SnackbarRootModule {}
