import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  MissingTopicsReportViewComponent,
  MissingTopicsReportViewSkeletonComponent
} from './missing-topics-report-view/missing-topics-report-view.component';
import { MissingTopicsReportsRoutingModule } from './missing-topics-reports-routing.module';
import { MissingTopicsReportsComponent } from './missing-topics-reports/missing-topics-reports.component';

@NgModule({
  declarations: [
    MissingTopicsReportsComponent,
    MissingTopicsReportViewComponent,
    MissingTopicsReportViewSkeletonComponent
  ],
  imports: [
    MissingTopicsReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    SelectFieldModule,
    NomSelectModule
  ]
})
export class MissingTopicsReportsModule {}
