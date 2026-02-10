import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ToastModule } from '@shared/components/toast/toast.module';
import { SharedModule } from '@shared/shared.module';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';
import { DashboardMenuComponent } from './components/dashboard-menu/dashboard-menu.component';
import { EmailTemplateListComponent } from './components/email-template-list/email-template-list.component';
import { EmailTemplatePreviewModalComponent } from './components/email-template-preview-modal/email-template-preview-modal.component';
import { EmailTemplateSendModalComponent } from './components/email-template-send-modal/email-template-send-modal.component';
import { EmailTemplateComponent } from './components/email-template/email-template.component';
import { NeispuoModuleCardComponent } from './components/neispuo-module-card/neispuo-module-card.component';
import { RefreshComponent } from './components/refresh/refresh.component';
import { SystemMessageFormComponent } from './components/system-message-form/system-message-form.component';
import { SystemUserMessagesComponent } from './components/system-user-messages/system-user-messages.component';
import { TagsComponent } from './components/tags/tags.component';
import { UserGuideManagementComponent } from './components/user-guide-management/user-guide-management.component';
import { UserGuidesMenuComponent } from './components/user-guides-menu/user-guides-menu.component';
import { UserMenuComponent } from './components/user-menu/user-menu.component';
import { UserProfileComponent } from './components/user-profile/user-profile.component';
import { NeispuoModuleService } from './neispuo-modules/neispuo-module.service';
import { ChildrenCodesPage } from './pages/children-codes/children-codes.page';
import { DashboardPage } from './pages/dashboard/dashboard.page';
import { PortalLayoutPage } from './pages/portal-layout/portal-layout.page';
import { PortalRoutingModule } from './portal-routing.module';
import { UserGuideManagementService } from './services/user-guide-management.service';

@NgModule({
  declarations: [
    PortalLayoutPage,
    DashboardPage,
    UserMenuComponent,
    DashboardMenuComponent,
    UserGuidesMenuComponent,
    BreadcrumbComponent,
    NeispuoModuleCardComponent,
    UserProfileComponent,
    ChildrenCodesPage,
    UserGuideManagementComponent,
    RefreshComponent,
    SystemUserMessagesComponent,
    TagsComponent,
    SystemMessageFormComponent,
    EmailTemplateComponent,
    EmailTemplateListComponent,
    EmailTemplateSendModalComponent,
    EmailTemplatePreviewModalComponent
  ],
  imports: [CommonModule, SharedModule, PortalRoutingModule, ToastModule],
  providers: [NeispuoModuleService, UserGuideManagementService]
})
export class PortalModule {}
