import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegBookQualificationComponent } from './reg-book-qualification/reg-book-qualification.component';

const routes: Routes = [{ path: '', component: RegBookQualificationComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RegBookQualificationRoutingModule {}
