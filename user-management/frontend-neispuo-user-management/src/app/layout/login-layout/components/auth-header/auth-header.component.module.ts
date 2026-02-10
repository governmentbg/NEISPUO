import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { AuthHeaderComponent } from './auth-header.component';

@NgModule({
    declarations: [AuthHeaderComponent],
    imports: [SharedModule],
    exports: [AuthHeaderComponent],
})
export class AuthHeaderComponentModule {}
