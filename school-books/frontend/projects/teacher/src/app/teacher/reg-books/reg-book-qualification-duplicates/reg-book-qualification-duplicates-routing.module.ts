import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegBookQualificationDuplicatesComponent } from './reg-book-qualification-duplicates/reg-book-qualification-duplicates.component';

const routes: Routes = [{ path: '', component: RegBookQualificationDuplicatesComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RegBookQualificationDuplicatesRoutingModule {}
