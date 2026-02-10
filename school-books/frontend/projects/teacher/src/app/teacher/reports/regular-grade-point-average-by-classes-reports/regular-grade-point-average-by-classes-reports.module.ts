import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  RegularGradePointAverageByClassesReportViewComponent,
  RegularGradePointAverageByClassesReportViewSkeletonComponent
} from './regular-grade-point-average-by-classes-report-view/regular-grade-point-average-by-classes-report-view.component';
import { RegularGradePointAverageByClassesReportsRoutingModule } from './regular-grade-point-average-by-classes-reports-routing.module';
import { RegularGradePointAverageByClassesReportsComponent } from './regular-grade-point-average-by-classes-reports/regular-grade-point-average-by-classes-reports.component';

@NgModule({
  declarations: [
    RegularGradePointAverageByClassesReportsComponent,
    RegularGradePointAverageByClassesReportViewComponent,
    RegularGradePointAverageByClassesReportViewSkeletonComponent
  ],
  imports: [
    RegularGradePointAverageByClassesReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule
  ]
})
export class RegularGradePointAverageByClassesReportsModule {}
