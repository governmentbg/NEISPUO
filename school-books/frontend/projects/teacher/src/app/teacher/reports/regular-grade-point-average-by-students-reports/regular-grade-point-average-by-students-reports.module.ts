import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  RegularGradePointAverageByStudentsReportViewComponent,
  RegularGradePointAverageByStudentsReportViewSkeletonComponent
} from './regular-grade-point-average-by-students-report-view/regular-grade-point-average-by-students-report-view.component';
import { RegularGradePointAverageByStudentsReportsRoutingModule } from './regular-grade-point-average-by-students-reports-routing.module';
import { RegularGradePointAverageByStudentsReportsComponent } from './regular-grade-point-average-by-students-reports/regular-grade-point-average-by-students-reports.component';

@NgModule({
  declarations: [
    RegularGradePointAverageByStudentsReportsComponent,
    RegularGradePointAverageByStudentsReportViewComponent,
    RegularGradePointAverageByStudentsReportViewSkeletonComponent
  ],
  imports: [
    RegularGradePointAverageByStudentsReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule
  ]
})
export class RegularGradePointAverageByStudentsReportsModule {}
