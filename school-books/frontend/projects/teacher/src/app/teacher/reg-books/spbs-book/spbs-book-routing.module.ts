import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SpbsBookViewSkeletonComponent } from './spbs-book-view/spbs-book-view.component';
import { SpbsBookComponent } from './spbs-book/spbs-book.component';

const routes: Routes = [
  { path: '', component: SpbsBookComponent },
  { path: ':recordSchoolYear/:spbsBookRecordId', component: SpbsBookViewSkeletonComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SpbsBookRoutingModule {}
