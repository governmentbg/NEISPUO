import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  AbsencesByStudentsReportViewComponent,
  AbsencesByStudentsReportViewSkeletonComponent
} from './absences-by-students-report-view/absences-by-students-report-view.component';
import { AbsencesByStudentsReportsRoutingModule } from './absences-by-students-reports-routing.module';
import { AbsencesByStudentsReportsComponent } from './absences-by-students-reports/absences-by-students-reports.component';

@NgModule({
  declarations: [
    AbsencesByStudentsReportsComponent,
    AbsencesByStudentsReportViewComponent,
    AbsencesByStudentsReportViewSkeletonComponent
  ],
  imports: [
    AbsencesByStudentsReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule
  ]
})
export class AbsencesByStudentsReportsModule {}
