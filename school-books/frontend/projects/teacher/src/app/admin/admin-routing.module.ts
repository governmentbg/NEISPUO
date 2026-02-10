import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SnackbarRootComponent } from 'projects/shared/components/snackbar-root/snackbar-root.component';
import { AdminComponent } from './admin.component';

const routes: Routes = [
  {
    path: '',
    component: SnackbarRootComponent,
    children: [
      {
        path: '',
        component: AdminComponent,
        children: [
          {
            path: 'institutions',
            loadChildren: () => import('./institutions/institutions.module').then((m) => m.InstitutionsModule)
          },
          {
            path: 'his-medical-notices',
            loadChildren: () =>
              import('./his-medical-notices/his-medical-notices.module').then((m) => m.HisMedicalNoticesModule)
          }
        ]
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule {}
