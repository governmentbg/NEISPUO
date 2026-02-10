import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ExportButtonComponent } from './export-button.component';

@NgModule({
    declarations: [ExportButtonComponent],
    imports: [CommonModule, DialogModule, ButtonModule, TranslateModule],
    exports: [ExportButtonComponent],
})
export class ExportButtonModule {}
