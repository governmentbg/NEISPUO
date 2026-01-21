import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  ScheduleAndAbsencesByTermReportViewComponent,
  ScheduleAndAbsencesByTermReportViewSkeletonComponent
} from './schedule-and-absences-by-term-report-view/schedule-and-absences-by-term-report-view.component';
import { ScheduleAndAbsencesByTermReportsRoutingModule } from './schedule-and-absences-by-term-reports-routing.module';
import { ScheduleAndAbsencesByTermReportsComponent } from './schedule-and-absences-by-term-reports/schedule-and-absences-by-term-reports.component';

@NgModule({
  declarations: [
    ScheduleAndAbsencesByTermReportsComponent,
    ScheduleAndAbsencesByTermReportViewComponent,
    ScheduleAndAbsencesByTermReportViewSkeletonComponent
  ],
  imports: [
    ScheduleAndAbsencesByTermReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule
  ]
})
export class ScheduleAndAbsencesByTermReportsModule {}
