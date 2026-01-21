import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from 'src/app/core/authentication/guards/auth.guard';
import { MunicipalInstitutionListPage } from './pages/municipal-institution-list/municipal-institution-list.page';
import { MiBulstatLoaderComponent } from './pages/mi-bulstat-loader/mi-bulstat-loader.component';
import { MiCreateNewComponent } from './pages/mi-create-new/mi-create-new.component';
import { MiCreationStepperComponent } from './pages/mi-creation-stepper/mi-creation-stepper.component';
import { MiEditExistingComponent } from './pages/mi-edit-existing/mi-edit-existing.component';
import { MiPreviewExistingComponent } from './pages/mi-preview-existing/mi-preview-existing.component';
import { MiDeleteExistingComponent } from './pages/mi-delete-existing/mi-delete-existing.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',

    data: {
      breadcrumb: 'Действащи институции',
      scope: 'active',
    },
    canActivate: [AuthGuard],

    component: MunicipalInstitutionListPage,
  },
  {
    path: 'closed',
    data: {
      breadcrumb: 'Закрити институции',
      scope: 'closed',
    },
    canActivate: [AuthGuard],

    component: MunicipalInstitutionListPage,

  },
  {
    path: 'edit/:id',
    data: {
      breadcrumb: 'Редактиране',
    },
    component: MiEditExistingComponent,
  },
  {
    path: 'preview/:id',
    data: {
      breadcrumb: 'Преглед',
    },
    canActivate: [AuthGuard],

    component: MiPreviewExistingComponent,
  },
  {
    path: 'create',
    data: {
      breadcrumb: 'Откриване',
    },
    component: MiCreationStepperComponent,
    children: [
      {
        path: 'bulstat-loader',
        canActivate: [AuthGuard],

        component: MiBulstatLoaderComponent,
      },
      {
        path: 'mi-create',
        canActivate: [AuthGuard],

        component: MiCreateNewComponent,
      },
    ],
  },
  {
    path: 'delete/:id',
    data: {
      breadcrumb: 'Закриване',
    },
    canActivate: [AuthGuard],

    component: MiDeleteExistingComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class MunicipalInstitutionRoutingModule { }
