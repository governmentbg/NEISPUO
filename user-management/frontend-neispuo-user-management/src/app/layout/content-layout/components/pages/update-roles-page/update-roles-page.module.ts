import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { UpdateRolesPageComponent } from './update-roles-page.component';
import { RoleAuditComponent } from './role-audit/role-audit.component';

@NgModule({
    declarations: [UpdateRolesPageComponent, RoleAuditComponent],
    imports: [SharedModule],
    exports: [UpdateRolesPageComponent],
})
export class UpdateRolesPageModule {}
