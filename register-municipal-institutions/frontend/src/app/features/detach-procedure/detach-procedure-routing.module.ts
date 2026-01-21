import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DetachProcedureStepperMenuPage } from './components/detach-procedure-stepper-menu/detach-procedure-stepper-menu.page';
import { DetachMIsToCreateComponent } from './pages/detach-mis-to-create/detach-mis-to-create.component';
import { DetachConfirmationComponent } from './pages/detach-confirmation/detach-confirmation.component';
import { DetachMIToUpdateComponent } from './pages/detach-mi-to-update/detach-mi-to-update.component';
import { DetachRiDocumentComponent } from './pages/detach-ri-document/detach-ri-document.component';

const routes: Routes = [
  {
    path: '',
    data: {
      breadcrumb: 'Отделяне',
    },
    component: DetachProcedureStepperMenuPage,
    children: [
      {
        path: 'mi-to-update',
        component: DetachMIToUpdateComponent,
      },

      {
        path: 'mis-to-create',
        component: DetachMIsToCreateComponent,
      },
      {
        path: 'ri-document',
        component: DetachRiDocumentComponent,
      },
      {
        path: 'confirm',
        component: DetachConfirmationComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DetachProceduresRoutingModule { }
