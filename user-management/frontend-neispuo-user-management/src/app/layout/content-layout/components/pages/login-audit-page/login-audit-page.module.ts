import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { LoginAuditPageComponent } from './login-audit-page.component';

@NgModule({
    declarations: [LoginAuditPageComponent],
    imports: [SharedModule],
})
export class LoginAuditPageModule {}
