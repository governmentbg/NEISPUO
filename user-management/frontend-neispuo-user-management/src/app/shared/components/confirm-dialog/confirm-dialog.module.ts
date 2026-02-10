import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { ConfirmDialogModule as CDM } from 'primeng/confirmdialog';
import { ConfirmDialogComponent } from './confirm-dialog.component';

@NgModule({
    declarations: [ConfirmDialogComponent],
    imports: [CommonModule, CDM, TranslateModule],
    exports: [ConfirmDialogComponent],
})
export class ConfirmDialogModule {}
