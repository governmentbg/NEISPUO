import { NgModule } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { PaginatorModule } from 'projects/shared/components/paginator/paginator.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { LectureScheduleViewDialogComponent } from './lecture-schedule-view-dialog/lecture-schedule-view-dialog.component';
import {
  LectureScheduleViewComponent,
  LectureScheduleViewSkeletonComponent
} from './lecture-schedule-view/lecture-schedule-view.component';
import { LectureSchedulesRoutingModule } from './lecture-schedules-routing.module';
import { LectureSchedulesComponent } from './lecture-schedules/lecture-schedules.component';

@NgModule({
  declarations: [
    LectureSchedulesComponent,
    LectureScheduleViewComponent,
    LectureScheduleViewDialogComponent,
    LectureScheduleViewSkeletonComponent
  ],
  imports: [
    LectureSchedulesRoutingModule,
    ActionServiceModule,
    BannerModule,
    CommonFormUiModule,
    DateFieldModule,
    DatePipesModule,
    NomSelectModule,
    MatCheckboxModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    PaginatorModule,
    SelectFieldModule
  ],
  providers: [DeactivateGuard]
})
export class LectureSchedulesModule {}
