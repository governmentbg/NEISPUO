import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from 'src/app/core/authentication/guards/auth.guard';
import { PortalLayoutPage } from '@shared/modules/portal/pages/portal-layout/portal-layout.page';
import { DivideProcedureStepperMenuPage } from './components/divide-procedure-stepper-menu/divide-procedure-stepper-menu.page';
import { DivideMIToDeleteComponent } from './pages/divide-mi-to-delete/divide-mi-to-delete.component';
import { DivideMIsToCreateComponent } from './pages/divide-mis-to-create/divide-mis-to-create.component';
import { DivideConfirmationComponent } from './pages/divide-confirmation/divide-confirmation.component';
import { DivideRiDocumentComponent } from './pages/divide-ri-document/divide-ri-document.component';

const routes: Routes = [
  {
    path: '',
    data: {
      breadcrumb: 'Разделяне',
    },
    component: DivideProcedureStepperMenuPage,
    children: [
      {
        path: 'mi-to-delete',
        component: DivideMIToDeleteComponent,
      },

      {
        path: 'mis-to-create',
        component: DivideMIsToCreateComponent,
      },
      {
        path: 'ri-document',
        component: DivideRiDocumentComponent,
      },

      {
        path: 'confirm',
        component: DivideConfirmationComponent,
      },
    ],
  },
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DivideProceduresRoutingModule { }
