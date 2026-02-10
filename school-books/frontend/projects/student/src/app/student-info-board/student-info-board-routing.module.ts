import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StudentInfoBoardViewSkeletonComponent } from './student-info-board-view/student-info-board-view.component';
import { StudentInfoBoardSkeletonComponent } from './student-info-board/student-info-board.component';

const routes: Routes = [
  { path: ':instId', component: StudentInfoBoardSkeletonComponent },
  { path: ':instId/:publicationId', component: StudentInfoBoardViewSkeletonComponent },
  { path: '**', redirectTo: '/not-found' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StudentInfoBoardRoutingModule {}
