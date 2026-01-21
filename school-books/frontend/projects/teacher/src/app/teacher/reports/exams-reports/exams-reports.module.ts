import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  ExamsReportViewComponent,
  ExamsReportViewSkeletonComponent
} from './exams-report-view/exams-report-view.component';
import { ExamsReportsRoutingModule } from './exams-reports-routing.module';
import { ExamsReportsComponent } from './exams-reports/exams-reports.component';

@NgModule({
  declarations: [ExamsReportsComponent, ExamsReportViewComponent, ExamsReportViewSkeletonComponent],
  imports: [
    ExamsReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    BannerModule
  ]
})
export class ExamsReportsModule {}
