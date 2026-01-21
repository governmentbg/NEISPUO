import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  SessionStudentsReportViewComponent,
  SessionStudentsReportViewSkeletonComponent
} from './session-students-report-view/session-students-report-view.component';
import { SessionStudentsReportsRoutingModule } from './session-students-reports-routing.module';
import { SessionStudentsReportsComponent } from './session-students-reports/session-students-reports.component';

@NgModule({
  declarations: [
    SessionStudentsReportsComponent,
    SessionStudentsReportViewComponent,
    SessionStudentsReportViewSkeletonComponent
  ],
  imports: [
    SessionStudentsReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule
  ]
})
export class SessionStudentsReportsModule {}
