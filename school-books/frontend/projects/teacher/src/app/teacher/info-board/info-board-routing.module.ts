import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InfoBoardViewSkeletonComponent } from './info-board-view/info-board-view.component';
import { InfoBoardSkeletonComponent } from './info-board/info-board.component';

const routes: Routes = [
  { path: '', component: InfoBoardSkeletonComponent },
  { path: ':publicationId', component: InfoBoardViewSkeletonComponent },
  { path: '**', redirectTo: '/not-found' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InfoBoardRoutingModule {}
