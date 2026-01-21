import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ToastComponent } from './toast.component';

@NgModule({
    declarations: [ToastComponent],
    imports: [CommonModule],
    exports: [ToastComponent]
})
export class ToastModule {}
