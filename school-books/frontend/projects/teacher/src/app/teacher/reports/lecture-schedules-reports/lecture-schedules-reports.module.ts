import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  LectureSchedulesReportViewComponent,
  LectureSchedulesReportViewSkeletonComponent
} from './lecture-schedules-report-view/lecture-schedules-report-view.component';
import { LectureSchedulesReportsRoutingModule } from './lecture-schedules-reports-routing.module';
import { LectureSchedulesReportsComponent } from './lecture-schedules-reports/lecture-schedules-reports.component';

@NgModule({
  declarations: [
    LectureSchedulesReportsComponent,
    LectureSchedulesReportViewComponent,
    LectureSchedulesReportViewSkeletonComponent
  ],
  imports: [
    LectureSchedulesReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule
  ]
})
export class LectureSchedulesReportsModule {}
