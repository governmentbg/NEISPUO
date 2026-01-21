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
import { TextareaFieldModule } from 'projects/shared/components/textarea-field/textarea-field.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { TeacherAbsenceViewDialogComponent } from './teacher-absence-view-dialog/teacher-absence-view-dialog.component';
import { TeacherAbsenceViewSelectHoursDialogComponent } from './teacher-absence-view-select-hours-dialog/teacher-absence-view-select-hours-dialog.component';
import {
  TeacherAbsenceViewComponent,
  TeacherAbsenceViewSkeletonComponent
} from './teacher-absence-view/teacher-absence-view.component';
import { TeacherAbsencesRoutingModule } from './teacher-absences-routing.module';
import { TeacherAbsencesComponent } from './teacher-absences/teacher-absences.component';
import { TeacherReplHoursViewDialogComponent } from './teacher-repl-hours-view-dialog/teacher-repl-hours-view-dialog.component';

@NgModule({
  declarations: [
    TeacherAbsencesComponent,
    TeacherAbsenceViewComponent,
    TeacherAbsenceViewDialogComponent,
    TeacherAbsenceViewSelectHoursDialogComponent,
    TeacherReplHoursViewDialogComponent,
    TeacherAbsenceViewSkeletonComponent
  ],
  imports: [
    TeacherAbsencesRoutingModule,
    ActionServiceModule,
    BannerModule,
    CommonFormUiModule,
    DateFieldModule,
    DatePipesModule,
    NomSelectModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    PaginatorModule,
    TextareaFieldModule,
    MatCheckboxModule
  ],
  providers: [DeactivateGuard]
})
export class TeacherAbsencesModule {}
