import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PortalLayoutPage } from '../../shared/modules/portal/pages/portal-layout/portal-layout.page';
import { AuthGuard } from '../../core/authentication/guards/auth.guard';
import { FlexFieldDetailPage } from './pages/flex-field-detail/flex-field-detail.page';
import { FlexFieldListPage } from './pages/flex-field-list/flex-field-list.page';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    data: {
      breadcrumb: 'Списък',
    },
    canActivate: [AuthGuard],
    component: FlexFieldListPage,
  },
  {
    path: 'new',
    data: {
      breadcrumb: 'Създай Ново',
    },
    canActivate: [AuthGuard],
    component: FlexFieldDetailPage,
  },
  {
    path: 'edit/:id',
    data: {
      breadcrumb: 'Редактиране',
    },
    canActivate: [AuthGuard],
    component: FlexFieldDetailPage,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class FlexFieldsRoutingModule { }
