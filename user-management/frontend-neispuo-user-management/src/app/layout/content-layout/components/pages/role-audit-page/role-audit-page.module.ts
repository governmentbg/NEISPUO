import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { RoleAuditPageComponent } from './role-audit-page.component';

@NgModule({
    declarations: [RoleAuditPageComponent],
    imports: [SharedModule],
})
export class RoleAuditPageModule {}
