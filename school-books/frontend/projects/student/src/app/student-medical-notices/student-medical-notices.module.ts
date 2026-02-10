import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { PaginatorModule } from 'projects/shared/components/paginator/paginator.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { StudentMedicalNoticesRoutingModule } from './student-medical-notices-routing.module';
import {
  StudentMedicalNoticesComponent,
  StudentMedicalNoticesSkeletonComponent
} from './student-medical-notices.component';

@NgModule({
  declarations: [StudentMedicalNoticesComponent, StudentMedicalNoticesSkeletonComponent],
  imports: [StudentMedicalNoticesRoutingModule, CommonFormUiModule, BannerModule, DatePipesModule, PaginatorModule]
})
export class StudentMedicalNoticesModule {}
