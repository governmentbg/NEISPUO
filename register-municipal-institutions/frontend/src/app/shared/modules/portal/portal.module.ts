import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { PortalLayoutPage } from './pages/portal-layout/portal-layout.page';
import { PortalRoutingModule } from './portal-routing.module';
import { UserMenuComponent } from './components/user-menu/user-menu.component';
import { DashboardMenuComponent } from './components/dashboard-menu/dashboard-menu.component';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';

import { NeispuoModuleService } from './neispuo-modules/neispuo-module.service';

@NgModule({
  declarations: [
    PortalLayoutPage,
    UserMenuComponent,
    DashboardMenuComponent,
    BreadcrumbComponent,
  ],
  imports: [CommonModule, SharedModule, PortalRoutingModule],
  providers: [NeispuoModuleService],
})
export class PortalModule { }
