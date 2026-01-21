import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { PaginatorModule } from 'projects/shared/components/paginator/paginator.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { StudentInfoBoardRoutingModule } from './student-info-board-routing.module';
import {
  StudentInfoBoardViewComponent,
  StudentInfoBoardViewSkeletonComponent
} from './student-info-board-view/student-info-board-view.component';
import {
  StudentInfoBoardComponent,
  StudentInfoBoardSkeletonComponent
} from './student-info-board/student-info-board.component';

@NgModule({
  declarations: [
    StudentInfoBoardComponent,
    StudentInfoBoardSkeletonComponent,
    StudentInfoBoardViewComponent,
    StudentInfoBoardViewSkeletonComponent
  ],
  imports: [StudentInfoBoardRoutingModule, CommonFormUiModule, BannerModule, DatePipesModule, PaginatorModule]
})
export class StudentInfoBoardModule {}
