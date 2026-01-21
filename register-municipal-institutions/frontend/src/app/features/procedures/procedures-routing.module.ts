import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProcedureChoicePage } from './pages/procedure-choice/procedure-choice.page';

const routes: Routes = [
  {
    path: '',
    data: {
      breadcrumb: 'Преобразуване',
    },
    component: ProcedureChoicePage,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProceduresRoutingModule { }
