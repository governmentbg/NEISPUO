import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StudentInfoSkeletonComponent } from './student-info/student-info.component';

const routes: Routes = [
  {
    path: ':classBookId/:personId',
    component: StudentInfoSkeletonComponent,
    children: [
      // empty route
      { path: '', children: [] },
      // student-book
      {
        path: 'student-book',
        loadChildren: () => import('./student-book/student-book.module').then((m) => m.StudentBookModule)
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StudentInfoRoutingModule {}
