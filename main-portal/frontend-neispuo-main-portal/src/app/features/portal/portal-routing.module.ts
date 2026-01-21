import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PortalLayoutPage } from './pages/portal-layout/portal-layout.page';
import { DashboardPage } from './pages/dashboard/dashboard.page';
import { AuthGuard } from '@authentication/guards/auth.guard';
import { ChildrenCodesPage } from './pages/children-codes/children-codes.page';
import { ParentGuard } from '@authentication/guards/parent.guard';
import { MonAdminRoleGuard } from '@authentication/guards/mon-admin-role.guard';
import { UserGuideManagementGuard } from '@authentication/guards/user-guide-management.guard';
import { UserGuideManagementComponent } from './components/user-guide-management/user-guide-management.component';
import { SystemUserMessagesComponent } from './components/system-user-messages/system-user-messages.component';
import { EmailTemplateListComponent } from './components/email-template-list/email-template-list.component';
import { EmailTemplateComponent } from './components/email-template/email-template.component';

const routes: Routes = [
  {
    path: '',
    component: PortalLayoutPage,
    data: {
      breadcrumb: 'Табло за управление'
    },
    canActivate: [AuthGuard],
    children: [
      {
        path: 'children-codes',
        component: ChildrenCodesPage,
        data: {
          breadcrumb: 'Добави код'
        },
        canActivate: [ParentGuard]
      },
      {
        path: '',
        component: DashboardPage,
        data: {
          breadcrumb: 'Всички'
        }
      },

      {
        path: 'user-guide-management',
        component: UserGuideManagementComponent,
        data: {
          breadcrumb: 'Управление на ръководства'
        },
        canActivate: [UserGuideManagementGuard]
      },
      {
        path: 'system-user-messages',
        component: SystemUserMessagesComponent,
        data: {
          breadcrumb: 'Системни съобщения'
        },
        canActivate: [MonAdminRoleGuard]
      },
      {
        path: 'sync-messages',
        component: EmailTemplateListComponent,
        data: {
          breadcrumb: 'Съобщения за синхронизация'
        },
        canActivate: [MonAdminRoleGuard]
      },
      {
        path: 'sync-messages/create',
        component: EmailTemplateComponent,
        data: {
          breadcrumb: 'Ново съобщение'
        },
        canActivate: [MonAdminRoleGuard]
      },
      {
        path: 'sync-messages/edit/:id',
        component: EmailTemplateComponent,
        data: {
          breadcrumb: 'Редактиране на съобщение'
        },
        canActivate: [MonAdminRoleGuard]
      },
      {
        path: ':categoryId',
        component: DashboardPage,
        data: {
          breadcrumb: 'Всички'
        }
      },
      {
        path: '**',
        pathMatch: 'full',
        redirectTo: ''
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PortalRoutingModule {}
