import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  ScheduleAndAbsencesByTermAllClassesReportViewComponent,
  ScheduleAndAbsencesByTermAllClassesReportViewSkeletonComponent
} from './schedule-and-absences-by-term-all-classes-report-view/schedule-and-absences-by-term-all-classes-report-view.component';
import { ScheduleAndAbsencesByTermAllClassesReportsRoutingModule } from './schedule-and-absences-by-term-all-classes-reports-routing.module';
import { ScheduleAndAbsencesByTermAllClassesReportsComponent } from './schedule-and-absences-by-term-all-classes-reports/schedule-and-absences-by-term-all-classes-reports.component';

@NgModule({
  declarations: [
    ScheduleAndAbsencesByTermAllClassesReportsComponent,
    ScheduleAndAbsencesByTermAllClassesReportViewComponent,
    ScheduleAndAbsencesByTermAllClassesReportViewSkeletonComponent
  ],
  imports: [
    ScheduleAndAbsencesByTermAllClassesReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule
  ]
})
export class ScheduleAndAbsencesByTermAllClassesReportsModule {}
