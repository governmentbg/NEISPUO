import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogModule } from 'primeng/dialog';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ButtonModule } from 'primeng/button';
import { SpinnerComponent } from './spinner.component';

@NgModule({
    declarations: [SpinnerComponent],
    imports: [CommonModule, DialogModule, ProgressSpinnerModule, ButtonModule],
    exports: [SpinnerComponent],
})
export class SpinnerModule {}
