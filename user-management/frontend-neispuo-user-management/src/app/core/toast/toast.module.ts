import { NgModule } from '@angular/core';
import { ToastModule as TModule } from 'primeng/toast';
import { ToastComponent } from './toast.component';

@NgModule({
    declarations: [ToastComponent],
    imports: [TModule],
    exports: [ToastComponent],
})
export class ToastModule {}
