import { NgModule } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  GradelessStudentsReportViewComponent,
  GradelessStudentsReportViewSkeletonComponent
} from './gradeless-students-report-view/gradeless-students-report-view.component';
import { GradelessStudentsReportsRoutingModule } from './gradeless-students-reports-routing.module';
import { GradelessStudentsReportsComponent } from './gradeless-students-reports/gradeless-students-reports.component';

@NgModule({
  declarations: [
    GradelessStudentsReportsComponent,
    GradelessStudentsReportViewComponent,
    GradelessStudentsReportViewSkeletonComponent
  ],
  imports: [
    GradelessStudentsReportsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    MatCheckboxModule,
    MatRadioModule,
    SelectFieldModule,
    NomSelectModule,
    BannerModule
  ]
})
export class GradelessStudentsReportsModule {}
