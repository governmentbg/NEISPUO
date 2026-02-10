import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  ScheduleAndAbsencesByMonthReportViewComponent,
  ScheduleAndAbsencesByMonthReportViewSkeletonComponent
} from './schedule-and-absences-by-month-report-view/schedule-and-absences-by-month-report-view.component';
import { ScheduleAndAbsencesByMonthReportsRoutingModule } from './schedule-and-absences-by-month-reports-routing.module';
import { ScheduleAndAbsencesByMonthReportsComponent } from './schedule-and-absences-by-month-reports/schedule-and-absences-by-month-reports.component';

@NgModule({
  declarations: [
    ScheduleAndAbsencesByMonthReportsComponent,
    ScheduleAndAbsencesByMonthReportViewComponent,
    ScheduleAndAbsencesByMonthReportViewSkeletonComponent
  ],
  imports: [
    ScheduleAndAbsencesByMonthReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule
  ]
})
export class ScheduleAndAbsencesByMonthReportsModule {}
