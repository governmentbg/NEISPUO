import { NgModule } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { PaginatorModule } from 'projects/shared/components/paginator/paginator.module';
import { TeacherSchedulesRoutingModule } from './teacher-schedules-routing.module';
import { TeacherSchedulesComponent } from './teacher-schedules/teacher-schedules.component';

@NgModule({
  declarations: [TeacherSchedulesComponent],
  imports: [
    TeacherSchedulesRoutingModule,
    CommonFormUiModule,
    NomSelectModule,
    PaginatorModule,
    MatProgressSpinnerModule,
    BannerModule
  ]
})
export class TeacherSchedulesModule {}
