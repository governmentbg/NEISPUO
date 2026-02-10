import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { NumberFieldModule } from 'projects/shared/components/number-field/number-field.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { GradePipesModule } from 'projects/shared/pipes/grade-pipes/grade-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  FinalGradePointAverageByStudentsReportViewComponent,
  FinalGradePointAverageByStudentsReportViewSkeletonComponent
} from './final-grade-point-average-by-students-report-view/final-grade-point-average-by-students-report-view.component';
import { FinalGradePointAverageByStudentsReportsRoutingModule } from './final-grade-point-average-by-students-reports-routing.module';
import { FinalGradePointAverageByStudentsReportsComponent } from './final-grade-point-average-by-students-reports/final-grade-point-average-by-students-reports.component';

@NgModule({
  declarations: [
    FinalGradePointAverageByStudentsReportsComponent,
    FinalGradePointAverageByStudentsReportViewComponent,
    FinalGradePointAverageByStudentsReportViewSkeletonComponent
  ],
  imports: [
    FinalGradePointAverageByStudentsReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule,
    NumberFieldModule,
    GradePipesModule
  ]
})
export class FinalGradePointAverageByStudentsReportsModule {}
