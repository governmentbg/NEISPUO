import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { PaginatorModule } from 'projects/shared/components/paginator/paginator.module';
import { SimpleSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-skeleton-template.module';
import {
  MyScheduleContentComponent,
  MyScheduleContentSkeletonComponent
} from './my-schedule-content/my-schedule-content.component';
import { MyScheduleRoutingModule } from './my-schedule-routing.module';
import { MyScheduleComponent } from './my-schedule/my-schedule.component';

@NgModule({
  declarations: [MyScheduleComponent, MyScheduleContentSkeletonComponent, MyScheduleContentComponent],
  imports: [MyScheduleRoutingModule, CommonFormUiModule, PaginatorModule, SimpleSkeletonTemplateModule]
})
export class MyScheduleModule {}
