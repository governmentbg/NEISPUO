import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from 'src/app/core/authentication/guards/auth.guard';
import { PortalLayoutPage } from '@shared/modules/portal/pages/portal-layout/portal-layout.page';
import { JoinProcedureStepperMenuPage } from './components/join-procedure-stepper-menu/join-procedure-stepper-menu.page';
import { JoinMIsToDeleteComponent } from './pages/join-mis-to-delete/join-mis-to-delete.component';
import { JoinMIToUpdateComponent } from './pages/join-mi-to-update/join-mi-to-update.component';
import { JoinConfirmationComponent } from './pages/join-confirmation/join-confirmation.component';
import { JoinRiDocumentComponent } from './pages/join-ri-document/join-ri-document.component';

const routes: Routes = [
  {
    path: '',
    data: {
      breadcrumb: 'Вливане',
    },
    component: JoinProcedureStepperMenuPage,
    children: [
      {
        path: 'mis-to-delete',
        component: JoinMIsToDeleteComponent,
        data: {
          type: 'join',
        },
      },

      {
        path: 'mi-to-update',
        component: JoinMIToUpdateComponent,
        data: {
          type: 'join',
        },
      },
      {
        path: 'ri-document',
        component: JoinRiDocumentComponent,
      },
      {
        path: 'confirm',
        component: JoinConfirmationComponent,
        data: {
          type: 'join',
        },
      },
    ],
  },
]
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class JoinProceduresRoutingModule { }
