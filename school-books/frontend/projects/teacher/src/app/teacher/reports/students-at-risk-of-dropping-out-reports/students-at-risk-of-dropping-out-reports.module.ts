import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  StudentsAtRiskOfDroppingOutReportViewComponent,
  StudentsAtRiskOfDroppingOutReportViewSkeletonComponent
} from './students-at-risk-of-dropping-out-report-view/students-at-risk-of-dropping-out-report-view.component';
import { StudentsAtRiskOfDroppingOutReportsRoutingModule } from './students-at-risk-of-dropping-out-reports-routing.module';
import { StudentsAtRiskOfDroppingOutReportsComponent } from './students-at-risk-of-dropping-out-reports/students-at-risk-of-dropping-out-reports.component';

@NgModule({
  declarations: [
    StudentsAtRiskOfDroppingOutReportsComponent,
    StudentsAtRiskOfDroppingOutReportViewComponent,
    StudentsAtRiskOfDroppingOutReportViewSkeletonComponent
  ],
  imports: [
    StudentsAtRiskOfDroppingOutReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule
  ]
})
export class StudentsAtRiskOfDroppingOutReportsModule {}
