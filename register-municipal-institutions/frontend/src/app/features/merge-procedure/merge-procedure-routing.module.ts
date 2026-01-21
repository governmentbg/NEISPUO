import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from 'src/app/core/authentication/guards/auth.guard';
import { PortalLayoutPage } from '@shared/modules/portal/pages/portal-layout/portal-layout.page';
import { MergeProcedureStepperMenuPage } from './components/merge-procedure-stepper-menu/merge-procedure-stepper-menu.page';
import { MergeMIsToDeleteComponent } from './pages/merge-mis-to-delete/merge-mis-to-delete.component';
import { MergeMIToCreateComponent } from './pages/merge-mi-to-create/merge-mi-to-create.component';
import { MergeConfirmationComponent } from './pages/merge-confirmation/merge-confirmation.component';
import { MergeRiDocumentComponent } from './pages/merge-ri-document/merge-ri-document.component';

const routes: Routes = [
  {
    path: '',
    data: {
      breadcrumb: 'Сливане',
    },
    component: MergeProcedureStepperMenuPage,
    children: [
      {
        path: 'mis-to-delete',
        component: MergeMIsToDeleteComponent,
      },

      {
        path: 'mi-to-create',
        component: MergeMIToCreateComponent,
      },
      {
        path: 'ri-document',
        component: MergeRiDocumentComponent,
      },
      {
        path: 'confirm',
        component: MergeConfirmationComponent,
      },
    ],
  },
]


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class MergeProceduresRoutingModule { }
