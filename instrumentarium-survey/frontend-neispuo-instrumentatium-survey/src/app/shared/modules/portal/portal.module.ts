import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PortalLayoutPage } from './pages/portal-layout/portal-layout.page';
import { SharedModule } from '@shared/shared.module';
import { PortalRoutingModule } from './portal-routing.module';
import { UserMenuComponent } from './components/user-menu/user-menu.component';
import { DashboardMenuComponent } from './components/dashboard-menu/dashboard-menu.component';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';

@NgModule({
  declarations: [
    PortalLayoutPage,
    UserMenuComponent,
    DashboardMenuComponent,
    BreadcrumbComponent,
  ],
  imports: [CommonModule, SharedModule, PortalRoutingModule],
  providers: []
})
export class PortalModule { }
