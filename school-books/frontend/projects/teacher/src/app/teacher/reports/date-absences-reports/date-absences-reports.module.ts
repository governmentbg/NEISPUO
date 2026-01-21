import { NgModule } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  DateAbsencesReportViewComponent,
  DateAbsencesReportViewSkeletonComponent
} from './date-absences-report-view/date-absences-report-view.component';
import { DateAbsencesReportsRoutingModule } from './date-absences-reports-routing.module';
import { DateAbsencesReportsComponent } from './date-absences-reports/date-absences-reports.component';

@NgModule({
  declarations: [
    DateAbsencesReportsComponent,
    DateAbsencesReportViewComponent,
    DateAbsencesReportViewSkeletonComponent
  ],
  imports: [
    DateAbsencesReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    MatCheckboxModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule
  ]
})
export class DateAbsencesReportsModule {}
