import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  AbsencesByClassesReportViewComponent,
  AbsencesByClassesReportViewSkeletonComponent
} from './absences-by-classes-report-view/absences-by-classes-report-view.component';
import { AbsencesByClassesReportsRoutingModule } from './absences-by-classes-reports-routing.module';
import { AbsencesByClassesReportsComponent } from './absences-by-classes-reports/absences-by-classes-reports.component';

@NgModule({
  declarations: [
    AbsencesByClassesReportsComponent,
    AbsencesByClassesReportViewComponent,
    AbsencesByClassesReportViewSkeletonComponent
  ],
  imports: [
    AbsencesByClassesReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule
  ]
})
export class AbsencesByClassesReportsModule {}
